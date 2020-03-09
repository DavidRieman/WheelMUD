//-----------------------------------------------------------------------------
// <copyright file="DefaultComposer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using WheelMUD.Interfaces;

    /// <summary>A class for simplifying common composition tasks.</summary>
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
        public static List<T> GetLatestDistinctTypeInstances<T>(IEnumerable<T> importedTypes)
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

        /// <summary>Get just the latest version of the highest-priority instances of an exported type.</summary>
        /// <typeparam name="T">The instance type.</typeparam>
        /// <typeparam name="Meta">The export metadata type.</typeparam>
        /// <param name="importedTypesWithMetadata">The imported types, with metadata, to search for the latest priority version.</param>
        /// <returns>The latest priority instance.</returns>
        public static T GetLatestPriorityTypeInstance<T, Meta>(IEnumerable<Lazy<T, Meta>> importedTypesWithMetadata)
            where Meta: IExportWithPriority
        {
            return (from exportData in importedTypesWithMetadata
                    where exportData.Metadata.Priority >= 0
                    orderby exportData.Metadata.Priority descending,
                            new FileInfo(exportData.Value.GetType().Module.FullyQualifiedName).LastWriteTime descending
                    select exportData.Value).FirstOrDefault();
        }

        public static List<Type> GetLatestDistinctPriorityTypes<T, Meta>(IEnumerable<Lazy<T, Meta>> importedTypesWithMetadata)
            where Meta : IExportWithPriority
        {
            var distinctTypeNames = importedTypesWithMetadata.Select(t => t.Value.GetType().Name).Distinct();
            return (from typeName in distinctTypeNames
                    select GetLatestPriorityTypeInstance<T, Meta>(importedTypesWithMetadata.Where(t => t.Value.GetType().Name == typeName)).GetType()).ToList();
        }

        public static ConstructorInfo GetLatestPriorityTypeConstructor<T, Meta>(IEnumerable<Lazy<T, Meta>> importedTypesWithMetadata, Type[] constructorTypes)
            where Meta : IExportWithPriority
        {
            return GetLatestPriorityTypeInstance(importedTypesWithMetadata).GetType().GetConstructor(constructorTypes);
        }
    }
}