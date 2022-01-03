//-----------------------------------------------------------------------------
// <copyright file="TestOpensClosesBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WheelMUD.Core;

namespace WheelMUD.Tests.Behaviors
{
    /// <summary>Tests for the OpensClosesBehavior class.</summary>
    [TestClass]
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
        [TestInitialize]
        public void Init()
        {
            // Create the basic actor instances and behavior for test.
            witnessThing = new Thing() { Name = "WitnessThing", Id = TestThingID.Generate("testthing") };
            actingThing = new Thing() { Name = "ActingThing", Id = TestThingID.Generate("testthing") };
            openableThing = new Thing() { Name = "OpenableThing", Id = TestThingID.Generate("testthing") };
            opensClosesBehavior = new OpensClosesBehavior();

            // Set up the actors inside another (which we'll call a "room" although it needn't actually be a room).
            room = new Thing() { Name = "Room", Id = TestThingID.Generate("room") };
            room.Add(witnessThing);
            room.Add(actingThing);
            room.Add(openableThing);

            // Prepare to verify correct eventing occurs.
            witnessThing.Eventing.MiscellaneousRequest += (root, e) => { lastWitnessRequest = e; };
            witnessThing.Eventing.MiscellaneousEvent += (root, e) => { lastWitnessEvent = e; };
            actingThing.Eventing.MiscellaneousRequest += (root, e) => { lastActorRequest = e; };
            actingThing.Eventing.MiscellaneousEvent += (root, e) => { lastActorEvent = e; };
        }

        /// <summary>Test OpensClosesBehavior without an attached parent.</summary>
        [TestMethod]
        public void TestUnattachedOpensClosesBehavior()
        {
            // Verify that an unattached behavior does not change state between Open/Close attempts, and
            // that such attempts do not throw. (This keeps the behavior solid in the face of the parent
            // being destroyed or whatnot as a race versus a user trying to activate it.)
            bool initialState = opensClosesBehavior.IsOpen;
            opensClosesBehavior.Close(actingThing);
            Assert.AreEqual(initialState, opensClosesBehavior.IsOpen);
            opensClosesBehavior.Open(actingThing);
            Assert.AreEqual(initialState, opensClosesBehavior.IsOpen);
        }

        /// <summary>Test normal OpensClosesBehavior operation.</summary>
        [TestMethod]
        public void TestOpeningAndClosing()
        {
            openableThing.Behaviors.Add(opensClosesBehavior);

            // Verify that the default state is closed.
            Assert.IsTrue(!opensClosesBehavior.IsOpen);

            // Closing closed thing => closed.
            ClearTrackedEvents();
            opensClosesBehavior.Close(actingThing);
            Assert.IsTrue(!opensClosesBehavior.IsOpen);

            // Verify that no event occurred (but any potentially-canceled requests are irrelevant).
            Assert.IsTrue(lastWitnessEvent == null);
            Assert.IsTrue(lastActorEvent == null);

            // Opening a closed thing => open.
            ClearTrackedEvents();
            opensClosesBehavior.Open(actingThing);
            Assert.IsTrue(opensClosesBehavior.IsOpen);

            // Verify that an appropriate close request and close event were witnessed by both the actor and the witness.
            Assert.IsTrue(lastWitnessRequest != null);
            Assert.IsTrue(lastActorRequest != null);
            Assert.IsTrue(lastWitnessEvent != null);
            Assert.IsTrue(lastActorEvent != null);

            string witnessMessage = lastWitnessEvent.SensoryMessage.Message.Parse(witnessThing);
            Assert.IsTrue(witnessMessage.Contains("opens"));
            string actorMessage = lastActorEvent.SensoryMessage.Message.Parse(actingThing);
            Assert.IsTrue(actorMessage.Contains("You open"));

            // Opening an open thing => open.
            ClearTrackedEvents();
            opensClosesBehavior.Open(actingThing);
            Assert.IsTrue(opensClosesBehavior.IsOpen);

            // Verify that no event occurred (but any potentially-canceled requests are irrelevant).
            Assert.IsTrue(lastWitnessEvent == null);
            Assert.IsTrue(lastActorEvent == null);

            // Closing an open thing => closed.
            ClearTrackedEvents();
            opensClosesBehavior.Close(actingThing);
            Assert.IsTrue(!opensClosesBehavior.IsOpen);

            // Verify that an appropriate open request and open event were witnessed by both the actor and the witness.
            Assert.IsTrue(lastWitnessRequest != null);
            Assert.IsTrue(lastActorRequest != null);
            Assert.IsTrue(lastWitnessEvent != null);
            Assert.IsTrue(lastActorEvent != null);

            witnessMessage = lastWitnessEvent.SensoryMessage.Message.Parse(witnessThing);
            Assert.IsTrue(witnessMessage.Contains("closes"));
            actorMessage = lastActorEvent.SensoryMessage.Message.Parse(actingThing);
            Assert.IsTrue(actorMessage.Contains("You close"));
        }

        /// <summary>Test special OpensClosesBehavior operations for exits.</summary>
        [TestMethod]
        public void TestOpeningClosingAndMovementForExits()
        {
            // Create two one-way exits and two rooms to attach them to.
            var openableExitA = new Thing() { Name = "OpenableExitA", Id = TestThingID.Generate("testthing") };
            var openableExitB = new Thing() { Name = "OpenableExitB", Id = TestThingID.Generate("testthing") };
            var roomA = new Thing(new RoomBehavior()) { Name = "Room A", Id = TestThingID.Generate("testroom") };
            var roomB = new Thing(new RoomBehavior()) { Name = "Room B", Id = TestThingID.Generate("testroom") };
            roomA.Add(openableExitA);
            roomB.Add(openableExitB);

            // Attach ExitBehavior and OpensClosesBehaviors in different orders though, to verify in test that 
            // eventing and such work correctly regardless of attachment order.
            var exitBehaviorA = new ExitBehavior();
            var exitBehaviorB = new ExitBehavior();
            var opensClosesBehaviorB = new OpensClosesBehavior();
            openableExitA.Behaviors.Add(exitBehaviorA);
            openableExitA.Behaviors.Add(opensClosesBehavior);
            openableExitB.Behaviors.Add(opensClosesBehaviorB);
            openableExitB.Behaviors.Add(exitBehaviorB);

            // Rig up behaviors so the actor can move, and move from one A to B, and from B to A.
            actingThing.Behaviors.Add(new MovableBehavior());
            exitBehaviorA.AddDestination("toB", roomB.Id);
            exitBehaviorB.AddDestination("toA", roomA.Id);

            // Ensure that the actingThing cannot move through either exit while it is in default (closed) state.
            roomA.Add(actingThing);
            exitBehaviorA.MoveThrough(actingThing);
            Assert.AreSame(roomA, actingThing.Parent);

            roomB.Add(actingThing);
            exitBehaviorB.MoveThrough(actingThing);
            Assert.AreSame(roomB, actingThing.Parent);

            // Ensure that the actingThing can open and move through each openable exit to get between rooms.
            opensClosesBehaviorB.Open(actingThing);
            exitBehaviorB.MoveThrough(actingThing);
            Assert.AreSame(roomA, actingThing.Parent);

            opensClosesBehavior.Open(actingThing);
            exitBehaviorA.MoveThrough(actingThing);
            Assert.AreSame(roomB, actingThing.Parent);
        }

        /// <summary>Clear all potentially tracked events so we can verify new ones.</summary>
        private void ClearTrackedEvents()
        {
            lastActorEvent = null;
            lastActorRequest = null;
            lastWitnessEvent = null;
            lastWitnessRequest = null;
        }
    }
}