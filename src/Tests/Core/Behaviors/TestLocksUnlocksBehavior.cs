//-----------------------------------------------------------------------------
// <copyright file="TestLocksUnlocksBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: March 2012 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WheelMUD.Core;
    using WheelMUD.Core.Events;
    using WheelMUD.Universe;

    /// <summary>Tests for the TestLocksUnlocksBehavior class.</summary>
    [TestClass]
    [TestFixture]
    public class TestLocksUnlocksBehavior
    {
        /// <summary>Common actors in the test.</summary>
        private Thing witnessThing, actingThing, lockableThing, room;

        /// <summary>The basic LocksUnlocksBehavior instance to test.</summary>
        private LocksUnlocksBehavior locksUnlocksBehavior;

        /// <summary>Keeps track of the last requests received, for easy verification.</summary>
        private CancellableGameEvent lastWitnessRequest, lastActorRequest;

        /// <summary>Keeps track of the last events received, for easy verification.</summary>
        private GameEvent lastWitnessEvent, lastActorEvent;

        /// <summary>Common preparation for all LocksUnlocksBehavior tests.</summary>
        [TestInitialize]
        [SetUp]
        public void Init()
        {
            // Create the basic actor instances and behavior for test.
            this.witnessThing = new Thing() { Name = "WitnessThing", ID = TestThingID.Generate("testthing") };
            this.actingThing = new Thing() { Name = "ActingThing", ID = TestThingID.Generate("testthing") };
            this.lockableThing = new Thing() { Name = "LockableThing", ID = TestThingID.Generate("testthing") };
            this.locksUnlocksBehavior = new LocksUnlocksBehavior();

            // Set up the actors inside another (which we'll call a "room" although it needn't actually be a room).
            this.room = new Thing() { Name = "Room", ID = TestThingID.Generate("room") };
            this.room.Add(witnessThing);
            this.room.Add(actingThing);
            this.room.Add(lockableThing);

            // Prepare to verify correct eventing occurs.
            this.witnessThing.Eventing.MiscellaneousRequest += (root, e) => { this.lastWitnessRequest = e; };
            this.witnessThing.Eventing.MiscellaneousEvent += (root, e) => { this.lastWitnessEvent = e; };
            this.actingThing.Eventing.MiscellaneousRequest += (root, e) => { this.lastActorRequest = e; };
            this.actingThing.Eventing.MiscellaneousEvent += (root, e) => { this.lastActorEvent = e; };
        }

        /// <summary>Test LocksUnlocksBehavior without an attached parent.</summary>
        [TestMethod]
        [Test]
        public void TestUnattachedLocksUnlocksBehavior()
        {
            // Verify that an unattached behavior does not change state between Lock/Unlock attempts, and
            // that such attempts do not throw. (This keeps the behavior solid in the face of the parent
            // being destroyed or whatnot as a race versus a user trying to activate it.)
            bool initialState = this.locksUnlocksBehavior.IsLocked;
            this.locksUnlocksBehavior.Lock(this.actingThing);
            Verify.Equals(initialState, this.locksUnlocksBehavior.IsLocked);
            this.locksUnlocksBehavior.Unlock(this.actingThing);
            Verify.Equals(initialState, this.locksUnlocksBehavior.IsLocked);
        }

        /// <summary>Test normal LocksUnlocksBehavior operation.</summary>
        [TestMethod]
        [Test]
        public void TestLockingAndUnlocking()
        {
            this.lockableThing.Behaviors.Add(this.locksUnlocksBehavior);

            // Verify that the default state is locked.
            Verify.IsTrue(this.locksUnlocksBehavior.IsLocked);

            // Verify locking a locked thing => locked.
            this.ClearTrackedEvents();
            this.locksUnlocksBehavior.Lock(this.actingThing);
            Verify.IsTrue(this.locksUnlocksBehavior.IsLocked);

            // Verify that no event occurred (but any potentially-cancelled requests are irrelevant).
            Verify.IsTrue(this.lastWitnessEvent == null);
            Verify.IsTrue(this.lastActorEvent == null);

            // Verify unlocking a locked thing => unlocked.
            this.ClearTrackedEvents();
            this.locksUnlocksBehavior.Unlock(this.actingThing);
            Verify.IsTrue(!this.locksUnlocksBehavior.IsLocked);

            // Verify that an appropriate unlock request and unlock event were witnessed by both the actor and the witness.
            Verify.IsTrue(this.lastWitnessRequest != null);
            Verify.IsTrue(this.lastActorRequest != null);
            Verify.IsTrue(this.lastWitnessEvent != null);
            Verify.IsTrue(this.lastActorEvent != null);

            string witnessMessage = this.lastWitnessEvent.SensoryMessage.Message.Parse(this.witnessThing);
            Verify.IsTrue(witnessMessage.Contains(" unlocks "));
            string actorMessage = this.lastActorEvent.SensoryMessage.Message.Parse(this.actingThing);
            Verify.IsTrue(actorMessage.Contains("You unlock "));

            // Verify unlocking an unlocked thing => unlocked.
            this.ClearTrackedEvents();
            this.locksUnlocksBehavior.Unlock(this.actingThing);
            Verify.IsTrue(!this.locksUnlocksBehavior.IsLocked);

            // Verify that no event occurred (but any potentially-cancelled requests are irrelevant).
            Verify.IsTrue(this.lastWitnessEvent == null);
            Verify.IsTrue(this.lastActorEvent == null);

            // Verify locking an unlocked thing => locked.
            this.ClearTrackedEvents();
            this.locksUnlocksBehavior.Lock(this.actingThing);
            Verify.IsTrue(this.locksUnlocksBehavior.IsLocked);

            // Verify that an appropriate lock request and lock event were witnessed by both the actor and the witness.
            Verify.IsTrue(this.lastWitnessRequest != null);
            Verify.IsTrue(this.lastActorRequest != null);
            Verify.IsTrue(this.lastWitnessEvent != null);
            Verify.IsTrue(this.lastActorEvent != null);

            witnessMessage = this.lastWitnessEvent.SensoryMessage.Message.Parse(this.witnessThing);
            Verify.IsTrue(witnessMessage.Contains(" locks "));
            actorMessage = this.lastActorEvent.SensoryMessage.Message.Parse(this.actingThing);
            Verify.IsTrue(actorMessage.Contains("You lock "));
        }

        /// <summary>Test special LocksUnlocksBehavior operations for openable things.</summary>
        [TestMethod]
        [Test]
        public void TestOpeningAndClosingOfLockedAndUnlockedThings()
        {
            // Make our lockable thing also openable.
            var opensClosesBehavior = new OpensClosesBehavior();
            this.lockableThing.Behaviors.Add(opensClosesBehavior);
            this.lockableThing.Behaviors.Add(this.locksUnlocksBehavior);

            // Verify that the thing is still in the default locked state.
            Verify.IsTrue(this.locksUnlocksBehavior.IsLocked);

            // Verify that attempts to open the locked thing do not work.
            // (Note that adding player-convenience features like automatic unlock attempts on behalf of the 
            // player when trying to open something, depending on their settings or whatnot, may require such 
            // tests to become much more robust here.)
            opensClosesBehavior.Open(this.actingThing);
            Verify.IsTrue(this.locksUnlocksBehavior.IsLocked);
            Verify.IsTrue(!opensClosesBehavior.IsOpen);

            // Verify that attempts to open an unlocked thing do work though.
            this.locksUnlocksBehavior.Unlock(this.actingThing);
            opensClosesBehavior.Open(this.actingThing);
            Verify.IsTrue(!this.locksUnlocksBehavior.IsLocked);
            Verify.IsTrue(opensClosesBehavior.IsOpen);

            // Verify that trying to lock an open thing is either cancelled (leaving it open and unlocked) 
            // or is automatically closed for the actor since the intent could be implied.
            this.locksUnlocksBehavior.Lock(this.actingThing);
            bool isClosedAndLocked = !opensClosesBehavior.IsOpen && this.locksUnlocksBehavior.IsLocked;
            bool isOpenAndUnlocked = opensClosesBehavior.IsOpen && !this.locksUnlocksBehavior.IsLocked;
            Verify.IsTrue(isClosedAndLocked || isOpenAndUnlocked);
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