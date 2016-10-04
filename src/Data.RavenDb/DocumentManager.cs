//-----------------------------------------------------------------------------
// <copyright file="DocumentManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : May 29, 2011
//   Purpose   : Central class to host code that will load and save documents 
//               from the document database (RavenDb).
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.RavenDb
{
    using System;
    using System.Linq;

    /// <summary>Central class to host code that will load and save documents from the document database (RavenDb).</summary>
    public class DocumentManager
    {
        /// <summary>Saves the player document.</summary>
        /// <param name="playerDocument">The player document to be saved.</param>
        public static void SavePlayerDocument(PlayerDocument playerDocument)
        {
            using (var ravenSession = DalUtils.GetRavenSession())
            {
                // RavenDB will automatically either insert a new document or update the
                // existing document with this document ID, as appropriate.
                ravenSession.Store(playerDocument);
                ravenSession.SaveChanges();
            }
        }

        /// <summary>Loads the player document.</summary>
        /// <param name="databaseId">The database id.</param>
        /// <returns>The loaded PlayerDocument.</returns>
        public static PlayerDocument LoadPlayerDocument(long databaseId)
        {
            using (var ravenSession = DalUtils.GetRavenSession())
            {
                return (from pd in ravenSession.Query<PlayerDocument>()
                        where pd.DatabaseId == databaseId
                        select pd).FirstOrDefault();
            }
        }

        /// <summary>Loads the player document.</summary>
        /// <param name="playerName">Name of the player.</param>
        /// <returns>The loaded PlayerDocument.</returns>
        public static PlayerDocument LoadPlayerDocument(string playerName)
        {
            using (var ravenSession = DalUtils.GetRavenSession())
            {
                return (from pd in ravenSession.Query<PlayerDocument>()
                        where pd.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase)
                        select pd).FirstOrDefault();
            }
        }
    }
}