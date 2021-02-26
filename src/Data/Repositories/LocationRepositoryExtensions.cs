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

    public static class LocationRepositoryExtensions
    {
        /// <summary>Gets the rooms for area.</summary>
        /// <param name="repository">The Area repository.</param>
        /// <param name="areaId">The id for the area that will be worked upon.</param>
        /// <returns>Returns an collection of RoomRecord objects.</returns>
        public static ICollection<RoomRecord> GetRoomsForArea(this RelationalRepository<AreaRecord> repository, long areaId)
        {
            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.Select<RoomRecord>($"AreaID = {areaId}");
        }

        /// <summary>Loads the exits for a room.</summary>
        /// <param name="roomId">The room id used to load the exits.</param>
        /// <returns>Returns a collection of ExitRecord objects.</returns>
        public static ICollection<ExitRecord> LoadExitsForRoom(this RelationalRepository<RoomRecord> repository, long roomId)
        {
            // Return just "standard" exits, where this room is the primary owner.
            using IDbCommand session = Helpers.OpenRelationalSession();
            return session.Connection.Select<ExitRecord>($"ExitRoomAID = {roomId}");
        }
    }
}