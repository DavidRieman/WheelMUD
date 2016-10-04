//-----------------------------------------------------------------------------
// <copyright file="PlayerRoleRepository.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 7/17/2009 10:16:25 PM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using ServiceStack.OrmLite;
    using WheelMUD.Data.Entities;

    /// <summary>Custom code for the PlayerRoleRepository class.</summary>
    public partial class PlayerRoleRepository
    {
        /// <summary>Deletes a list of roles from a player.</summary>
        /// <param name="playerId">The id of the player that will have roles removed.</param>
        public void DeleteAllRolesForPlayer(long playerId)
        {
            ICollection<PlayerRoleRecord> playerRoleRecords = this.FetchAllPlayerRoleRecordsForPlayer(playerId);

            foreach (var playerRoleRecord in playerRoleRecords)
            {
                this.Remove(playerRoleRecord);
            }
        }

        /// <summary>Adds a list of roles to a player.</summary>
        /// <param name="roles">The roles that will be added to a player.</param>
        public void AddRolesToPlayer(List<PlayerRoleRecord> roles)
        {
            foreach (var role in roles)
            {
                if (role.ID == 0)
                {
                    this.Add(role);
                }
                else
                {
                    this.Update(role);
                }
            }
        }

        /// <summary>Fetches all player role records for player.</summary>
        /// <param name="playerId">The player id that will be used to retrieve the roles.</param>
        /// <returns>A list of PlayerRoleRecord objects.</returns>
        public ICollection<PlayerRoleRecord> FetchAllPlayerRoleRecordsForPlayer(long playerId)
        {
            long id = playerId;

            using (IDbCommand session = Helpers.OpenSession())
            {
                return session.Connection.Select<PlayerRoleRecord>("PlayerID = {0}", id);
            }
        }
    }
}