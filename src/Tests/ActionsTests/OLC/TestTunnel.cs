//-----------------------------------------------------------------------------
// <copyright file="TestTunnel.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using WheelMUD.Core;

namespace WheelMUD.Actions.Tests
{
    [TestClass]
    public class TestTunnel
    {
        private Thing room1, room2;

        [TestInitialize]
        public void Init()
        {
            room1 = new Thing(new RoomBehavior()) { Id = "r1", Persists = false };
            room2 = new Thing(new RoomBehavior()) { Id = "r2", Persists = false };
        }

        [TestMethod]
        public void StaticTunnelCreatesTwoWayContextCommands()
        {
            Tunnel.TwoWay(room1, "east", room2);

            // Ensure both rooms have the Exit thing.
            var exitThing = room1.Children.First();
            Assert.IsTrue(room2.Children.First() == exitThing);
            var exitBehavior = exitThing.FindBehavior<ExitBehavior>();
            Assert.IsNotNull(exitBehavior);

            // Ensure both rooms have only the appropriate context command rigged up.
            var commandKeys1 = (from c in room1.Commands orderby c.Key select c.Key).ToArray();
            var commandKeys2 = (from c in room2.Commands orderby c.Key select c.Key).ToArray();
            Assert.IsTrue(commandKeys1.SequenceEqual(new string[] { "e", "east" }));
            Assert.IsTrue(commandKeys2.SequenceEqual(new string[] { "w", "west" }));
        }
    }
}
