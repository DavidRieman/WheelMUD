//-----------------------------------------------------------------------------
// <copyright file="PlayerLogInEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A 'player logged in' event.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A 'player logged in' event.</summary>
    public class PlayerLogInEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="PlayerLogInEvent"/> class.</summary>
        /// <param name="activeThing">The player that has logged in.</param>
        /// <param name="senseMessage">The sensory message describing the event to those who can perceive it.</param>
        public PlayerLogInEvent(Thing activeThing, SensoryMessage senseMessage)
            : base(activeThing, senseMessage)
        {
        }
    }
}
