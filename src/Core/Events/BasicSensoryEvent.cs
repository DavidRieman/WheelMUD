//-----------------------------------------------------------------------------
// <copyright file="BasicSensoryEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>A basic sensory event. Designed to be observed by senses of an entity.</summary>
    public class BasicSensoryEvent : GameEvent
    {
        /// <summary>Initializes a new instance of the BasicSensoryEvent class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sensory message.</param>
        public BasicSensoryEvent(Thing activeThing, SensoryMessage senseMessage)
            : base(activeThing, senseMessage)
        {
        }
    }
}