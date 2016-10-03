//-----------------------------------------------------------------------------
// <copyright file="OpensClosesBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: May 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Actions;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;

    /// <summary>OpensClosesBehavior adds the ability to open and close a thing.</summary>
    public class OpensClosesBehavior : Behavior
    {
        /// <summary>The reused open and close strings.</summary>
        private const string OpenString = "open", CloseString = "close";

        /// <summary>The contextual commands for this behavior instance.</summary>
        private OpensClosesBehaviorCommands commands;

        /// <summary>Initializes a new instance of the OpensClosesBehavior class.</summary>
        public OpensClosesBehavior()
            : this(0, null)
        {
        }

        /// <summary>Initializes a new instance of the OpensClosesBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public OpensClosesBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.commands = new OpensClosesBehaviorCommands(this);
            this.ID = instanceID;
        }

        /// <summary>Gets a value indicating whether our state is currently "open".</summary>
        public bool IsOpen { get; private set; }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to this.Parent.)</summary>
        public override void OnAddBehavior()
        {
            // When adding this behavior to a Thing, register relevant movement events so we can cancel
            // the movement of anything through our parent Thing while our parent Thing is "closed".
            var parent = this.Parent;
            if (parent != null)
            {
                parent.Eventing.MovementRequest += this.MovementRequestHandler;

                // Register the "open" and "close" context commands to be available to siblings of our parent,
                // and to the openable/closable thing's children (IE in case it can be entered itself).
                var contextAvailability = ContextAvailability.ToSiblings | ContextAvailability.ToChildren;
                var openContextCommand = new ContextCommand(this.commands, OpenString, contextAvailability, SecurityRole.all);
                var closeContextCommand = new ContextCommand(this.commands, CloseString, contextAvailability, SecurityRole.all);
                parent.Commands.Add(OpenString, openContextCommand);
                parent.Commands.Add(CloseString, closeContextCommand);
            }

            base.OnAddBehavior();
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to this.Parent.)</summary>
        public override void OnRemoveBehavior()
        {
            var parent = this.Parent;
            if (parent != null)
            {
                parent.Eventing.MovementRequest -= this.MovementRequestHandler;
                parent.Commands.Remove(OpenString);
                parent.Commands.Remove(CloseString);
            }

            base.OnRemoveBehavior();
        }

        /// <summary>Attempt to open this behavior's parent, via the specified opener.</summary>
        /// <param name="opener">The actor doing the opening.</param>
        public void Open(Thing opener)
        {
            this.OpenOrClose(opener, OpenString, true);
        }

        /// <summary>Attempt to close this behavior's parent, via the specified closer.</summary>
        /// <param name="closer">The actor doing the closing.</param>
        public void Close(Thing closer)
        {
            this.OpenOrClose(closer, CloseString, false);
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.IsOpen = false;
        }

        /// <summary>Handle any movement requests.</summary>
        /// <param name="root">The root Thing where this event broadcast started.</param>
        /// <param name="e">The cancellable event/request arguments.</param>
        private void MovementRequestHandler(Thing root, CancellableGameEvent e)
        {
            // Only cancel movement requests through our parent if it is currently closed.
            if (!this.IsOpen)
            {
                var parent = this.Parent;
                if (parent != null)
                {
                    // If this is a standard movement request, find out if we need to cancel it.
                    var movementEvent = e as MovementEvent;
                    if (movementEvent != null && movementEvent.GoingVia == this.Parent)
                    {
                        // @@@ TODO: If the actor also cannot perceive our parent properly, perhaps broadcast
                        //     a sensory event like "Dude blindly ran into a door."
                        string message = string.Format("You cannot move through {0} since it is closed!", this.Parent.Name);
                        movementEvent.Cancel(message);
                    }
                }
            }
        }

        /// <summary>Open or close this behavior's parent, via the specified actor.</summary>
        /// <param name="actor">The actor doing the opening or closing.</param>
        /// <param name="verb">Whether this is an "open" or "close" action.</param>
        /// <param name="newOpenedState">The new IsOpen state to be set, if the request is not cancelled.</param>
        private void OpenOrClose(Thing actor, string verb, bool newOpenedState)
        {
            // If we're already in the desired opened/closed state, we're already done with state changes.
            if (newOpenedState == this.IsOpen)
            {
                // @@@ TODO: Message to the actor that it is already open/closed.
                return;
            }

            // Prepare the Close/Open game event for sending as a request, and if not cancelled, again as an event.
            var csb = new ContextualStringBuilder(actor, this.Parent);
            csb.Append(@"You " + verb + " $TargetThing.Name.", ContextualStringUsage.OnlyWhenBeingPassedToOriginator);
            csb.Append(@"$ActiveThing.Name " + verb + "s you.", ContextualStringUsage.OnlyWhenBeingPassedToReceiver);
            csb.Append(@"$ActiveThing.Name " + verb + "s $TargetThing.Name.", ContextualStringUsage.WhenNotBeingPassedToReceiverOrOriginator);
            var message = new SensoryMessage(SensoryType.Sight, 100, csb);
            var e = new OpenCloseEvent(this.Parent, newOpenedState, actor, message);

            // Broadcast the Open or Close Request and carry on if nothing cancelled it.
            // Use a temporary ref to our own parent to avoid race conditions like sudden parent removal.
            var thisThing = this.Parent;
            if (thisThing != null)
            {
                // Broadcast from the parents of the openable/closable thing (IE the rooms an openable exit is attached to).
                thisThing.Eventing.OnMiscellaneousRequest(e, EventScope.ParentsDown);
                if (!e.IsCancelled)
                {
                    // Open or Close the thing.
                    this.IsOpen = newOpenedState;

                    // Broadcast the Open or Close event.
                    thisThing.Eventing.OnMiscellaneousEvent(e, EventScope.ParentsDown);
                }
            }
        }

        /// <summary>Contextual commands for the OpensClosesBehavior.</summary>
        private class OpensClosesBehaviorCommands : GameAction
        {
            /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
            private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
            {
                CommonGuards.InitiatorMustBeAlive, 
                CommonGuards.InitiatorMustBeConscious,
                CommonGuards.InitiatorMustBeBalanced,
                CommonGuards.InitiatorMustBeMobile,
            };

            /// <summary>The OpensClosesBehavior this class belongs to.</summary>
            private OpensClosesBehavior opensClosesBehavior;

            /// <summary>Initializes a new instance of the OpensClosesBehaviorCommands class.</summary>
            /// <param name="opensClosesBehavior">The OpensClosesBehavior this class belongs to.</param>
            public OpensClosesBehaviorCommands(OpensClosesBehavior opensClosesBehavior)
                : base()
            {
                this.opensClosesBehavior = opensClosesBehavior;
            }

            /// <summary>Execute the action.</summary>
            /// <param name="actionInput">The full input specified for executing the command.</param>
            public override void Execute(ActionInput actionInput)
            {
                // If the user invoked the context command, try to open or close their target.
                if (OpenString.Equals(actionInput.Noun, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.opensClosesBehavior.Open(actionInput.Controller.Thing);
                }
                else if (CloseString.Equals(actionInput.Noun, StringComparison.CurrentCultureIgnoreCase))
                {
                    this.opensClosesBehavior.Close(actionInput.Controller.Thing);
                }
            }

            /// <summary>Checks against the guards for the command.</summary>
            /// <param name="actionInput">The full input specified for executing the command.</param>
            /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
            public override string Guards(ActionInput actionInput)
            {
                string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
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