//-----------------------------------------------------------------------------
// <copyright file="LocksUnlocksBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using WheelMUD.Actions;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;

    /// <summary>LocksUnlocksBehavior provides the ability to lock and unlock a Thing.</summary>
    public class LocksUnlocksBehavior : Behavior
    {
        /// <summary>The reused lock and unlock strings.</summary>
        private const string LockString = "lock", UnlockString = "unlock";

        /// <summary>The contextual commands for this behavior instance.</summary>
        private LocksUnlocksBehaviorCommands commands;

        /// <summary>Initializes a new instance of the LocksUnlocksBehavior class.</summary>
        public LocksUnlocksBehavior()
            : this(0, null)
        {
        }

        /// <summary>Initializes a new instance of the LocksUnlocksBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public LocksUnlocksBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.commands = new LocksUnlocksBehaviorCommands(this);
            this.ID = instanceID;
        }

        /// <summary>Gets a value indicating whether the attached thing is currently locked.</summary>
        public bool IsLocked { get; private set; }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to this.Parent)</summary>
        protected override void OnAddBehavior()
        {
            var parent = this.Parent;
            if (parent != null)
            {
                // When adding this behavior to a Thing, register relevant events so we can cancel
                // the opening of our parent Thing while our parent Thing is "locked".
                parent.Eventing.MiscellaneousRequest += this.RequestHandler;

                // Register the "lock" and "unlock" context commands to be available to siblings of our parent,
                // and to the lockable/unlockable thing's children (IE in case it can be entered itself).
                var contextAvailability = ContextAvailability.ToSiblings | ContextAvailability.ToChildren;
                var lockContextCommand = new ContextCommand(this.commands, LockString, contextAvailability, SecurityRole.all);
                var unlockContextCommand = new ContextCommand(this.commands, UnlockString, contextAvailability, SecurityRole.all);
                Debug.Assert(!parent.Commands.ContainsKey(LockString), "The Thing this LocksUnlocksBehavior attached to already had a Lock command.");
                Debug.Assert(!parent.Commands.ContainsKey(UnlockString), "The Thing this LocksUnlocksBehavior attached to already had an Unlock command.");
                parent.Commands.Add(LockString, lockContextCommand);
                parent.Commands.Add(UnlockString, unlockContextCommand);
            }

            base.OnAddBehavior();
        }

        /// <summary>Lock this Thing.</summary>
        /// <param name="locker">The actor who is locking this Thing.</param>
        public void Lock(Thing locker)
        {
            this.LockOrUnlock(locker, LockString, true);
        }

        /// <summary>Unlock this Thing.</summary>
        /// <param name="unlocker">The actor who is unlocking this Thing.</param>
        public void Unlock(Thing unlocker)
        {
            this.LockOrUnlock(unlocker, UnlockString, false);
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.IsLocked = true;
        }

        /// <summary>Lock or unlock this behavior's parent, via the specified actor.</summary>
        /// <param name="actor">The actor doing the locking or unlocking.</param>
        /// <param name="verb">Whether this is an "lock" or "unlock" action.</param>
        /// <param name="newLockedState">The new IsLocked state to be set, if the request is not cancelled.</param>
        private void LockOrUnlock(Thing actor, string verb, bool newLockedState)
        {
            // If we're already in the desired locked/unlocked state, we're already done with state changes.
            if (newLockedState == this.IsLocked)
            {
                // @@@ TODO: Message to the actor that it is already locked/unlocked.
                return;
            }

            // Use a temporary ref to our own parent to avoid race conditions like sudden parent removal.
            var thisThing = this.Parent;
            if (thisThing == null)
            {
                return; // Abort if the behavior is unattached (e.g. being destroyed).
            }
            
            if (newLockedState && thisThing != null)
            {
                // If we are attempting to lock an opened thing, cancel the lock attempt.
                var opensClosesBehavior = thisThing.Behaviors.FindFirst<OpensClosesBehavior>();
                if (opensClosesBehavior != null && opensClosesBehavior.IsOpen)
                {
                    // @@@ TODO: Message to the actor that they can't lock an open thing.
                    return;
                }
            }

            // Prepare the Lock/Unlock game event for sending as a request, and if not cancelled, again as an event.
            var contextMessage = new ContextualString(actor, thisThing)
            {
                ToOriginator = $"You {verb} {thisThing.Name}.",
                ToReceiver = $"{actor.Name} {verb}s you.",
                ToOthers = $"{actor.Name} {verb}s {thisThing.Name}.",
            };
            var message = new SensoryMessage(SensoryType.Sight, 100, contextMessage);
            var e = new LockUnlockEvent(thisThing, false, actor, message);

            // Broadcast the Lock or Unlock Request and carry on if nothing cancelled it.
            // Broadcast from the parents of the lockable/unlockable thing (IE a room or inventory where the lockable resides).
            thisThing.Eventing.OnMiscellaneousRequest(e, EventScope.ParentsDown);
            if (!e.IsCancelled)
            {
                // Lock or Unlock the thing.
                this.IsLocked = newLockedState;

                // Broadcast the Lock or Unlock event.
                thisThing.Eventing.OnMiscellaneousEvent(e, EventScope.ParentsDown);
            }
        }

        /// <summary>Handle any requests this behavior is registered to.</summary>
        /// <param name="root">The root Thing where this event broadcast started.</param>
        /// <param name="e">The cancellable event/request arguments.</param>
        private void RequestHandler(Thing root, CancellableGameEvent e)
        {
            // Only cancel requestes to open our parent if it is currently locked.
            if (this.IsLocked)
            {
                var parent = this.Parent;
                if (parent != null)
                {
                    // If this is a standard open request, find out if we need to cancel it.
                    var openCloseEvent = e as OpenCloseEvent;
                    if (openCloseEvent != null && openCloseEvent.IsBeingOpened && openCloseEvent.Target == this.Parent)
                    {
                        string message = string.Format("You cannot open {0} since it is locked!", this.Parent.Name);
                        openCloseEvent.Cancel(message);
                    }
                }
            }
        }

        /// <summary>Contextual commands for the LocksUnlocksBehavior.</summary>
        private class LocksUnlocksBehaviorCommands : GameAction
        {
            /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
            private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
            {
                CommonGuards.InitiatorMustBeAlive, 
                CommonGuards.InitiatorMustBeConscious,
                CommonGuards.InitiatorMustBeBalanced,
                CommonGuards.InitiatorMustBeMobile,
            };

            /// <summary>The LocksUnlocksBehavior this class belongs to.</summary>
            private LocksUnlocksBehavior locksUnlocksBehavior;

            /// <summary>Initializes a new instance of the LocksUnlocksBehaviorCommands class.</summary>
            /// <param name="locksUnlocksBehavior">The OpensClosesBehavior this class belongs to.</param>
            public LocksUnlocksBehaviorCommands(LocksUnlocksBehavior locksUnlocksBehavior)
                : base()
            {
                this.locksUnlocksBehavior = locksUnlocksBehavior;
            }

            /// <summary>Execute the action.</summary>
            /// <param name="actionInput">The full input specified for executing the command.</param>
            public override void Execute(ActionInput actionInput)
            {
                // If the user invoked the context command, try to lock or unlock their target.
                if (LockString.Equals(actionInput.Noun, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.locksUnlocksBehavior.Lock(actionInput.Controller.Thing);
                }
                else if (UnlockString.Equals(actionInput.Noun, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.locksUnlocksBehavior.Unlock(actionInput.Controller.Thing);
                }
            }

            /// <summary>Checks against the guards for the command.</summary>
            /// <param name="actionInput">The full input specified for executing the command.</param>
            /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
            public override string Guards(ActionInput actionInput)
            {
                string commonFailure = this.VerifyCommonGuards(actionInput, ActionGuards);
                if (commonFailure != null)
                {
                    return commonFailure;
                }

                if (actionInput.Controller.Thing.Behaviors.FindFirst<MovableBehavior>() == null)
                {
                    return "You do not have the ability to move it.";
                }

                return null;
            }
        }
    }
}