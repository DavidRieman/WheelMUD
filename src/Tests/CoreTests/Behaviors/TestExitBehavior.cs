//-----------------------------------------------------------------------------
// <copyright file="TestExitBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WheelMUD.Core;

namespace WheelMUD.Tests.Behaviors
{
    /// <summary>Tests for the ExitBehavior class.</summary>
    [TestClass]
    public class TestExitBehavior
    {
        /// <summary>The actors in the test.</summary>
        private Thing roomA, roomB, exit, actor;

        /// <summary>The behavior being tested.</summary>
        private ExitBehavior exitBehavior;

        /// <summary>Common preparation for all ExitBehavior tests.</summary>
        [TestInitialize]
        public void Init()
        {
            // Create 2 rooms and a basic ExitBehavior in prep for testing.
            roomA = new Thing(new RoomBehavior()) { Name = "Room A", Id = TestThingID.Generate("testroom") };
            roomB = new Thing(new RoomBehavior()) { Name = "Room B", Id = TestThingID.Generate("testroom") };
            exitBehavior = new ExitBehavior();
            exit = new Thing(exitBehavior) { Name = "Exit", Id = TestThingID.Generate("testexit") };
        }

        /// <summary>Test behaviors of a one-way exit.</summary>
        [TestMethod]
        public void TestOneWayExitBehavior()
        {
            // Put the exit in room A only, and register an exit command to travel one way.
            roomA.Add(exit);
            exitBehavior.AddDestination("east", roomB);

            // Ensure the exit is rigged up to the correct location now, but does not work the other way around.
            Assert.AreSame(exitBehavior.GetDestination(roomA), roomB);
            Assert.AreSame(exitBehavior.GetDestination(roomA), roomB);
            Assert.AreNotSame(exitBehavior.GetDestination(roomB), roomA);

            // Create an unmovable actor, and ensure that said actor cannot move through.
            actor = new Thing() { Name = "Actor" };
            roomA.Add(actor);
            exitBehavior.MoveThrough(actor);
            Assert.AreSame(actor.Parent, roomA);

            // Make the actor movable, and try moving the actor through again.
            actor.Behaviors.Add(new MovableBehavior());
            exitBehavior.MoveThrough(actor);
            Assert.AreSame(actor.Parent, roomB);

            // Ensure the actor does not end up in room A if we try to shove the actor through again.
            exitBehavior.MoveThrough(actor);
            Assert.AreSame(actor.Parent, roomB);

            // TODO: Place the actor back in room A, and try using the context command to move it?
            roomA.Add(actor);
        }

        /// <summary>Test behaviors of a two-way exit.</summary>
        [TestMethod]
        public void TestTwoWayExitBehavior()
        {
            // Allow the exit to reside in two places, and place it in both room A and room B.
            exit.Behaviors.Add(new MultipleParentsBehavior());
            roomA.Add(exit);
            roomB.Add(exit);

            // Ensure there are no destinations before rigging them up.
            Assert.AreSame(exitBehavior.GetDestination(roomA), null);
            Assert.AreSame(exitBehavior.GetDestination(roomB), null);

            // Rig the exits and ensure both got rigged up to the correct destinations.
            exitBehavior.AddDestination("north", roomB);
            exitBehavior.AddDestination("south", roomA);
            Assert.AreSame(exitBehavior.GetDestination(roomA), roomB);
            Assert.AreSame(exitBehavior.GetDestination(roomB), roomA);

            // Create an unmovable actor, and ensure that said actor cannot move through.
            actor = new Thing() { Name = "Actor", Id = TestThingID.Generate("testactor") };
            roomA.Add(actor);
            exitBehavior.MoveThrough(actor);
            Assert.AreSame(actor.Parent, roomA);

            // Make the actor movable, and ensure they end up in the next room when moving through.
            actor.Behaviors.Add(new MovableBehavior());
            exitBehavior.MoveThrough(actor);
            Assert.AreSame(actor.Parent, roomB);

            // Move the actor back through the exit again, and ensure they end up in the starting room.
            exitBehavior.MoveThrough(actor);
            Assert.AreSame(actor.Parent, roomA);

            // TODO: Ensure the actor does not move through when using the wrong context command?
            // TODO: Ensure the actor moves through both ways when using correct context commands?
        }
    }
}