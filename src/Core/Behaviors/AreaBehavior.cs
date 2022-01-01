//-----------------------------------------------------------------------------
// <copyright file="AreaBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Data.Entities;
using WheelMUD.Data.Repositories;

namespace WheelMUD.Core
{
    /// <summary>Behavior which defines a game area.</summary>
    /// <remarks>
    /// ATM an Area doesn't do anything special.
    /// TODO: This could get used as the base level "document" loaded for a section of the game world, to give
    ///       the ability to still load most of the world when some sub-section fails, etc.
    /// TODO: This could apply area-based builder permissions (defining who can modify the descendants), perhaps
    ///       with eventing that would cancel OLC modification requests that aren't from a permitted builder.
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
            var areaRepository = new RelationalRepository<AreaRecord>();
            string areaNumber = Parent.Id.Replace("area/", string.Empty);
            long persistedAreaID = long.Parse(areaNumber);
            ICollection<RoomRecord> rooms = areaRepository.GetRoomsForArea(persistedAreaID);

            foreach (var roomRecord in rooms)
            {
                var roomBehavior = new RoomBehavior()
                {
                    ID = roomRecord.ID,
                };
                var currRoom = new Thing(roomBehavior)
                {
                    Name = roomRecord.Name,
                    Description = roomRecord.Description,
                    Id = "room/" + roomRecord.ID,
                };

                // Load this room and it's children.
                roomBehavior.Load();
                Parent.Add(currRoom);
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}