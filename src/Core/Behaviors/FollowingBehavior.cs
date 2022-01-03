//-----------------------------------------------------------------------------
// <copyright file="FollowingBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    /// <summary>A behavior for an entity to follow another target entity around.</summary>
    /// <remarks>
    /// The FollowingBehavior is applied to a player or mobile thing that is following another
    /// entity in the game. This could occur when one player follows another player, or perhaps
    /// when a monster is chasing its prey.
    /// The FollowingBehavior class exposes a Target property, which points to the thing being
    /// followed. It is currently only possible to follow one target at a time.
    /// </remarks>
    public class FollowingBehavior : Behavior
    {
        private SimpleWeakReference<Thing> target;

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
            ID = instanceID;
        }

        /// <summary>Gets or sets the target being followed.</summary>
        [JsonIgnore]
        public virtual Thing Target
        {
            get
            {
                return (target == null) ? null : target.Target;
            }

            set
            {
                // Strengthen the reference.
                var localTarget = target;

                // Do nothing if the target isn't changing.
                if ((localTarget != null && localTarget.Target == value) || (localTarget == null && value == null))
                {
                    return;
                }

                if (value == null)
                {
                    // localTarget won't also be null, per above
                    RemoveTarget();
                }
                else if (localTarget == null || localTarget.Target == null)
                {
                    // value won't also be null, per above
                    AddTarget(value);
                }
                else
                {
                    // localTarget and value are different objects
                    ChangeTarget(value);
                }
            }
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to Parent.)</summary>
        protected override void OnRemoveBehavior()
        {
            // Setting Target to null ensures the MovementEvent handler is removed.
            Target = null;
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
            var self = Parent;
            var currentRoom = self.Parent;
            var arriveEvent = e as ArriveEvent;

            if (arriveEvent != null && arriveEvent.ActiveThing == Target && arriveEvent.GoingFrom == currentRoom)
            {
                var leaveMessage = CreateLeaveMessage(self);
                var arriveMessage = CreateArriveMessage(self);

                var movableBehavior = self.FindBehavior<MovableBehavior>();
                movableBehavior.Move(arriveEvent.GoingTo, arriveEvent.GoingVia, leaveMessage, arriveMessage);
            }
        }

        /// <summary>Adds the new target.</summary>
        /// <remarks>Precondition: target is null, newTarget is non-null.</remarks>
        /// <param name="newTarget">The new target.</param>
        private void AddTarget(Thing newTarget)
        {
            var self = Parent;
            if (self != null)
            {
                var message = CreateFollowMessage(self, newTarget);
                var followEvent = new FollowEvent(self, message, self, newTarget);

                self.Eventing.OnMovementRequest(followEvent, EventScope.ParentsDown);

                if (!followEvent.IsCanceled)
                {
                    target = new SimpleWeakReference<Thing>(newTarget);
                    newTarget.Eventing.MovementEvent += ProcessMovementEvent;
                    self.Eventing.OnMovementEvent(followEvent, EventScope.ParentsDown);
                }
            }
        }

        /// <summary>
        /// Changes the target being followed.
        /// Simply removes the existing target and adds the specified new target using existing
        /// methods. This will emit messages for both activities.
        /// Precondition: target and newTarget are both non-null.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        private void ChangeTarget(Thing newTarget)
        {
            RemoveTarget();
            AddTarget(newTarget);
        }

        /// <summary>Stops following the current target.</summary>
        /// <remarks>Precondition: target is non-null.</remarks>
        private void RemoveTarget()
        {
            var self = Parent;
            if (self != null)
            {
                var oldTarget = target.Target;

                if (oldTarget != null)
                {
                    // Stop tracking the old target's movements
                    target.Target.Eventing.MovementEvent -= ProcessMovementEvent;

                    // Create an event with the appropriate "unfollow" sensory messages
                    var message = CreateUnfollowMessage(Parent, oldTarget);
                    var followEvent = new FollowEvent(Parent, message, Parent, oldTarget);

                    Parent.Eventing.OnMovementRequest(followEvent, EventScope.ParentsDown);

                    if (!followEvent.IsCanceled)
                    {
                        // Finally make the change
                        target.Target = null;

                        // Broadcast the change
                        Parent.Eventing.OnMovementEvent(followEvent, EventScope.ParentsDown);
                    }
                }
            }
        }

        private SensoryMessage CreateFollowMessage(Thing self, Thing newTarget)
        {
            var message = new ContextualString(self, newTarget)
            {
                ToOriginator = $"You start following {newTarget.Name}.",
                ToReceiver = $"{self.Name} starts following you.",
                ToOthers = $"{self.Name} starts following {newTarget.Name}.",
            };

            return new SensoryMessage(SensoryType.Sight, 100, message);
        }

        private SensoryMessage CreateUnfollowMessage(Thing self, Thing oldTarget)
        {
            var message = new ContextualString(self, oldTarget)
            {
                ToOriginator = $"You stop following {oldTarget.Name}.",
                ToReceiver = $"{self.Name} stops following you.",
            };

            return new SensoryMessage(SensoryType.Sight, 100, message);
        }

        private SensoryMessage CreateArriveMessage(Thing self)
        {
            var message = new ContextualString(self, Target)
            {
                ToReceiver = $"{self.Name} arrives, following you.",
                ToOthers = $"{self.Name} arrives, following {Target.Name}.",
            };

            return new SensoryMessage(SensoryType.Sight, 100, message);
        }

        private SensoryMessage CreateLeaveMessage(Thing self)
        {
            var message = new ContextualString(self, Target)
            {
                ToOriginator = $"You follow {Target.Name}.",
                ToOthers = $"{self.Name} leaves, following {Target.Name}."
            };

            return new SensoryMessage(SensoryType.Sight, 100, message);
        }
    }
}