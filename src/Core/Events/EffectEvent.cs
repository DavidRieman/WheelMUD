//-----------------------------------------------------------------------------
// <copyright file="EffectEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Event associated with effects - added, removed, expired, etc.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    using System.Linq;

    /// <summary>Event associated with effects - added, removed, expired, etc.</summary>
    public class EffectEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="EffectEvent"/> class.</summary>
        /// <param name="activeThing">The thing that initiated the event.</param>
        /// <param name="sensoryMessage">The sensory message.</param>
        public EffectEvent(Thing activeThing, SensoryMessage sensoryMessage)
            : base(activeThing, sensoryMessage)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="EffectEvent"/> class.</summary>
        /// <param name="activeThing">The thing that initiated the event.</param>
        /// <param name="target">The the target of the event.</param>
        /// <param name="sensoryMessage">The sensory message.</param>
        public EffectEvent(Thing activeThing, Thing target, SensoryMessage sensoryMessage)
            : base(activeThing, sensoryMessage)
        {
            this.Target = target;
            if (sensoryMessage != null)
            {
                sensoryMessage.Context.Add("Target", this.Target);
            }
        }

        /// <summary>Gets the target of the event.</summary>
        /// <value>The target.</value>
        public Thing Target { get; private set; }
    }
}
