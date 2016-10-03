//-----------------------------------------------------------------------------
// <copyright file="AttackEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>An attack event.</summary>
    public class AttackEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the AttackEvent class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sensory message.</param>
        /// <param name="aggressor">The aggressor of the attack.</param>
        public AttackEvent(Thing activeThing, SensoryMessage senseMessage, Thing aggressor)
            : base(activeThing, senseMessage)
        {
            this.Aggressor = aggressor;
            senseMessage.Context.Add("Aggressor", aggressor);
        }

        /// <summary>Gets the aggressor of the attack.</summary>
        public Thing Aggressor { get; private set; }
    }
}