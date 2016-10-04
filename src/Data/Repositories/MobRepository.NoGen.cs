//-----------------------------------------------------------------------------
// <copyright file="MobRepository.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 7/5/2009 9:52:27 PM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System;
    using System.Collections.Generic;  
    using System.Data;
    using ServiceStack.OrmLite;
    using WheelMUD.Data.Entities;

    /// <summary>Custom code for the MobRepository class.</summary>
    public partial class MobRepository
    {
        /// <summary>Gets a list of mobiles that should exist for the specified room.</summary>
        /// <param name="roomId">The ID of the room.</param>
        /// <returns>A list of mobile data structures for mobiles that start in this room.</returns>
        public ICollection<MobRecord> GetMobsForRoom(long roomId)
        {
            int id = Convert.ToInt32(roomId);

            using (IDbCommand session = Helpers.OpenSession())
            {
                return session.Connection.Select<MobRecord>("CurrentRoomID = {0}", id);
            }
        }
    }
}
