//-----------------------------------------------------------------------------
// <copyright file="RoomRepository.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   TODO: Add summary
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using ServiceStack.OrmLite;
    using WheelMUD.Data.Entities;

    /// <summary>Custom code for the RoomRepository class.</summary>
    public partial class RoomRepository
    {
        /// <summary>Loads the exits for a room.</summary>
        /// <param name="roomId">The room id used to load the exits.</param>
        /// <returns>Returns a collection of ExitRecord objects.</returns>
        public ICollection<ExitRecord> LoadExitsForRoom(long roomId)
        {
            using (IDbCommand session = Helpers.OpenSession())
            {
                // Return just "standard" exits, where this room is the primary owner.
                return session.Connection.Select<ExitRecord>("ExitRoomAID = {0}", roomId);
            }
        }
    }
}