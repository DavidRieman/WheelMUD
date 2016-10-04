//-----------------------------------------------------------------------------
// <copyright file="SensoryEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A sensory event.</summary>
    public class SensoryEvent : GameEvent
    {
        /// <summary>Initializes a new instance of the SensoryEvent class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sensory message.</param>
        public SensoryEvent(Thing activeThing, SensoryMessage senseMessage)
            : base(activeThing, senseMessage)
        {
        }
    }
}