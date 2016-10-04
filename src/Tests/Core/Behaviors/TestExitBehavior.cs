//-----------------------------------------------------------------------------
// <copyright file="TestExitBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: August 2011 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WheelMUD.Core;
    
    /// <summary>Tests for the ExitBehavior class.</summary>
    [TestFixture]
    [TestClass]
    public class TestExitBehavior
    {
        /// <summary>The actors in the test.</summary>
        private Thing roomA, roomB, exit, actor;

        /// <summary>The behavior being tested.</summary>
        private ExitBehavior exitBehavior;

        /// <summary>Common preparation for all ExitBehavior tests.</summary>
        [TestInitialize][SetUp]
        public void Init()
        {
            // Create 2 rooms and a basic ExitBehavior in prep for testing.
            this.roomA = new Thing(new RoomBehavior()) { Name = "Room A", ID = TestThingID.Generate("testroom") };
            this.roomB = new Thing(new RoomBehavior()) { Name = "Room B", ID = TestThingID.Generate("testroom") };
            this.exitBehavior = new ExitBehavior();
            this.exit = new Thing(this.exitBehavior) { Name = "Exit", ID = TestThingID.Generate("testexit") };
        }

        /// <summary>Test behaviors of a one-way exit.</summary>
        [TestMethod][Test]
        public void TestOneWayExitBehavior()
        {
            // Put the exit in room A only, and register an exit command to travel one way.
            this.roomA.Add(this.exit);
            this.exitBehavior.AddDestination("east", this.roomB.ID);

            // Ensure the exit is rigged up to the correct location now, but does not work the other way around.
            Verify.AreSame(this.exitBehavior.GetDestination(this.roomA), this.roomB);
            Verify.AreSame(this.exitBehavior.GetDestination(this.roomA), this.roomB);
            Verify.AreNotSame(this.exitBehavior.GetDestination(this.roomB), this.roomA);
            
            // Create an unmovable actor, and ensure that said actor cannot move through.
            this.actor = new Thing() { Name = "Actor", Parent = this.roomA };
            this.exitBehavior.MoveThrough(this.actor);
            Verify.AreSame(this.actor.Parent, this.roomA);

            // Make the actor movable, and try moving the actor through again.
            this.actor.Behaviors.Add(new MovableBehavior());
            this.exitBehavior.MoveThrough(this.actor);
            Verify.AreSame(this.actor.Parent, this.roomB);

            // Ensure the actor does not end up in room A if we try to shove the actor through again.
            this.exitBehavior.MoveThrough(this.actor);
            Verify.AreSame(this.actor.Parent, this.roomB);

            // @@@ TODO: Place the actor back in room A, and try using the context command to move it?
            this.actor.Parent = this.roomA;
        }

        /// <summary>Test behaviors of a two-way exit.</summary>
        [TestMethod][Test]
        public void TestTwoWayExitBehavior()
        {
            // Allow the exit to reside in two places, and place it in both room A and room B.
            this.exit.Behaviors.Add(new MultipleParentsBehavior());
            this.roomA.Add(this.exit);
            this.roomB.Add(this.exit);

            // Ensure there are no destinations before rigging them up.
            Verify.AreSame(this.exitBehavior.GetDestination(this.roomA), null);
            Verify.AreSame(this.exitBehavior.GetDestination(this.roomB), null);
            
            // Rig the exits and ensure both got rigged up to the correct destinations.
            this.exitBehavior.AddDestination("north", this.roomB.ID);
            this.exitBehavior.AddDestination("south", this.roomA.ID);
            Verify.AreSame(this.exitBehavior.GetDestination(this.roomA), this.roomB);
            Verify.AreSame(this.exitBehavior.GetDestination(this.roomB), this.roomA);

            // Create an unmovable actor, and ensure that said actor cannot move through.
            this.actor = new Thing() { Name = "Actor", Parent = this.roomA, ID = TestThingID.Generate("testactor") };
            this.exitBehavior.MoveThrough(this.actor);
            Verify.AreSame(this.actor.Parent, this.roomA);

            // Make the actoc movable, and ensure they end up in the next room when moving through.
            this.actor.Behaviors.Add(new MovableBehavior());
            this.exitBehavior.MoveThrough(this.actor);
            Verify.AreSame(this.actor.Parent, this.roomB);

            // Move the actor back through the exit again, and ensure they end up in the starting room.
            this.exitBehavior.MoveThrough(this.actor);
            Verify.AreSame(this.actor.Parent, this.roomA);

            // @@@ TODO: Ensure the actor does not move through when using the wrong context command?
            // @@@ TODO: Ensure the actor moves through both ways when using correct context commands?
        }
    }
}