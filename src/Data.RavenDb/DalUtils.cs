//-----------------------------------------------------------------------------
// <copyright file="DalUtils.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Utilities for making the use of RavenDb easier.
//   Date: May 15, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.RavenDb
{
    using System;
    using System.IO;
    using System.Linq;
    using Raven.Client;
    using Raven.Client.Indexes;
    using WheelMUD.Utilities;

    /// <summary>Utilities for making the use of RavenDb easier.</summary>
    public class DalUtils
    {
        /// <summary>Gets the path to where the RavenDb data will be stored.</summary>
        /// <returns>The path to the folder where the RavenDb data is stored.</returns>
        public static string GetDbPath()
        {
            string root = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string mudName = MudEngineAttributes.Instance.MudName;

            root = Path.Combine(root, "WheelMUD");
            root = Path.Combine(root, mudName);
            root = Path.Combine(root, "DocumentDatabase");

            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
                CreateIndexes();
            }

            return root;
        }

        /// <summary>Gets the raven session.</summary>
        /// <returns>Returns a Raven.Client.IDocumentSession</returns>
        public static IDocumentSession GetRavenSession()
        {
            var store = DocumentStore.Instance;
            var session = store.OpenSession();
            return session;
        }

        /// <summary>Creates the needed indexes, if they don't exist.</summary>
        public static void CreateIndexes()
        {
            using (var store = DocumentStore.Instance)
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
            }
        }
    }
}