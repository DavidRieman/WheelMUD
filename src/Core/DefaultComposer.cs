//-----------------------------------------------------------------------------
// <copyright file="DefaultComposer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using WheelMUD.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>A class for simplifying common composition tasks.</summary>
    /// <remarks>
    /// TODO: Migrate this documentation to .md and reference that instead from Priority members? (And link it from other docs.)
    /// 
    /// WheelMUD uses an extensive composition system to bring together various parts and systems dynamically.
    /// 
    /// The main goals of this system are to:
    /// * Allow game customizations to occur independently from "core" code.
    /// * Allow easy "plugging in" of external parts and systems.
    /// * Allow for "recomposition" scenarios.
    /// These goals help support additional benefits:
    /// * Keeping "core" modifications to what are true improvements of core code rather than customizations will
    ///   improve the ability for a derived game to continue taking merges from the main WheelMUD repositories,
    ///   and improves the ability for the community to contribute back to everyone else through pull requests,
    ///   since developers generally won't have to "sanitize" a bunch of game-specific changes to do so.
    /// * Developers should have an easier time sharing individual parts and systems in interesting ways, such as
    ///   publishing NuGet packages with specific extension functionality.
    /// * Ability to "recompose" on the fly mainly supports non-reboot code updates, meaning less down time and
    ///   interruptions of your gamers for code upgrades. For example, if you've fixed a bug in a GameAction and
    ///   locally tested it, you could drop the tested DLL into your running MUD folder, and run "update-actions"
    ///   in the ServerHarness command prompt. (Eventually we may implement file watchers and automatic updates.)
    /// 
    /// To best support these goals, export attributes can implement IExportWithPriority and use DefaultComposer
    /// functions to compose those parts and systems. DefaultComposer will select one instance from each class
    /// name with the highest, non-zero "Priority" value. Core parts tend to export with priority 0, while any
    /// sample game system parts (such as the default WarriorRogueMage sample implementation) use priority 100.
    /// You should pick a value like 200 for any parts you want to replace. For example, if you decided to fully
    /// replace how the "tell" action worked in your specific game, you could create a new class in your own
    /// game's library called "Tell" that inherits from GameAction with [ExportGameAction(200)]. Your version
    /// would always get loaded instead of the core version.
    /// 
    /// In case of ties to "Priority", DefaultComposer will inspect the FileInfo where the classes came from, to
    /// pick the version that was latest modified. This tie-breaker is mainly to support the non-reboot-update
    /// recomposition path: If you keep making code modifications, testing them, and deploying them to your live
    /// server as additional temporary DLL files copied in, then DefaultComposer will keep selecting the one you
    /// newest ones. (You will likely still want a proper deployment process including reboots occasionally, but
    /// having the option to live-patch minor safe fixes can reduce many player inconveniences.)
    /// 
    /// If you want to disable a part or system, you can set a negative Priority to have it ignored. Note that
    /// you will not want to pair this with non-reboot updates as any existing DLL found with a higher Priority
    /// will win load preference and bring back that older version. Depending on the part/system, you may find
    /// it better to disable pre-existing versions by exporting your own gutted version with a higher Priority.
    /// For example, exporting a new "Tell" class with [ExportGameAction(200)] but with no action aliases would
    /// make it so nobody in the game could use the "tell" command any more. These may be useful strategies for
    /// emergency disabling of problematic parts, until the problem can be properly solved and reintroduced.
    /// </remarks>
    public static class DefaultComposer
    {
        /// <summary>Initializes static members of the DefaultComposer class.</summary>
        static DefaultComposer()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string rootPath = Path.GetDirectoryName(assembly.Location);
            var asmCatalog = new AssemblyCatalog(assembly);
            var dirCatalog = new DirectoryCatalog(rootPath);
            var aggregateCatalog = new AggregateCatalog(asmCatalog, dirCatalog);
            Container = new CompositionContainer(aggregateCatalog);
        }

        /// <summary>Gets or sets the default composition container.</summary>
        /// <remarks>This should only be set by the static constructor, or changed by tests, in order to mock composition results.</remarks>
        public static CompositionContainer Container { get; set; }

        /// <summary>
        /// Get just the latest distinct list of type instances from the set of freshly-imported types.
        /// "Latest" is defined as the instance of a given name whose type is defined in the
        /// most-recently-modified assembly.  For use immediately after composition and recomposition.
        /// </summary>
        /// <typeparam name="T">The imported type.</typeparam>
        /// <param name="importedTypes">The set of imported type instances.</param>
        /// <returns>The "latest" distinct list of type instances.</returns>
        /// <remarks>TODO: DEPRECATED - Try to replace with Priority versions only.</remarks>
        public static List<T> GetNonPrioritizedInstances<T>(IEnumerable<T> importedTypes)
        {
            var distinctInstances = new List<T>();
            var distinctTypeNames = importedTypes.Select(t => t.GetType().Name).Distinct();
            foreach (var distinctTypeName in distinctTypeNames)
            {
                var latestMatching = (from g in importedTypes
                                      where g.GetType().Name == distinctTypeName
                                      orderby new FileInfo(g.GetType().Module.FullyQualifiedName).LastWriteTime descending
                                      select g).FirstOrDefault();
                distinctInstances.Add(latestMatching);
            }

            return distinctInstances;
        }

        public static ConstructorInfo GetConstructor<T, Meta>(IEnumerable<Lazy<T, Meta>> importedTypesWithMetadata, Type[] constructorTypes)
            where Meta : IExportWithPriority
        {
            return GetType(importedTypesWithMetadata).GetConstructor(constructorTypes);
        }

        /// <summary>Get just the latest version of the highest-priority instances of an exported type.</summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <typeparam name="Meta">The export metadata type.</typeparam>
        /// <param name="importedTypesWithMetadata">The imported types, with metadata, to search for the latest priority version.</param>
        /// <returns>The latest priority instance.</returns>
        public static T GetInstance<T, Meta>(IEnumerable<Lazy<T, Meta>> importedTypesWithMetadata)
            where Meta : IExportWithPriority
        {
            return (from exportData in importedTypesWithMetadata
                    where exportData.Metadata.Priority >= 0
                    orderby exportData.Metadata.Priority descending,
                            new FileInfo(exportData.Value.GetType().Module.FullyQualifiedName).LastWriteTime descending
                    select exportData.Value).FirstOrDefault();
        }

        public static Type GetType<T, Meta>(IEnumerable<Lazy<T, Meta>> importedTypesWithMetadata)
            where Meta : IExportWithPriority
        {
            return GetInstance(importedTypesWithMetadata).GetType();
        }

        public static List<ConstructorInfo> GetConstructors<T, Meta>(IEnumerable<Lazy<T, Meta>> importedTypesWithMetadata, Type[] constructorTypes)
            where Meta : IExportWithPriority
        {
            var distinctTypeNames = importedTypesWithMetadata.Select(t => t.Value.GetType().Name).Distinct();
            return (from typeName in distinctTypeNames
                    let importsWithThatName = importedTypesWithMetadata.Where(t => t.Value.GetType().Name == typeName)
                    select GetConstructor(importsWithThatName, constructorTypes)).ToList();
        }

        public static List<T> GetInstances<T, Meta>(IEnumerable<Lazy<T, Meta>> importedTypesWithMetadata)
            where Meta : IExportWithPriority
        {
            var distinctTypeNames = importedTypesWithMetadata.Select(t => t.Value.GetType().Name).Distinct();
            return (from typeName in distinctTypeNames
                    let importsWithThatName = importedTypesWithMetadata.Where(t => t.Value.GetType().Name == typeName)
                    select GetInstance(importsWithThatName)).ToList();
        }

        public static List<Type> GetTypes<T, Meta>(IEnumerable<Lazy<T, Meta>> importedTypesWithMetadata)
            where Meta : IExportWithPriority
        {
            var distinctTypeNames = importedTypesWithMetadata.Select(t => t.Value.GetType().Name).Distinct();
            return (from typeName in distinctTypeNames
                    let importsWithThatName = importedTypesWithMetadata.Where(t => t.Value.GetType().Name == typeName)
                    select GetType(importsWithThatName)).ToList();
        }
    }
}