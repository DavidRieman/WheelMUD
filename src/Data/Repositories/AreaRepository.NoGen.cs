//-----------------------------------------------------------------------------
// <copyright file="AreaRepository.NoGen.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 7/2/2009 9:44:29 PM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using ServiceStack.DataAnnotations;
    using ServiceStack.OrmLite;
    using WheelMUD.Data.Entities;

    [Alias("Areas")]
    public partial class AreaRepository
    {
        /// <summary>Gets the rooms for area.</summary>
        /// <param name="areaId">The id for the area that will be worked upon.</param>
        /// <returns>Returns an collection of RoomRecord objects.</returns>
        public ICollection<RoomRecord> GetRoomsForArea(long areaId)
        {
            long id = areaId;

            using (IDbCommand session = Helpers.OpenSession())
            {
                return session.Connection.Select<RoomRecord>("AreaID = {0}", id);
            }
        }
    }
}