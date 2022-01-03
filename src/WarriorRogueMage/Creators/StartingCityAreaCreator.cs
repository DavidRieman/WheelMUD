//-----------------------------------------------------------------------------
// <copyright file="StartingCityAreaCreator.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;

namespace WarriorRogueMage.Creators
{
    [CreatorExports.Area(0)]
    public class StartingCityAreaCreator : CreatorDefinitions.Area
    {
        public override string ID => "area/wrmStartingCity";

        public override Thing Create()
        {
            var area = new Thing(new AreaBehavior())
            {
                Id = ID,
                Name = "Starting City"
            };

            // Add more WRM thematic flavor here...
            var roomCitySquare = new Thing(new RoomBehavior())
            {
                Name = "City Square",
                Description = "You stand amidst a bustling medieval city..."
            };
            area.Add(roomCitySquare);

            var roomMainRoadE = new Thing(new RoomBehavior())
            {
                Name = "Main Street E",
                Description = "You are just east of the main city square...",
            };
            area.Add(roomMainRoadE);

            var roomMainRoadW = new Thing(new RoomBehavior())
            {
                Name = "Main Street W",
                Description = "You are just west of the main city square...",
            };
            area.Add(roomMainRoadW);

            var exitMainRoadE = new Thing(new ExitBehavior(), new MultipleParentsBehavior());
            roomCitySquare.Add(exitMainRoadE);
            roomMainRoadE.Add(exitMainRoadE);

            var exitMainRoadW = new Thing(new ExitBehavior(), new MultipleParentsBehavior());
            roomCitySquare.Add(exitMainRoadW);
            roomMainRoadW.Add(exitMainRoadW);

            // @@@ TEMP TEST FOR PERSISTENCE EFFECTS @@@
            //var exitTest = new Thing(new ExitBehavior(), new MultipleParentsBehavior());
            //roomCitySquare.Add(exitTest);
            //ThingManager.Instance.FindThing("area/foundation").Children.First().Add(exitTest);

            return area;
        }
    }
}
