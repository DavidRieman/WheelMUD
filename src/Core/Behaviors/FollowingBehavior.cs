//-----------------------------------------------------------------------------
// <copyright file="FollowingBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A behavior that indicates who the player/mob is currently following.
//   Created: 3/23/2012 by James McManus.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Raven.Imports.Newtonsoft.Json;
    using WheelMUD.Core.Events;
    using WheelMUD.Utilities;

    /// <summary>
    /// <para>
    /// The FollowingBehavior is applied to a player or mobile thing that is following another
    /// entity in the game. This could occur when one player follows another player, or perhaps
    /// when a monster is chasing its prey.
    /// </para>
    /// <para>
    /// The FollowingBehavior class exposes a Target property, which points to the thing being
    /// followed. It is currently only possible to follow one target at a time.
    /// </para>
    /// </summary>
    public class FollowingBehavior : Behavior
    {
        private WeakReference<Thing> target;

        /// <summary>Initializes a new instance of the FollowingBehavior class.</summary>
        public FollowingBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the FollowingBehavior class.</summary>
        /// <param name="instanceID">The instance ID.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public FollowingBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets or sets the target being followed.</summary>
        [JsonIgnore]
        public virtual Thing Target
        {
            get
            {
                return (this.target == null) ? null : this.target.Target;
            }

            set
            {
                // Strengthen the reference.
                var localTarget = this.target;

                // Do nothing if the target isn't changing.
                if ((localTarget != null && localTarget.Target == value) || (localTarget == null && value == null))
                {
                    return;
                }

                if (value == null)
                {
                    // localTarget won't also be null, per above
                    this.RemoveTarget();
                }
                else if (localTarget == null || localTarget.Target == null)
                {
                    // value won't also be null, per above
                    this.AddTarget(value);
                }
                else
                {
                    // localTarget and value are different objects
                    this.ChangeTarget(value);
                }
            }
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to this.Parent.)</summary>
        public override void OnRemoveBehavior()
        {
            // Setting Target to null ensures the MovementEvent handler is removed.
            this.Target = null;
            base.OnRemoveBehavior();
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }

        /// <summary>Processes the movement event.</summary>
        /// <param name="root">The root.</param>
        /// <param name="e">The e.</param>
        private void ProcessMovementEvent(Thing root, GameEvent e)
        {
            var self = this.Parent;
            var currentRoom = self.Parent;
            var arriveEvent = e as ArriveEvent;

            if (arriveEvent != null && arriveEvent.ActiveThing == this.Target && arriveEvent.GoingFrom == currentRoom)
            {
                var leaveMessage = this.CreateLeaveMessage(self);
                var arriveMessage = this.CreateArriveMessage(self);

                var movableBehavior = self.Behaviors.FindFirst<MovableBehavior>();
                movableBehavior.Move(arriveEvent.GoingTo, arriveEvent.GoingVia, leaveMessage, arriveMessage);
            }
        }

        /// <summary>Adds the new target.</summary>
        /// <remarks>Precondition: this.target is null, newTarget is non-null.</remarks>
        /// <param name="newTarget">The new target.</param>
        private void AddTarget(Thing newTarget)
        {
            var self = this.Parent;
            if (self != null)
            {
                var message = this.CreateFollowMessage(self, newTarget);
                var followEvent = new FollowEvent(self, message, self, newTarget);

                self.Eventing.OnMovementRequest(followEvent, EventScope.ParentsDown);

                if (!followEvent.IsCancelled)
                {
                    this.target = new WeakReference<Thing>(newTarget);
                    newTarget.Eventing.MovementEvent += this.ProcessMovementEvent;
                    self.Eventing.OnMovementEvent(followEvent, EventScope.ParentsDown);
                }
            }
        }

        /// <summary>
        /// Changes the target being followed.
        /// Simply removes the existing target and adds the specified new target using existing
        /// methods. This will emit messages for both activities.
        /// Precondition: this.target and newTarget are both non-null.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        private void ChangeTarget(Thing newTarget)
        {
            this.RemoveTarget();
            this.AddTarget(newTarget);
        }

        /// <summary>Stops following the current target.</summary>
        /// <remarks>Precondition: this.target is non-null.</remarks>
        private void RemoveTarget()
        {
            var self = this.Parent;
            if (self != null)
            {
                var oldTarget = this.target.Target;

                if (oldTarget != null)
                {
                    // Stop tracking the old target's movements
                    this.target.Target.Eventing.MovementEvent -= this.ProcessMovementEvent;

                    // Create an event with the appropriate "unfollow" sensory messages
                    var message = this.CreateUnfollowMessage(this.Parent, oldTarget);
                    var followEvent = new FollowEvent(this.Parent, message, this.Parent, oldTarget);

                    this.Parent.Eventing.OnMovementRequest(followEvent, EventScope.ParentsDown);

                    if (!followEvent.IsCancelled)
                    {
                        // Finally make the change
                        this.target.Target = null;

                        // Broadcast the change
                        this.Parent.Eventing.OnMovementEvent(followEvent, EventScope.ParentsDown);
                    }
                }
            }
        }

        private SensoryMessage CreateFollowMessage(Thing self, Thing newTarget)
        {
            var message = new ContextualString(self, newTarget)
            {
                ToOriginator = "You start following $Target.Name.",
                ToReceiver = "$ActiveThing.Name starts following you."
            };

            return new SensoryMessage(SensoryType.Sight, 100, message);
        }

        private SensoryMessage CreateUnfollowMessage(Thing self, Thing oldTarget)
        {
            var message = new ContextualString(self, oldTarget)
            {
                ToOriginator = "You stop following $Target.Name.",
                ToReceiver = "$ActiveThing.Name stops following you."
            };

            return new SensoryMessage(SensoryType.Sight, 100, message);
        }

        private SensoryMessage CreateArriveMessage(Thing self)
        {
            var message = new ContextualString(self, this.Target)
            {
                ToReceiver = "$ActiveThing.Name arrives, following you.",
                ToOthers = "$ActiveThing.Name arrives, following " + this.Target.Name + "."
            };

            return new SensoryMessage(SensoryType.Sight, 100, message);
        }

        private SensoryMessage CreateLeaveMessage(Thing self)
        {
            var message = new ContextualString(self, this.Target)
            {
                ToOriginator = "You follow " + this.Target.Name + ".",
                ToOthers = "$ActiveThing.Name leaves, following " + this.Target.Name + "."
            };

            return new SensoryMessage(SensoryType.Sight, 100, message);
        }
    }
}