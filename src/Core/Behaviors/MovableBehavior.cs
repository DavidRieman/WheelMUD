//-----------------------------------------------------------------------------
// <copyright file="MovableBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: May 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using WheelMUD.Core.Events;

    /// <summary>Behavior applied to objects that can move or be moved.</summary>
    public class MovableBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the MovableBehavior class.</summary>
        public MovableBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the MovableBehavior class.</summary>
        /// <param name="instanceId">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public MovableBehavior(long instanceId, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceId;
        }

        // @@@ OpensClosesBehavior needs to listen for movement events and set e.Cancel if the transition object
        //     is closed at the time.

        /// <summary>Move the entity to the specified destination.</summary>
        /// <param name="destination">
        /// @@@ TODO: The destination to move the entity to; if the destination has an ExitBehavior then this Thing is
        /// automatically moved to the other destination of the exit (IE an adjacent room, portal destination,
        /// or inside/outside of a vehicle, et cetera).
        /// </param>
        /// <param name="goingVia">The thing we are travelling via (IE an Exit, an Enterable thing, etc.)</param>
        /// <param name="leavingMessage">A sensory message describing this sort of 'leaving' movement.</param>
        /// <param name="arrivingMessage">A sensory message describing this sort of 'arriving' movement.</param>
        /// <returns>True if the entity was successfully moved, else false.</returns>
        public bool Move(Thing destination, Thing goingVia, SensoryMessage leavingMessage, SensoryMessage arrivingMessage)
        {
            Thing actor = this.Parent;
            Thing goingFrom = actor.Parent;

            // Prepare events to request and send (if not cancelled).
            var leaveEvent = new LeaveEvent(actor, goingFrom, destination, goingVia, leavingMessage);
            var arriveEvent = new ArriveEvent(actor, goingFrom, destination, goingVia, arrivingMessage);

            // Broadcast the Leave Request first to see if the player is allowed to leave.
            actor.Eventing.OnMovementRequest(leaveEvent, EventScope.ParentsDown);
            if (!leaveEvent.IsCancelled)
            {
                // Next see if the player is allowed to Arrive at the new location.
                destination.Eventing.OnMovementRequest(arriveEvent, EventScope.SelfDown);
                if (!arriveEvent.IsCancelled)
                {
                    actor.Eventing.OnMovementEvent(leaveEvent, EventScope.ParentsDown);
                    actor.RemoveFromParents();
                    destination.Add(actor);

                    // @@@ TODO: Ensure these automatically enqueue a save.
                    destination.Eventing.OnMovementEvent(arriveEvent, EventScope.SelfDown);
                    return true;
                }
            }

            return false;
        }

        /*
        /// <summary>Move the entity in the specified direction.</summary>
        /// <param name="direction">The direction to move the entity in.</param>
        /// <returns>True if the entity sucessfully moved, else false.</returns>
        public virtual bool MoveInDirection(string direction)
        {
            // @@@ TODO: Probably should be context-sensitive commands that are rigged
            //     up by the ExitBehavior, so this shouldn't be needed?
            // @@@ TODO: Send a movement Request, then if not Cancelled, a movement Event.
            //direction = direction.ToLower();
            //Thing currentRoom = this.Parent;
            //if (currentRoom.Children.ContainsKey(direction))
            //{
            //    Exit exit = currentRoom.Exits[direction];
            //    return exit.Use(this);
            //}
            return false;
        }
        */

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}