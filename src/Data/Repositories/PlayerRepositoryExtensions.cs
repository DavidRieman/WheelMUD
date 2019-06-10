//-----------------------------------------------------------------------------
// <copyright file="WorldBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using ServiceStack.OrmLite;
    using WheelMUD.Data.Entities;

    public static class PlayerRepositoryExtensions
    {
        /// <summary>Authenticates the specified name.</summary>
        /// <param name="userName">The user name for the player trying to authenticate.</param>
        /// <param name="password">The password for the player trying to authenticate.</param>
        /// <returns>A Boolean value indicating whether the authentication request was successful or not.</returns>
        public static bool Authenticate(string userName, string password)
        {
            PlayerRecord player;

            using (IDbCommand session = Helpers.OpenRelationalSession())
            {
                if ("sqlite".Equals(HelperConfigInfo.Instance.RelationalDataProviderName, StringComparison.OrdinalIgnoreCase))
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
        public static PlayerRecord GetPlayerByUserName(this RelationalRepository<PlayerRecord> repository, string userName)
        {
            PlayerRecord player;

            using (IDbCommand session = Helpers.OpenRelationalSession())
            {
                if ("sqlite".Equals(HelperConfigInfo.Instance.RelationalDataProviderName, StringComparison.OrdinalIgnoreCase))
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
        public static List<RoleRecord> GetPlayerRoles(this RelationalRepository<PlayerRecord> repository, long playerId)
        {
            const string sql = @"SELECT DISTINCT pr.RoleID As ID, r.Name, 
                                    r.SecurityRoleMask 
                                 FROM PlayerRoles pr
                                 INNER JOIN Roles r 
                                 ON pr.RoleID = r.ID 
                                 WHERE pr.PlayerID = {0}";

            using (IDbCommand session = Helpers.OpenRelationalSession())
            {
                return session.Connection.Select<RoleRecord>(sql, playerId);
            }
        }


        /// <summary>Deletes a list of roles from a player.</summary>
        /// <param name="playerId">The id of the player that will have roles removed.</param>
        public static void DeleteAllRolesForPlayer(this RelationalRepository<PlayerRoleRecord> repository, long playerId)
        {
            ICollection<PlayerRoleRecord> playerRoleRecords = repository.FetchAllPlayerRoleRecordsForPlayer(playerId);
            foreach (var playerRoleRecord in playerRoleRecords)
            {
                repository.Remove(playerRoleRecord);
            }
        }

        /// <summary>Adds a list of roles to a player.</summary>
        /// <param name="roles">The roles that will be added to a player.</param>
        public static void AddRolesToPlayer(this RelationalRepository<PlayerRoleRecord> repository, List<PlayerRoleRecord> roles)
        {
            foreach (var role in roles)
            {
                if (role.ID == 0)
                {
                    repository.Add(role);
                }
                else
                {
                    repository.Update(role);
                }
            }
        }

        /// <summary>Fetches all player role records for player.</summary>
        /// <param name="playerId">The player id that will be used to retrieve the roles.</param>
        /// <returns>A list of PlayerRoleRecord objects.</returns>
        public static ICollection<PlayerRoleRecord> FetchAllPlayerRoleRecordsForPlayer(this RelationalRepository<PlayerRoleRecord> repository, long playerId)
        {
            long id = playerId;

            using (IDbCommand session = Helpers.OpenRelationalSession())
            {
                return session.Connection.Select<PlayerRoleRecord>("PlayerID = {0}", id);
            }
        }


        /// <summary>Gets a role record that is associated with the role name.</summary>
        /// <param name="roleName">The user name to look up a role.</param>
        /// <returns>Returns a role record loaded with the role's data.</returns>
        public static RoleRecord GetRoleByName(this RelationalRepository<RoleRecord> repository, string roleName)
        {
            RoleRecord roleRecord;

            using (IDbCommand session = Helpers.OpenRelationalSession())
            {
                if ("sqlite".Equals(HelperConfigInfo.Instance.RelationalDataProviderName, StringComparison.OrdinalIgnoreCase))
                {
                    var sql = new StringBuilder();

                    sql.Append("SELECT * FROM Roles ");
                    sql.Append("WHERE Name = {0} ");
                    sql.Append(" COLLATE NOCASE ");

                    roleRecord = session.Connection.Select<RoleRecord>(sql.ToString(), roleName).First();
                }
                else
                {
                    roleRecord = session.Connection.Select<RoleRecord>("Name = {0}", roleName).First();
                }
            }

            return roleRecord;
        }
    }
}