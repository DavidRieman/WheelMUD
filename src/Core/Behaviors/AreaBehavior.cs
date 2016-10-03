//-----------------------------------------------------------------------------
// <copyright file="AreaBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: August 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using WheelMUD.Data.Entities;
    using WheelMUD.Data.Repositories;

    /// <summary>Behavior which defines a game area.</summary>
    /// <remarks>
    /// @@@ ATM an Area doesn't do anything special. Later it can be used for area-based builder 
    /// permissions,maybe some respawn/instancing rules specific to the area, etc.
    /// </remarks>
    public class AreaBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the AreaBehavior class.</summary>
        public AreaBehavior()
            : base(null)
        {
        }

        /// <summary>Loads this instance.</summary>
        public void Load()
        {
            var areaRepository = new AreaRepository();

            // @@@ TODO: Fix hack: http://www.wheelmud.net/tabid/59/aft/1622/Default.aspx
            string areaNumber = this.Parent.ID.Replace("area/", string.Empty);
            long persistedAreaID = long.Parse(areaNumber);
            ICollection<RoomRecord> rooms = areaRepository.GetRoomsForArea(persistedAreaID);

            foreach (var roomRecord in rooms)
            {
                // @@@ TODO: Fix hack: http://www.wheelmud.net/tabid/59/aft/1622/Default.aspx
                var roomBehavior = new RoomBehavior()
                {
                    ID = roomRecord.ID,
                };
                var currRoom = new Thing(roomBehavior)
                {
                    Name = roomRecord.Name,
                    Description = roomRecord.Description,
                    ID = "room/" + roomRecord.ID,
                };

                // Load this room and it's children.
                roomBehavior.Load();
                this.Parent.Add(currRoom);
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}