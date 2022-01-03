//-----------------------------------------------------------------------------
// <copyright file="WorldBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using ServiceStack.OrmLite;
    using System.Collections.Generic;
    using System.Data;
    using WheelMUD.Data.Entities;

    public static class GeneralRepositoryExtensions
    {
        /// <summary>Loads the alias entries for a given help topic.</summary>
        /// <param name="helpTopicId">The ID of the parent help topic</param>
        /// <returns>List of Help Topic Alias records</returns>
        public static List<HelpTopicAliasRecord> LoadAliasForTopic(this RelationalRepository<HelpTopicRecord> repository, long helpTopicId)
        {
            string sql = @"SELECT * 
                           FROM HelpTopicAliases 
                           WHERE HelpTopicID = {0}";

            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.Select<HelpTopicAliasRecord>(sql, helpTopicId);
        }

        // TODO: Generally we should create static spawner behaviors that exist in the world, which in turn spawn mobs,
        //       since we probably shouldn't try to persist mob instances in the world. (It would get barren quickly.)
        /// <summary>Gets a list of mobiles that should exist for the specified room.</summary>
        /// <param name="roomId">The ID of the room.</param>
        /// <returns>A list of mobile data structures for mobiles that start in this room.</returns>
        public static ICollection<MobRecord> GetMobsForRoom(this RelationalRepository<MobRecord> repository, long roomId)
        {
            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.Select<MobRecord>(string.Format("CurrentRoomID = {0}", roomId));
        }
    }
}