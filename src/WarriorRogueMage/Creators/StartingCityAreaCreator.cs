//-----------------------------------------------------------------------------
// <copyright file="StartingCityAreaCreator.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Actions;
using WheelMUD.Core;

namespace WarriorRogueMage.Creators
{
    [CreatorExports.Area(0)]
    public class StartingCityAreaCreator : CreatorDefinitions.Area
    {
        public override string ID => "areas/wrmStartingCity";

        public override Thing Create(/*Action<Thing> saveAreaProgress*/)
        {
            var area = new Thing(new AreaBehavior())
            {
                Id = ID,
                Name = "Starting City"
            };

            // Add more WRM thematic flavor here...
            var roomCitySquare = new Thing(new RoomBehavior())
            {
                Id = "rooms|",
                Name = "City Square",
                Description = "You stand amidst a bustling medieval city..."
            };
            area.Add(roomCitySquare);

            var roomMainRoadE = new Thing(new RoomBehavior())
            {
                Id = "rooms|",
                Name = "Main Street E",
                Description = "You are just east of the main city square...",
            };
            area.Add(roomMainRoadE);

            var roomMainRoadW = new Thing(new RoomBehavior())
            {
                Id = "rooms|",
                Name = "Main Street W",
                Description = "You are just west of the main city square...",
            };
            area.Add(roomMainRoadW);

            // The area, including the rooms, needs to be persisted before exits are added, so that the exit handlers
            // can simply work with only room IDs instead of 
            Tunnel.TwoWay(roomCitySquare, "east", roomMainRoadE);
            Tunnel.TwoWay(roomCitySquare, "west", roomMainRoadW);

            return area;
        }
    }
}
