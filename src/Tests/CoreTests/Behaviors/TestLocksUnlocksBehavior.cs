//-----------------------------------------------------------------------------
// <copyright file="TestLocksUnlocksBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using NUnit.Framework;
using WheelMUD.Core;
using WheelMUD.Universe;

namespace WheelMUD.Tests.Behaviors
{
    /// <summary>Tests for the TestLocksUnlocksBehavior class.</summary>
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
        [SetUp]
        public void Init()
        {
            // Create the basic actor instances and behavior for test.
            witnessThing = new Thing() { Name = "WitnessThing", Id = TestThingID.Generate("testthing") };
            actingThing = new Thing() { Name = "ActingThing", Id = TestThingID.Generate("testthing") };
            lockableThing = new Thing() { Name = "LockableThing", Id = TestThingID.Generate("testthing") };
            locksUnlocksBehavior = new LocksUnlocksBehavior();

            // Set up the actors inside another (which we'll call a "room" although it needn't actually be a room).
            room = new Thing() { Name = "Room", Id = TestThingID.Generate("room") };
            room.Add(witnessThing);
            room.Add(actingThing);
            room.Add(lockableThing);

            // Prepare to verify correct eventing occurs.
            witnessThing.Eventing.MiscellaneousRequest += (root, e) => { lastWitnessRequest = e; };
            witnessThing.Eventing.MiscellaneousEvent += (root, e) => { lastWitnessEvent = e; };
            actingThing.Eventing.MiscellaneousRequest += (root, e) => { lastActorRequest = e; };
            actingThing.Eventing.MiscellaneousEvent += (root, e) => { lastActorEvent = e; };
        }

        /// <summary>Test LocksUnlocksBehavior without an attached parent.</summary>
        [Test]
        public void TestUnattachedLocksUnlocksBehavior()
        {
            // Verify that an unattached behavior does not change state between Lock/Unlock attempts, and
            // that such attempts do not throw. (This keeps the behavior solid in the face of the parent
            // being destroyed or whatnot as a race versus a user trying to activate it.)
            bool initialState = locksUnlocksBehavior.IsLocked;
            locksUnlocksBehavior.Lock(actingThing);
            Assert.AreEqual(initialState, locksUnlocksBehavior.IsLocked);
            locksUnlocksBehavior.Unlock(actingThing);
            Assert.AreEqual(initialState, locksUnlocksBehavior.IsLocked);
        }

        /// <summary>Test normal LocksUnlocksBehavior operation.</summary>
        [Test]
        public void TestLockingAndUnlocking()
        {
            lockableThing.Behaviors.Add(locksUnlocksBehavior);

            // Verify that the default state is locked.
            Assert.IsTrue(locksUnlocksBehavior.IsLocked);

            // Verify locking a locked thing => locked.
            ClearTrackedEvents();
            locksUnlocksBehavior.Lock(actingThing);
            Assert.IsTrue(locksUnlocksBehavior.IsLocked);

            // Verify that no event occurred (but any potentially-cancelled requests are irrelevant).
            Assert.IsTrue(lastWitnessEvent == null);
            Assert.IsTrue(lastActorEvent == null);

            // Verify unlocking a locked thing => unlocked.
            ClearTrackedEvents();
            locksUnlocksBehavior.Unlock(actingThing);
            Assert.IsTrue(!locksUnlocksBehavior.IsLocked);

            // Verify that an appropriate unlock request and unlock event were witnessed by both the actor and the witness.
            Assert.IsTrue(lastWitnessRequest != null);
            Assert.IsTrue(lastActorRequest != null);
            Assert.IsTrue(lastWitnessEvent != null);
            Assert.IsTrue(lastActorEvent != null);

            string witnessMessage = lastWitnessEvent.SensoryMessage.Message.Parse(witnessThing);
            Assert.IsTrue(witnessMessage.Contains(" unlocks "));
            string actorMessage = lastActorEvent.SensoryMessage.Message.Parse(actingThing);
            Assert.IsTrue(actorMessage.Contains("You unlock "));

            // Verify unlocking an unlocked thing => unlocked.
            ClearTrackedEvents();
            locksUnlocksBehavior.Unlock(actingThing);
            Assert.IsTrue(!locksUnlocksBehavior.IsLocked);

            // Verify that no event occurred (but any potentially-cancelled requests are irrelevant).
            Assert.IsTrue(lastWitnessEvent == null);
            Assert.IsTrue(lastActorEvent == null);

            // Verify locking an unlocked thing => locked.
            ClearTrackedEvents();
            locksUnlocksBehavior.Lock(actingThing);
            Assert.IsTrue(locksUnlocksBehavior.IsLocked);

            // Verify that an appropriate lock request and lock event were witnessed by both the actor and the witness.
            Assert.IsTrue(lastWitnessRequest != null);
            Assert.IsTrue(lastActorRequest != null);
            Assert.IsTrue(lastWitnessEvent != null);
            Assert.IsTrue(lastActorEvent != null);

            witnessMessage = lastWitnessEvent.SensoryMessage.Message.Parse(witnessThing);
            Assert.IsTrue(witnessMessage.Contains(" locks "));
            actorMessage = lastActorEvent.SensoryMessage.Message.Parse(actingThing);
            Assert.IsTrue(actorMessage.Contains("You lock "));
        }

        /// <summary>Test special LocksUnlocksBehavior operations for openable things.</summary>
        [Test]
        public void TestOpeningAndClosingOfLockedAndUnlockedThings()
        {
            // Make our lockable thing also openable.
            var opensClosesBehavior = new OpensClosesBehavior();
            lockableThing.Behaviors.Add(opensClosesBehavior);
            lockableThing.Behaviors.Add(locksUnlocksBehavior);

            // Verify that the thing is still in the default locked state.
            Assert.IsTrue(locksUnlocksBehavior.IsLocked);

            // Verify that attempts to open the locked thing do not work.
            // (Note that adding player-convenience features like automatic unlock attempts on behalf of the 
            // player when trying to open something, depending on their settings or whatnot, may require such 
            // tests to become much more robust here.)
            opensClosesBehavior.Open(actingThing);
            Assert.IsTrue(locksUnlocksBehavior.IsLocked);
            Assert.IsTrue(!opensClosesBehavior.IsOpen);

            // Verify that attempts to open an unlocked thing do work though.
            locksUnlocksBehavior.Unlock(actingThing);
            opensClosesBehavior.Open(actingThing);
            Assert.IsTrue(!locksUnlocksBehavior.IsLocked);
            Assert.IsTrue(opensClosesBehavior.IsOpen);

            // Verify that trying to lock an open thing is either cancelled (leaving it open and unlocked) 
            // or is automatically closed for the actor since the intent could be implied.
            locksUnlocksBehavior.Lock(actingThing);
            bool isClosedAndLocked = !opensClosesBehavior.IsOpen && locksUnlocksBehavior.IsLocked;
            bool isOpenAndUnlocked = opensClosesBehavior.IsOpen && !locksUnlocksBehavior.IsLocked;
            Assert.IsTrue(isClosedAndLocked || isOpenAndUnlocked);
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