//-----------------------------------------------------------------------------
// <copyright file="TestFollowingBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using NUnit.Framework;
    using WheelMUD.Core;

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
            witness = new Thing() { Name = "Witness", Id = TestThingID.Generate("testthing") };
            stalker1 = new Thing() { Name = "Stalker1", Id = TestThingID.Generate("testthing") };
            stalker2 = new Thing() { Name = "Stalker2", Id = TestThingID.Generate("testthing") };
            victim1 = new Thing() { Name = "Victim1", Id = TestThingID.Generate("testthing") };
            victim2 = new Thing() { Name = "Victim2", Id = TestThingID.Generate("testthing") };

            // Set up the rooms.
            room1 = new Thing() { Name = "Room", Id = TestThingID.Generate("room") };
            room2 = new Thing() { Name = "Room 2", Id = TestThingID.Generate("room") };

            // Set up an exit connecting the two rooms.
            exit = new Thing() { Name = "East Exit", Id = TestThingID.Generate("exit") };
            var exitBehavior = new ExitBehavior();
            ////exitBehavior.AddDestination("west", room1.ID);
            ////exitBehavior.AddDestination("east", room1.ID);
            ////exit.BehaviorManager.Add(exitBehavior);

            room1.Add(exit);
            room2.Add(exit);

            // Populate the first room.
            room1.Add(witness);
            room1.Add(stalker1);
            room1.Add(stalker2);
            room1.Add(victim1);
            room1.Add(victim2);

            // Prepare to verify correct eventing occurs.
            witness.Eventing.MovementRequest += (root, e) => { lastWitnessRequest = e; };
            witness.Eventing.MovementEvent += (root, e) => { lastWitnessEvent = e; };
            stalker1.Eventing.MovementRequest += (root, e) => { lastStalkerRequest = e; };
            stalker1.Eventing.MovementEvent += (root, e) => { lastStalkerEvent = e; };
            stalker2.Eventing.MovementRequest += (root, e) => { lastStalkerRequest = e; };
            stalker2.Eventing.MovementEvent += (root, e) => { lastStalkerEvent = e; };
            victim1.Eventing.MovementRequest += (root, e) => { lastVictimRequest = e; };
            victim1.Eventing.MovementEvent += (root, e) => { lastVictimEvent = e; };
            victim2.Eventing.MovementRequest += (root, e) => { lastVictimRequest = e; };
            victim2.Eventing.MovementEvent += (root, e) => { lastVictimEvent = e; };
        }

        /// <summary>Test FollowingBehavior without an attached parent.</summary>
        [Test]
        public void TestUnattachedFollowingBehaviorCanNotTrackTargets()
        {
            var followingBehavior = new FollowingBehavior();
            followingBehavior.Target = victim1;
            Assert.AreEqual(null, followingBehavior.Target);
        }

        /// <summary>Verify that the default target is null.</summary>
        [Test]
        public void TestFollowingBehaviorStartsOutNull()
        {
            ClearTrackedEvents();
            var followingBehavior = new FollowingBehavior();
            stalker1.Behaviors.Add(followingBehavior);
            Assert.IsNull(followingBehavior.Target);
        }

        /// <summary>Verify that Target changes to the specified victim.</summary>
        [Test]
        public void TestSettingTarget()
        {
            ClearTrackedEvents();
            var followingBehavior = new FollowingBehavior();
            stalker1.Behaviors.Add(followingBehavior);
            followingBehavior.Target = victim1;
            Assert.IsNotNull(followingBehavior.Target);
            Assert.AreEqual(followingBehavior.Target, victim1);
        }

        /// <summary>
        /// Verify that removing a target (setting to null) works and doesn't throw exceptions.
        /// Verify that the stalker and victim saw the follow, but it wasn't seen by the witness
        /// </summary>
        [Test]
        public void TestRemovingTarget()
        {
            ClearTrackedEvents();

            Assert.IsNull(lastStalkerRequest);
            Assert.IsNull(lastStalkerEvent);

            Assert.IsNull(lastVictimRequest);
            Assert.IsNull(lastVictimEvent);

            var followingBehavior = new FollowingBehavior();
            stalker1.Behaviors.Add(followingBehavior);
            followingBehavior.Target = victim1;
            followingBehavior.Target = null;

            Assert.IsNull(followingBehavior.Target);

            Assert.IsNotNull(lastStalkerRequest);
            Assert.IsNotNull(lastStalkerEvent);

            Assert.IsNotNull(lastVictimRequest);
            Assert.IsNotNull(lastVictimEvent);
        }

        /// <summary>Verify sensory messages seen by stalker/victim and unseen by witness.</summary>
        [Test]
        public void TestFollowingMessages()
        {
            ClearTrackedEvents();
            var followingBehavior = new FollowingBehavior();
            stalker1.Behaviors.Add(followingBehavior);
            Assert.AreEqual(lastWitnessEvent, null);
            Assert.AreEqual(lastStalkerEvent, null);
            Assert.AreEqual(lastVictimEvent, null);

            followingBehavior.Target = victim1;
            var witnessMessage = lastWitnessEvent.SensoryMessage.Message.Parse(witness);
            var stalkerMessage = lastStalkerEvent.SensoryMessage.Message.Parse(stalker1);
            var victimMessage = lastVictimEvent.SensoryMessage.Message.Parse(victim1);
            Assert.AreEqual(witnessMessage, "Stalker1 starts following Victim1.");
            Assert.AreEqual(stalkerMessage, "You start following Victim1.");
            Assert.AreEqual(victimMessage, "Stalker1 starts following you.");

            followingBehavior.Target = null;
            witnessMessage = lastWitnessEvent.SensoryMessage.Message.Parse(witness);
            stalkerMessage = lastStalkerEvent.SensoryMessage.Message.Parse(stalker1);
            victimMessage = lastVictimEvent.SensoryMessage.Message.Parse(victim1);
            Assert.IsTrue(string.IsNullOrEmpty(witnessMessage));
            Assert.AreEqual(stalkerMessage, "You stop following Victim1.");
            Assert.AreEqual(victimMessage, "Stalker1 stops following you.");
        }

        /// <summary>Tests the garbage collected target.</summary>
        /// <remarks>TODO: Figure out whether it's the test or the code that's broken.</remarks>
        [Test]
        public void TestGarbageCollectedTarget()
        {
            ////ClearTrackedEvents();
            ////var followingBehavior = new FollowingBehavior();
            ////stalker1.BehaviorManager.Add(followingBehavior);
            ////followingBehavior.Target = victim1;

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
            lastStalkerEvent = null;
            lastStalkerRequest = null;
            lastVictimEvent = null;
            lastVictimRequest = null;
            lastWitnessEvent = null;
            lastWitnessRequest = null;
        }
    }
}
