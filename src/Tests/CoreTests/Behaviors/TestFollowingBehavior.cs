//-----------------------------------------------------------------------------
// <copyright file="TestFollowingBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Test cases for FollowingBehavior.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using NUnit.Framework;
    using WheelMUD.Core;
    using WheelMUD.Core.Behaviors;
    using WheelMUD.Core.Events;

    /// <summary>Test cases for FollowingBehavior.</summary>
    [TestFixture]
    public class TestFollowingBehavior
    {
        /// <summary>Common actors in the test.</summary>
        private Thing witness, stalker1, stalker2, victim1, victim2, room1, room2, exit;

        /// <summary>Keeps track of the last requests received, for easy verification.</summary>
        private CancellableGameEvent lastWitnessRequest, lastStalkerRequest, lastVictimRequest;

        /// <summary>Keeps track of the last events received, for easy verification.</summary>
        private GameEvent lastWitnessEvent, lastStalkerEvent, lastVictimEvent;

        /// <summary>Common preparation for all FollowingBehavior tests.</summary>
        [SetUp]
        public void Init()
        {
            // Create the basic actor instances and behavior for test.
            this.witness = new Thing() { Name = "Witness", Id = TestThingID.Generate("testthing") };
            this.stalker1 = new Thing() { Name = "Stalker1", Id = TestThingID.Generate("testthing") };
            this.stalker2 = new Thing() { Name = "Stalker2", Id = TestThingID.Generate("testthing") };
            this.victim1 = new Thing() { Name = "Victim1", Id = TestThingID.Generate("testthing") };
            this.victim2 = new Thing() { Name = "Victim2", Id = TestThingID.Generate("testthing") };

            // Set up the rooms.
            this.room1 = new Thing() { Name = "Room", Id = TestThingID.Generate("room") };
            this.room2 = new Thing() { Name = "Room 2", Id = TestThingID.Generate("room") };

            // Set up an exit connecting the two rooms.
            this.exit = new Thing() { Name = "East Exit", Id = TestThingID.Generate("exit") };
            var exitBehavior = new ExitBehavior();
            ////exitBehavior.AddDestination("west", room1.ID);
            ////exitBehavior.AddDestination("east", room1.ID);
            ////this.exit.BehaviorManager.Add(exitBehavior);

            this.room1.Add(this.exit);
            this.room2.Add(this.exit);

            // Populate the first room.
            this.room1.Add(this.witness);
            this.room1.Add(this.stalker1);
            this.room1.Add(this.stalker2);
            this.room1.Add(this.victim1);
            this.room1.Add(this.victim2);

            // Prepare to verify correct eventing occurs.
            this.witness.Eventing.MovementRequest += (root, e) => { this.lastWitnessRequest = e; };
            this.witness.Eventing.MovementEvent += (root, e) => { this.lastWitnessEvent = e; };
            this.stalker1.Eventing.MovementRequest += (root, e) => { this.lastStalkerRequest = e; };
            this.stalker1.Eventing.MovementEvent += (root, e) => { this.lastStalkerEvent = e; };
            this.stalker2.Eventing.MovementRequest += (root, e) => { this.lastStalkerRequest = e; };
            this.stalker2.Eventing.MovementEvent += (root, e) => { this.lastStalkerEvent = e; };
            this.victim1.Eventing.MovementRequest += (root, e) => { this.lastVictimRequest = e; };
            this.victim1.Eventing.MovementEvent += (root, e) => { this.lastVictimEvent = e; };
            this.victim2.Eventing.MovementRequest += (root, e) => { this.lastVictimRequest = e; };
            this.victim2.Eventing.MovementEvent += (root, e) => { this.lastVictimEvent = e; };
        }

        /// <summary>Test FollowingBehavior without an attached parent.</summary>
        [Test]
        public void TestUnattachedFollowingBehaviorCanNotTrackTargets()
        {
            var followingBehavior = new FollowingBehavior();
            followingBehavior.Target = this.victim1;
            Assert.AreEqual(null, followingBehavior.Target);
        }

        /// <summary>Verify that the default target is null.</summary>
        [Test]
        public void TestFollowingBehaviorStartsOutNull()
        {
            this.ClearTrackedEvents();
            var followingBehavior = new FollowingBehavior();
            this.stalker1.Behaviors.Add(followingBehavior);
            Assert.IsNull(followingBehavior.Target);
        }

        /// <summary>Verify that Target changes to the specified victim.</summary>
        [Test]
        public void TestSettingTarget()
        {
            this.ClearTrackedEvents();
            var followingBehavior = new FollowingBehavior();
            this.stalker1.Behaviors.Add(followingBehavior);
            followingBehavior.Target = this.victim1;
            Assert.IsNotNull(followingBehavior.Target);
            Assert.AreEqual(followingBehavior.Target, this.victim1);
        }

        /// <summary>
        /// Verify that removing a target (setting to null) works and doesn't throw exceptions.
        /// Verify that the stalker and victim saw the follow, but it wasn't seen by the witness
        /// </summary>
        [Test]
        public void TestRemovingTarget()
        {
            this.ClearTrackedEvents();

            Assert.IsNull(this.lastStalkerRequest);
            Assert.IsNull(this.lastStalkerEvent);

            Assert.IsNull(this.lastVictimRequest);
            Assert.IsNull(this.lastVictimEvent);
            
            var followingBehavior = new FollowingBehavior();
            this.stalker1.Behaviors.Add(followingBehavior);
            followingBehavior.Target = this.victim1;
            followingBehavior.Target = null;

            Assert.IsNull(followingBehavior.Target);

            Assert.IsNotNull(this.lastStalkerRequest);
            Assert.IsNotNull(this.lastStalkerEvent);

            Assert.IsNotNull(this.lastVictimRequest);
            Assert.IsNotNull(this.lastVictimEvent);
        }

        /// <summary>Verify sensory messages seen by stalker/victim and unseen by witness.</summary>
        [Test]
        public void TestFollowingMessages()
        {
            this.ClearTrackedEvents();
            var followingBehavior = new FollowingBehavior();
            this.stalker1.Behaviors.Add(followingBehavior);
            Assert.AreEqual(this.lastWitnessEvent, null);
            Assert.AreEqual(this.lastStalkerEvent, null);
            Assert.AreEqual(this.lastVictimEvent, null);
            
            followingBehavior.Target = this.victim1;
            var witnessMessage = this.lastWitnessEvent.SensoryMessage.Message.Parse(this.witness);
            var stalkerMessage = this.lastStalkerEvent.SensoryMessage.Message.Parse(this.stalker1);
            var victimMessage = this.lastVictimEvent.SensoryMessage.Message.Parse(this.victim1);
            Assert.AreEqual(witnessMessage, "Stalker1 starts following Victim1.");
            Assert.AreEqual(stalkerMessage, "You start following Victim1.");
            Assert.AreEqual(victimMessage, "Stalker1 starts following you.");

            followingBehavior.Target = null;
            witnessMessage = this.lastWitnessEvent.SensoryMessage.Message.Parse(this.witness);
            stalkerMessage = this.lastStalkerEvent.SensoryMessage.Message.Parse(this.stalker1);
            victimMessage = this.lastVictimEvent.SensoryMessage.Message.Parse(this.victim1);
            Assert.IsTrue(string.IsNullOrEmpty(witnessMessage));
            Assert.AreEqual(stalkerMessage, "You stop following Victim1.");
            Assert.AreEqual(victimMessage, "Stalker1 stops following you.");
        }

        /// <summary>Tests the garbage collected target.</summary>
        /// <remarks>TODO: Figure out whether it's the test or the code that's broken.</remarks>
        [Test]
        public void TestGarbageCollectedTarget()
        {
            ////this.ClearTrackedEvents();
            ////var followingBehavior = new FollowingBehavior();
            ////stalker1.BehaviorManager.Add(followingBehavior);
            ////followingBehavior.Target = this.victim1;

            ////Assert.IsNotNull(followingBehavior.Target);
            ////victim1 = null;
            ////GC.Collect();
            ////Assert.IsNotNull(followingBehavior.Target);

            ////var localVictim = new Thing() { Name = "LocalVictim", ID = TestThingID.Generate("testthing") };
            ////followingBehavior.Target = localVictim;

            ////Assert.IsNotNull(followingBehavior.Target);
            ////localVictim = null;
            ////GC.Collect();
            ////Assert.IsNull(followingBehavior.Target);
        }

        /// <summary>Clear all potentially tracked events so we can verify new ones.</summary>
        private void ClearTrackedEvents()
        {
            this.lastStalkerEvent = null;
            this.lastStalkerRequest = null;
            this.lastVictimEvent = null;
            this.lastVictimRequest = null;
            this.lastWitnessEvent = null;
            this.lastWitnessRequest = null;
        }
    }
}
