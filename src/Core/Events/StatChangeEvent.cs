//-----------------------------------------------------------------------------
// <copyright file="StatChangeEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A stat change event.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A stat change event.</summary>
    public class StatChangeEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the StatChangeEvent class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sensory message.</param>
        /// <param name="stat">The applicable stat.</param>
        /// <param name="modifier">The stat modifier.</param>
        /// <param name="oldValue">The old stat value.</param>
        public StatChangeEvent(Thing activeThing, SensoryMessage senseMessage, BaseStat stat, int modifier, int oldValue)
            : base(activeThing, senseMessage)
        {
            this.Stat = stat;
            this.OldValue = oldValue;
            this.Modifier = modifier;
        }

        /// <summary>Gets the applicable stat.</summary>
        public BaseStat Stat { get; private set; }

        /// <summary>Gets the stat modifier.</summary>
        public int Modifier { get; private set; }

        /// <summary>Gets the stat's old value.</summary>
        public int OldValue { get; private set; }
    }
}