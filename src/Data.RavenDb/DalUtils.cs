//-----------------------------------------------------------------------------
// <copyright file="DalUtils.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Utilities for making the use of RavenDb easier.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.RavenDb
{
    using Raven.Client.Documents.Session;
    using System;
    using System.IO;
    using WheelMUD.Utilities;

    /// <summary>Utilities for making the use of RavenDb easier.</summary>
    public class DalUtils
    {
        /// <summary>Gets the path to where the RavenDb data will be stored.</summary>
        /// <returns>The path to the folder where the RavenDb data is stored.</returns>
        public static string GetDbPath()
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var fullPath = Path.Combine(root, "WheelMUD", GameConfiguration.Name, "DocumentDatabase");

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                CreateIndexes();
            }

            return fullPath;
        }

        /// <summary>Gets the raven session.</summary>
        /// <returns>Returns a Raven.Client.IDocumentSession</returns>
        public static IDocumentSession GetRavenSession()
        {
            var store = DocumentStoreHolder.Instance;
            var session = store.OpenSession();
            return session;
        }

        /// <summary>Creates the needed indexes, if they don't exist.</summary>
        public static void CreateIndexes()
        {
            /* TODO: REPAIR INDEXES https://demo.ravendb.net/demos/related-documents/index-related-documents
             * using (var store = DocumentStore.Instance)
            {
                store.DatabaseCommands.PutIndex(
                    "GetPlayerByDatabaseId",
                    new IndexDefinitionBuilder<PlayerDocument>
                    {
                        Map = docs => from doc in docs
                                      select new
                                      {
                                          doc.DatabaseId
                                      }
                    });

                store.DatabaseCommands.PutIndex(
                    "GetPlayerByName",
                    new IndexDefinitionBuilder<PlayerDocument>
                    {
                        Map = docs => from doc in docs
                                      select new
                                      {
                                          doc.Name
                                      }
                    });
            }*/
        }
    }
}