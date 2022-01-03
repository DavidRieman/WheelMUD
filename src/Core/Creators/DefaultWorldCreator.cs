//-----------------------------------------------------------------------------
// <copyright file="DefaultWorldCreator.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>The core DefaultWorldCreator generates the default world.</summary>
    /// <remarks>
    /// If your game system needs to do anything specific while initializing the world for the first time only, this
    /// could be a good place to do that. Most MUDs shouldn't need to override this though.
    /// </remarks>
    [CreatorExports.World(0)]
    public class DefaultWorldCreator : CreatorDefinitions.World
    {
        public override Thing Create()
        {
            var world = new Thing(new WorldBehavior())
            {
                Id = "world/root",
                Name = "The World"
            };
            var defaultArea = new Thing(new AreaBehavior())
            {
                Id = "area/foundation",
                Name = "Foundation"
            };
            var firstRoom = new Thing(new RoomBehavior())
            {
                Id = "thing/void",
                Name = "The Void",
                Description = "You are amidst a featureless void."
                // TODO: Description should add advice for administrators as a mini-tutorial for getting started, e.g.
                // * How to identify other areas that were loaded/created.
                // * How to find a specific room via name (like a town square) and the room ID.
                // * How to tunnel a one-way exit going from here to said room.
                // * How to adjust the App.config to change the starting room.
            };
            defaultArea.Add(firstRoom);
            world.Add(defaultArea);
            return world;
        }
    }
}
