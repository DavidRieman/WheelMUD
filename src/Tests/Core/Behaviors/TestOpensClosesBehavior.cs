//-----------------------------------------------------------------------------
// <copyright file="TestOpensClosesBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: January 2012 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WheelMUD.Core;
    using WheelMUD.Core.Events;
    
    /// <summary>Tests for the OpensClosesBehavior class.</summary>
    [TestClass][TestFixture]
    public class TestOpensClosesBehavior
    {
        /// <summary>Common actors in the test.</summary>
        private Thing witnessThing, actingThing, openableThing, room;

        /// <summary>The basic OpensClosesBehavior instance to test.</summary>
        private OpensClosesBehavior opensClosesBehavior;

        /// <summary>Keeps track of the last requests received, for easy verification.</summary>
        private CancellableGameEvent lastWitnessRequest, lastActorRequest;

        /// <summary>Keeps track of the last events received, for easy verification.</summary>
        private GameEvent lastWitnessEvent, lastActorEvent;

        /// <summary>Common preparation for all OpensClosesBehavior tests.</summary>
        [TestInitialize][SetUp]
        public void Init()
        {
            // Create the basic actor instances and behavior for test.
            this.witnessThing = new Thing() { Name = "WitnessThing", ID = TestThingID.Generate("testthing") };
            this.actingThing = new Thing() { Name = "ActingThing", ID = TestThingID.Generate("testthing") };
            this.openableThing = new Thing() { Name = "OpenableThing", ID = TestThingID.Generate("testthing") };
            this.opensClosesBehavior = new OpensClosesBehavior();

            // Set up the actors inside another (which we'll call a "room" although it needn't actually be a room).
            this.room = new Thing() { Name = "Room", ID = TestThingID.Generate("room") };
            this.room.Add(witnessThing);
            this.room.Add(actingThing);
            this.room.Add(openableThing);
            
            // Prepare to verify correct eventing occurs.
            this.witnessThing.Eventing.MiscellaneousRequest += (root, e) => { this.lastWitnessRequest = e; };
            this.witnessThing.Eventing.MiscellaneousEvent += (root, e) => { this.lastWitnessEvent = e; };
            this.actingThing.Eventing.MiscellaneousRequest += (root, e) => { this.lastActorRequest = e; };
            this.actingThing.Eventing.MiscellaneousEvent += (root, e) => { this.lastActorEvent = e; };
        }

        /// <summary>Test OpensClosesBehavior without an attached parent.</summary>
        [TestMethod][Test]
        public void TestUnattachedOpensClosesBehavior()
        {
            // Verify that an unattached behavior does not change state between Open/Close attempts, and
            // that such attempts do not throw. (This keeps the behavior solid in the face of the parent
            // being destroyed or whatnot as a race versus a user trying to activate it.)
            bool initialState = this.opensClosesBehavior.IsOpen;
            this.opensClosesBehavior.Close(this.actingThing);
            Verify.Equals(initialState, this.opensClosesBehavior.IsOpen);
            this.opensClosesBehavior.Open(this.actingThing);
            Verify.Equals(initialState, this.opensClosesBehavior.IsOpen);
        }

        /// <summary>Test normal OpensClosesBehavior operation.</summary>
        [TestMethod][Test]
        public void TestOpeningAndClosing()
        {
            this.openableThing.Behaviors.Add(this.opensClosesBehavior);

            // Verify that the default state is closed.
            Verify.IsTrue(!this.opensClosesBehavior.IsOpen);

            // Closing closed thing => closed.
            this.ClearTrackedEvents();
            this.opensClosesBehavior.Close(this.actingThing);
            Verify.IsTrue(!this.opensClosesBehavior.IsOpen);

            // Verify that no event occurred (but any potentially-cancelled requests are irrelevant).
            Verify.IsTrue(this.lastWitnessEvent == null);
            Verify.IsTrue(this.lastActorEvent == null);
            
            // Opening a closed thing => open.
            this.ClearTrackedEvents();
            this.opensClosesBehavior.Open(this.actingThing);
            Verify.IsTrue(this.opensClosesBehavior.IsOpen);

            // Verify that an appropriate close request and close event were witnessed by both the actor and the witness.
            Verify.IsTrue(this.lastWitnessRequest != null);
            Verify.IsTrue(this.lastActorRequest != null);
            Verify.IsTrue(this.lastWitnessEvent != null);
            Verify.IsTrue(this.lastActorEvent != null);

            string witnessMessage = this.lastWitnessEvent.SensoryMessage.Message.Parse(this.witnessThing);
            Verify.IsTrue(witnessMessage.Contains("opens"));
            string actorMessage = this.lastActorEvent.SensoryMessage.Message.Parse(this.actingThing);
            Verify.IsTrue(actorMessage.Contains("You open"));
            
            // Opening an open thing => open.
            this.ClearTrackedEvents();
            this.opensClosesBehavior.Open(this.actingThing);
            Verify.IsTrue(this.opensClosesBehavior.IsOpen);

            // Verify that no event occurred (but any potentially-cancelled requests are irrelevant).
            Verify.IsTrue(this.lastWitnessEvent == null);
            Verify.IsTrue(this.lastActorEvent == null);
            
            // Closing an open thing => closed.
            this.ClearTrackedEvents();
            this.opensClosesBehavior.Close(this.actingThing);
            Verify.IsTrue(!this.opensClosesBehavior.IsOpen);

            // Verify that an appropriate open request and open event were witnessed by both the actor and the witness.
            Verify.IsTrue(this.lastWitnessRequest != null);
            Verify.IsTrue(this.lastActorRequest != null);
            Verify.IsTrue(this.lastWitnessEvent != null);
            Verify.IsTrue(this.lastActorEvent != null);

            witnessMessage = this.lastWitnessEvent.SensoryMessage.Message.Parse(this.witnessThing);
            Verify.IsTrue(witnessMessage.Contains("closes"));
            actorMessage = this.lastActorEvent.SensoryMessage.Message.Parse(this.actingThing);
            Verify.IsTrue(actorMessage.Contains("You close"));
        }

        /// <summary>Test special OpensClosesBehavior operations for exits.</summary>
        [TestMethod][Test]
        public void TestOpeningClosingAndMovementForExits()
        {
            // Create two one-way exits and two rooms to attach them to.
            var openableExitA = new Thing() { Name = "OpenableExitA", ID = TestThingID.Generate("testthing") };
            var openableExitB = new Thing() { Name = "OpenableExitB", ID = TestThingID.Generate("testthing") };
            var roomA = new Thing(new RoomBehavior()) { Name = "Room A", ID = TestThingID.Generate("testroom") };
            var roomB = new Thing(new RoomBehavior()) { Name = "Room B", ID = TestThingID.Generate("testroom") };
            roomA.Add(openableExitA);
            roomB.Add(openableExitB);
            
            // Attach ExitBehavior and OpensClosesBehaviors in different orders though, to verify in test that 
            // eventing and such work correctly regardless of attachment order.
            var exitBehaviorA = new ExitBehavior();
            var exitBehaviorB = new ExitBehavior();
            var opensClosesBehaviorB = new OpensClosesBehavior();
            openableExitA.Behaviors.Add(exitBehaviorA);
            openableExitA.Behaviors.Add(this.opensClosesBehavior);
            openableExitB.Behaviors.Add(opensClosesBehaviorB);
            openableExitB.Behaviors.Add(exitBehaviorB);

            // Rig up behaviors so the actor can move, and move from one A to B, and from B to A.
            this.actingThing.Behaviors.Add(new MovableBehavior());
            exitBehaviorA.AddDestination("toB", roomB.ID);
            exitBehaviorB.AddDestination("toA", roomA.ID);
            
            // Ensure that the actingThing cannot move through either exit while it is in default (closed) state.
            roomA.Add(this.actingThing);
            exitBehaviorA.MoveThrough(this.actingThing);
            Verify.AreSame(roomA, this.actingThing.Parent);
            
            roomB.Add(this.actingThing);
            exitBehaviorB.MoveThrough(this.actingThing);
            Verify.AreSame(roomB, this.actingThing.Parent);

            // Ensure that the actingThing can open and move through each openable exit to get between rooms.
            opensClosesBehaviorB.Open(this.actingThing);
            exitBehaviorB.MoveThrough(this.actingThing);
            Verify.AreSame(roomA, this.actingThing.Parent);

            this.opensClosesBehavior.Open(this.actingThing);
            exitBehaviorA.MoveThrough(this.actingThing);
            Verify.AreSame(roomB, this.actingThing.Parent);
        }

        /// <summary>Clear all potentially tracked events so we can verify new ones.</summary>
        private void ClearTrackedEvents()
        {
            this.lastActorEvent = null;
            this.lastActorRequest = null;
            this.lastWitnessEvent = null;
            this.lastWitnessRequest = null;
        }
    }
}