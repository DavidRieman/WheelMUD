//-----------------------------------------------------------------------------
// <copyright file="DefaultComposer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Karak
//   Date      : June 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Reflection;

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
        /// "Latest" is defined as the instance of a given full name whose type is defined in the 
        /// most-recently-modified assembly.  For use immediately after composition and recomposition.
        /// </summary>
        /// <typeparam name="T">The imported type.</typeparam>
        /// <param name="importedTypes">The set of imported type instances.</param>
        /// <returns>The "latest" distinct list of type instances.</returns>
        public static List<T> GetLatestDistinctTypeInstances<T>(IEnumerable<T> importedTypes)
        {
            var distinctInstances = new List<T>();
            var distinctTypeNames = (from g in importedTypes
                                     select g.GetType().FullName).Distinct();
            foreach (var distinctTypeName in distinctTypeNames)
            {
                var latestMatching = (from g in importedTypes
                                      where g.GetType().FullName == distinctTypeName
                                      orderby new FileInfo(g.GetType().Module.FullyQualifiedName).LastWriteTime descending
                                      select g).FirstOrDefault();
                distinctInstances.Add(latestMatching);
            }

            return distinctInstances;
        }
    }
}