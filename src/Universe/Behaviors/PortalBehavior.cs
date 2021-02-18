//-----------------------------------------------------------------------------
// <copyright file="PortalBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Core.Events;

namespace WheelMUD.Universe
{
    /// <summary>A portal item behavior adds the ability to enter an item to arrive at a new location.</summary>
    public class PortalBehavior : Behavior
    {
        /// <summary>Potentially cache a reference to the room this portal leads to.</summary>
        ////private Thing exitLocation = null;

        /// <summary>Initializes a new instance of the PortalBehavior class.</summary>
        public PortalBehavior()
            : base(null)
        {
            Parent.Eventing.MovementRequest += Parent_MovementRequest;
        }

        /// <summary>Initializes a new instance of the PortalBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public PortalBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceID;
        }

        /// <summary>Gets or sets the destination room ID for this portal.</summary>
        public string DestinationThingID { get; set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            DestinationThingID = null;
        }

        /// <summary>Handle movement requests from the parent Thing.</summary>
        /// <param name="root">The root Thing where this event broadcast started.</param>
        /// <param name="e">The cancellable event/request arguments.</param>
        private void Parent_MovementRequest(Thing root, CancellableGameEvent e)
        {
            // TODO: When our parent Thing gets an Arrive request (such as when a thing is attempting to enter an
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
            if (exitLocation == null || exitLocation.Id != DestinationThingID)
            {
                // TODO Repair: exitLocation = world.FindThing(DestinationThingID);
            }

            // Send a sensory event for entering the portal.
            var leaveMessage = new ContextualString(enteringEntity, enteringEntity)
            {
                ToOriginator = $"You step into {Parent.Name}.",
                ToReceiver = $"{actor.Name} steps into you.",
                ToOthers = $"{actor.Name} steps into {Parent.Name}.",
            };
            Thing parent = enteringEntity.Parent;
            SensoryMessage enterMessage = new SensoryMessage(SensoryType.Sight, 100, leaveMessage);
            SensoryEvent enterEvent = new SensoryEvent(enteringEntity, parent, enterMessage);
            parent.EventBroadcaster.Broadcast(enterEvent);

            // Move the specified entity to the destination room.
            bool moved = enteringEntity.Move(exitLocation);

            if (moved)
            {
                // If entity moved to the other side, send a sensory event to depict the arrival.
                var arriveMessage = new ContextualString(enteringEntity, enteringEntity)
                {
                    ToOriginator = $"You step out of {Parent.Name}, into {exitLocation.Name}.",
                    ToReceiver = $"{actor.Name} steps out of you.",
                    ToOthers = $"{actor.Name} steps out of {Parent.Name}.",
                }
                SensoryMessage exitMessage = new SensoryMessage(SensoryType.Sight, 100, arriveMessage);
                SensoryEvent exitEvent = new SensoryEvent(enteringEntity, exitLocation, exitMessage);
                exitLocation.EventBroadcaster.Broadcast(exitEvent);
            }
            else
            {
                // If entity failed to emerge at the other side, send a sensory event to describe the failure.
                var failedMessage = new ContextualString(enteringEntity, enteringEntity)
                {
                    ToOriginator = $"{Parent.Name} seems to be inactive.",
                    ToReceiver = $"{actor.Name} tried to move through you, but couldn't.",
                    ToOthers = $"{actor.Name} could not pass through {Parent.Name}.",
                }
                SensoryMessage cancelMessage = new SensoryMessage(SensoryType.Sight, 100, failedMessage);
                SensoryEvent cancelEvent = new SensoryEvent(enteringEntity, parent, cancelMessage);
                parent.EventBroadcaster.Broadcast(cancelEvent);
            }
        }*/
    }
}