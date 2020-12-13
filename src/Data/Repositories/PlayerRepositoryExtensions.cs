//-----------------------------------------------------------------------------
// <copyright file="WorldBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using ServiceStack.OrmLite;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using WheelMUD.Data.Entities;

    public static class PlayerRepositoryExtensions
    {
        public static User Authenticate(string userName, string password)
        {
            using (var session = Helpers.OpenDocumentSession())
            {
                var salt = (from u in session.Query<User>()
                            where u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                            select u.Salt).FirstOrDefault();
                if (salt == null)
                {
                    return null;
                }
                var hashedPassword = User.Hash(salt, password);
                return (from u in session.Query<User>()
                        where u.UserName.Equals(userName) &&
                              u.HashedPassword.Equals(hashedPassword)
                        select u).FirstOrDefault();
            }
        }

        public static bool UserNameExists(string userName)
        {
            using (var session = Helpers.OpenDocumentSession())
            {
                return (from u in session.Query<User>()
                        where u.UserName.Equals(userName)
                        select u).Any();
            }
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

            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.Select<RoleRecord>(sql, playerId);
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
            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.Select<PlayerRoleRecord>(string.Format("PlayerID = {0}", playerId));
        }

        /// <summary>Gets a role record that is associated with the role name.</summary>
        /// <param name="roleName">The user name to look up a role.</param>
        /// <returns>Returns a role record loaded with the role's data.</returns>
        public static RoleRecord GetRoleByName(this RelationalRepository<RoleRecord> repository, string roleName)
        {
            RoleRecord roleRecord;

            using (IDbCommand session = Helpers.OpenRelationalSession())
            {
                if ("sqlite".Equals(AppConfigInfo.Instance.RelationalDataProviderName, StringComparison.OrdinalIgnoreCase))
                {
                    var query = "SELECT * FROM Roles WHERE Name = {0} COLLATE NOCASE ";
                    roleRecord = session.Connection.Select<RoleRecord>(query, roleName).First();
                }
                else
                {
                    roleRecord = session.Connection.Select<RoleRecord>(string.Format("Name = {0}", roleName)).First();
                }
            }

            return roleRecord;
        }
    }
}