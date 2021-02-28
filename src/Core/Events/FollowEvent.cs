//-----------------------------------------------------------------------------
// <copyright file="FollowEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>Event to convey that someone has begun following someone else.</summary>
    public class FollowEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="FollowEvent"/> class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sense message.</param>
        /// <param name="follower">The follower.</param>
        /// <param name="target">The target.</param>
        public FollowEvent(Thing activeThing, SensoryMessage senseMessage, Thing follower, Thing target)
            : base(activeThing, senseMessage)
        {
            Follower = follower;
            Target = target;
            senseMessage.Context.Add("Follower", Follower);
            senseMessage.Context.Add("Target", Target);
        }

        /// <summary>Gets the Thing doing the following.</summary>
        public Thing Follower { get; private set; }

        /// <summary>Gets the Thing being followed.</summary>
        public Thing Target { get; private set; }
    }
}