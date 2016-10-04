//-----------------------------------------------------------------------------
// <copyright file="KnockEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A knock event.</summary>
    public class KnockEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="KnockEvent"/> class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sensory message.</param>
        public KnockEvent(Thing activeThing, SensoryMessage senseMessage)
            : base(activeThing, senseMessage)
        {
        }
    }
}