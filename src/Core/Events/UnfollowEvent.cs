// <copyright file="UnfollowEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Event associated with the "unfollow" command
//   Created: March 23, 2012 by James McManus.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>Event to convey that someone has stopped following someone else.</summary>
    public class UnfollowEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="UnfollowEvent"/> class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sense message.</param>
        /// <param name="follower">The follower.</param>
        /// <param name="target">The target.</param>
        public UnfollowEvent(Thing activeThing, SensoryMessage senseMessage, Thing follower, Thing target)
            : base(activeThing, senseMessage)
        {
            this.Follower = follower;
            this.Target = target;
            senseMessage.Context.Add("Follower", this.Follower);
            senseMessage.Context.Add("Target", this.Target);
        }

        /// <summary>Gets the Thing that was doing the following.</summary>
        public Thing Follower { get; private set; }

        /// <summary>Gets the Thing that was being followed.</summary>
        public Thing Target { get; private set; }
    }
}
