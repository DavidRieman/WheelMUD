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
                // The only non-plural ID, as there should only ever be one.
                // TODO: Refer to via a shared Core static rather than multiple "world/root" strings across the code...
                // If you want thematic "worlds" that have areas, you might probably actually want to represent them as
                // areas of areas (Thing with AreaBehavior that has Children Things with AreaBehavior) as WorldBehavior
                // is meant to be the single point we start the game world loading process.
                Id = "world/root",
                Name = "The World"
            };
            var defaultArea = new Thing(new AreaBehavior())
            {
                Id = "areas/foundation",
                Name = "Foundation"
            };
            var firstRoom = new Thing(new RoomBehavior())
            {
                Id = "rooms/void",
                Name = "The Void",
                Description = "You are amidst a featureless void. Players whose saved rooms cannot be found will end " +
                    "up loading here, assuming it is configured in App.config as the default room. For now, WheelMUD " +
                    "administrators logging in for the first time should familiarize with these first commands: " +
                    "'find city' will list rooms with that keyword, and then take the ID printed for City Square to " +
                    "'tunnel d rooms/1' (for example) to try out your first OLC command."
                // TODO: Description should add advice for administrators as a mini-tutorial for getting started, e.g.
                // * How to identify other areas that were loaded/created.
                // * Improve the appearance of the output.
                // * Helpful help commands.
            };
            defaultArea.Add(firstRoom);
            world.Add(defaultArea);
            return world;
        }
    }
}
