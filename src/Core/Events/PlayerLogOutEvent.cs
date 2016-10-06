//-----------------------------------------------------------------------------
// <copyright file="PlayerLogOutEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A 'player logged out' event.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A 'player logged out' event.</summary>
    public class PlayerLogOutEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the PlayerLogOutEvent class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sensory message.</param>
        public PlayerLogOutEvent(Thing activeThing, SensoryMessage senseMessage)
            : base(activeThing, senseMessage)
        {
        }
    }
}