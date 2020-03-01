//-----------------------------------------------------------------------------
// <copyright file="PortalBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Events;

    /// <summary>A portal item behavior adds the ability to enter an item to arrive at a new location.</summary>
    public class PortalBehavior : Behavior
    {
        /// <summary>Potentially cache a reference to the room this portal leads to.</summary>
        ////private Thing exitLocation = null;

        /// <summary>Initializes a new instance of the PortalBehavior class.</summary>
        public PortalBehavior()
            : base(null)
        {
            this.Parent.Eventing.MovementRequest += this.Parent_MovementRequest;
        }

        /// <summary>Initializes a new instance of the PortalBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public PortalBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets or sets the destination room ID for this portal.</summary>
        public string DestinationThingID { get; set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.DestinationThingID = null;
        }

        /// <summary>Handle movement requests from the parent Thing.</summary>
        /// <param name="root">The root Thing where this event broadcast started.</param>
        /// <param name="e">The cancellable event/request arguments.</param>
        private void Parent_MovementRequest(Thing root, CancellableGameEvent e)
        {
            // @@@ TODO: When our parent Thing gets an Arrive request (such as when a thing is attempting to enter an
            // enterable portal), we want to cancel the event and replace it with our own movement request to enter the
            // portal's target location.
        }

        /*
        /// <summary>Have the specified entity try to use the portal.</summary>
        /// <param name="enteringEntity">The entity to enter the portal.</param>
        /// <param name="world">The world with which we'll try to associate the destination room ID.</param>
        public void Use(Entity enteringEntity, Thing world)
        {
            // If the current exit isn't rigged up to the current destination, rig it up.
            if (this.exitLocation == null || this.exitLocation.Id != this.DestinationThingID)
            {
//@@@ Repair: this.exitLocation = world.FindThing(this.DestinationThingID);
            }

            // Send a sensory event for entering the portal.
            var leaveMessage = new ContextualString(enteringEntity, enteringEntity)
            {
                ToOriginator = $"You step into {this.Parent.Name}.",
                ToReceiver = $"{actor.Name} steps into you.",
                ToOthers = $"{actor.Name} steps into {this.Parent.Name}.",
            };
            Thing parent = enteringEntity.Parent;
            SensoryMessage enterMessage = new SensoryMessage(SensoryType.Sight, 100, leaveMessage);
            SensoryEvent enterEvent = new SensoryEvent(enteringEntity, parent, enterMessage);
            parent.EventBroadcaster.Broadcast(enterEvent);

            // Move the specified entity to the destination room.
            bool moved = enteringEntity.Move(this.exitLocation);

            if (moved)
            {
                // If entity moved to the other side, send a sensory event to depict the arrival.
                var arriveMessage = new ContextualString(enteringEntity, enteringEntity)
                {
                    ToOriginator = $"You step out of {this.Parent.Name}, into {this.exitLocation.Name}.",
                    ToReceiver = $"{actor.Name} steps out of you.",
                    ToOthers = $"{actor.Name} steps out of {this.Parent.Name}.",
                }
                SensoryMessage exitMessage = new SensoryMessage(SensoryType.Sight, 100, arriveMessage);
                SensoryEvent exitEvent = new SensoryEvent(enteringEntity, this.exitLocation, exitMessage);
                this.exitLocation.EventBroadcaster.Broadcast(exitEvent);
            }
            else
            {
                // If entity failed to emerge at the other side, send a sensory event to describe the failure.
                var failedMessage = new ContextualString(enteringEntity, enteringEntity)
                {
                    ToOriginator = $"{this.Parent.Name} seems to be inactive.",
                    ToReceiver = $"{actor.Name} tried to move through you, but couldn't.",
                    ToOthers = $"{actor.Name} could not pass through {this.Parent.Name}.",
                }
                SensoryMessage cancelMessage = new SensoryMessage(SensoryType.Sight, 100, failedMessage);
                SensoryEvent cancelEvent = new SensoryEvent(enteringEntity, parent, cancelMessage);
                parent.EventBroadcaster.Broadcast(cancelEvent);
            }
        }*/
    }
}