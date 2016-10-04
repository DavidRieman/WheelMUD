//-----------------------------------------------------------------------------
// <copyright file="MoveEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A movement event.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A bulk movement event.</summary>
    public class BulkMovementEvent : GameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="BulkMovementEvent"/> class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="sensoryMessage">The sensory message.</param>
        public BulkMovementEvent(Thing activeThing, SensoryMessage sensoryMessage)
            : base(activeThing, sensoryMessage)
        {
        }
    }

    /// <summary>Event associated with movement from one place to another.</summary>
    public class MovementEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="MovementEvent"/> class.</summary>
        /// <param name="thingMoving">The thing moving.</param>
        /// <param name="goingFrom">The original location of the moving thing.</param>
        /// <param name="goingTo">The location where the thing is going.</param>
        /// <param name="goingVia">The thing (typically an exit) through which the movement occurs.</param>
        /// <param name="sensoryMessage">The sensory message describing the movement.</param>
        public MovementEvent(Thing thingMoving, Thing goingFrom, Thing goingTo, Thing goingVia, SensoryMessage sensoryMessage)
            : base(thingMoving, sensoryMessage)
        {
            this.GoingFrom = goingFrom;
            this.GoingTo = goingTo;
            this.GoingVia = goingVia;
            if (sensoryMessage != null)
            {
                sensoryMessage.Context.Add("GoingFrom", this.GoingFrom);
                sensoryMessage.Context.Add("GoingTo", this.GoingTo);
                sensoryMessage.Context.Add("GoingVia", this.GoingVia);
            }
        }

        /// <summary>Gets the original location of the moving thing.</summary>
        /// <value>The going from.</value>
        public Thing GoingFrom { get; private set; }

        /// <summary>Gets the location where the thing is going.</summary>
        /// <value>The going to.</value>
        public Thing GoingTo { get; private set; }

        /// <summary>Gets the thing (typically an exit) through which the movement occurs.</summary>
        /// <value>The going via.</value>
        public Thing GoingVia { get; private set; }
    }

    /// <summary>A movment event specifically associated with leaving a location.</summary>
    public class LeaveEvent : MovementEvent
    {
        /// <summary>Initializes a new instance of the <see cref="LeaveEvent"/> class.</summary>
        /// <param name="thingMoving">The thing moving.</param>
        /// <param name="goingFrom">The original location of the moving thing.</param>
        /// <param name="goingTo">The location where the thing is going.</param>
        /// <param name="goingVia">The thing (typically an exit) through which the movement occurs.</param>
        /// <param name="sensoryMessage">The sensory message describing the movement.</param>
        public LeaveEvent(Thing thingMoving, Thing goingFrom, Thing goingTo, Thing goingVia, SensoryMessage sensoryMessage)
            : base(thingMoving, goingFrom, goingTo, goingVia, sensoryMessage)
        {
        }
    }

    /// <summary>A movment event specifically associated with arriving at a location.</summary>
    public class ArriveEvent : MovementEvent
    {
        /// <summary>Initializes a new instance of the <see cref="ArriveEvent"/> class.</summary>
        /// <param name="thingMoving">The thing moving.</param>
        /// <param name="goingFrom">The original location of the moving thing.</param>
        /// <param name="goingTo">The location where the thing is going.</param>
        /// <param name="goingVia">The thing (typically an exit) through which the movement occurs.</param>
        /// <param name="sensoryMessage">The sensory message describing the movement.</param>
        public ArriveEvent(Thing thingMoving, Thing goingFrom, Thing goingTo, Thing goingVia, SensoryMessage sensoryMessage)
            : base(thingMoving, goingFrom, goingTo, goingVia, sensoryMessage)
        {
        }
    }
}
