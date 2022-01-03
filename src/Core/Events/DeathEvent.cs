//-----------------------------------------------------------------------------
// <copyright file="DeathEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>A death event.</summary>
    public class DeathEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the DeathEvent class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sensory message.</param>
        /// <param name="killer">The thing that caused the death.</param>
        public DeathEvent(Thing activeThing, SensoryMessage senseMessage, Thing killer)
            : base(activeThing, senseMessage)
        {
            Killer = killer;
        }

        /// <summary>Gets the killer.</summary>
        public Thing Killer { get; private set; }
    }
}