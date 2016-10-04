//-----------------------------------------------------------------------------
// <copyright file="PlayerRepository.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 7/6/2009 8:53:34 PM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using ServiceStack.OrmLite;
    using WheelMUD.Data.Entities;

    /// <summary>Custom code for the PlayerRepository class.</summary>
    public partial class PlayerRepository
    {
        /// <summary>Authenticates the specified name.</summary>
        /// <param name="userName">The user name for the player trying to authenticate.</param>
        /// <param name="password">The password for the player trying to authenticate.</param>
        /// <returns>A Boolean value indicating whether the authentication request was successful or not.</returns>
        public static bool Authenticate(string userName, string password)
        {
            PlayerRecord player;

            using (IDbCommand session = Helpers.OpenSession())
            {
                if (Helpers.GetCurrentProviderName().ToLower() == "system.data.sqlite")
                {
                    const string sql = @"SELECT * FROM Players 
                                         WHERE UserName = {0} 
                                         COLLATE NOCASE 
                                         AND Password = {1}";

                    player = session.Connection.Select<PlayerRecord>(sql, userName, password).FirstOrDefault();
                }
                else
                {
                    player = session.Connection.Select<PlayerRecord>("UserName = {0} and Password = {1}", userName, password).FirstOrDefault();
                }
            }

            // If we found a player that matched that name/password pair, then authentication is successful.
            return player != null;
        }

        /// <summary>Gets a player record that is associated with the user name.</summary>
        /// <param name="userName">The user name to look up a player.</param>
        /// <returns>Returns a player record loaded with the player's data.</returns>
        public PlayerRecord GetPlayerByUserName(string userName)
        {
            PlayerRecord player;

            using (IDbCommand session = Helpers.OpenSession())
            {
                if (Helpers.GetCurrentProviderName().ToLower() == "system.data.sqlite")
                {
                    const string sql = @"SELECT * FROM Players 
                                         WHERE UserName = {0} 
                                         COLLATE NOCASE ";

                    player = session.Connection.Select<PlayerRecord>(sql, userName).FirstOrDefault();
                }
                else
                {
                    player = session.Connection.QuerySingle<PlayerRecord>("UserName = {0}", userName);
                }
            }

            return player;
        }

        /// <summary>Gets a list of RoleRecords for a specific player</summary>
        /// <param name="playerId">The player Id for which we want roles loaded.</param>
        /// <returns>Returns a list of RoleRecords, if any, for the specified player Id.</returns>
        public List<RoleRecord> GetPlayerRoles(long playerId)
        {
            const string sql = @"SELECT DISTINCT pr.RoleID As ID, r.Name, 
                                    r.SecurityRoleMask 
                                 FROM PlayerRoles pr
                                 INNER JOIN Roles r 
                                 ON pr.RoleID = r.ID 
                                 WHERE pr.PlayerID = {0}";

            using (IDbCommand session = Helpers.OpenSession())
            {
                return session.Connection.Select<RoleRecord>(sql, playerId);
            }
        }
    }
}