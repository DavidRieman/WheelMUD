//-----------------------------------------------------------------------------
// <copyright file="Effect.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A base class that represents time-sensitive Behaviors, known here as Effects.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Effects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;
    using WheelMUD.Core;

    /// <summary>Abstract base class for all effects.</summary>
    public abstract class Effect : Behavior
    {
        /// <summary>Initializes a new instance of the <see cref="Effect" /> class.</summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="duration">The duration.</param>
        public Effect(DateTime startTime, TimeSpan duration)
            : base(null)
        {
            this.StartTime = startTime;
            this.Duration = duration;
        }

        /// <summary>Initializes a new instance of the <see cref="Effect" /> class.</summary>
        /// <param name="duration">The duration.</param>
        public Effect(TimeSpan duration)
            : base(null)
        {
            this.StartTime = DateTime.Now;
            this.Duration = duration;
        }

        /// <summary>Initializes a new instance of the Effect class.</summary>
        /// <param name="instanceProperties">Dictionary of properties to spawn this effect instance with, if any.</param>
        protected Effect(Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.StartTime = DateTime.Now;
        }

        /// <summary>Gets or sets the effect duration.</summary>
        /// <remarks>
        /// Time starts counting down as soon as this property is set, unless the property
        /// is set to TimeSpan.MaxValue, in which case the effect will remain indefinitely.
        /// </remarks>
        public TimeSpan Duration { get; set; }

        /// <summary>Gets the remaining effect duration.</summary>
        public TimeSpan RemainingDuration
        {
            get { return this.Duration.Subtract(DateTime.Now - this.StartTime); }
        }

        /// <summary>Gets the time when the effect was applied.</summary>
        /// <value>The start time.</value>
        public DateTime StartTime { get; private set; }

        /// <summary>Gets the time when the effect will expire.</summary>
        /// <value>The time when the effect will expire.</value>
        public DateTime EndTime
        {
            get
            {
                return DateTime.Now + this.RemainingDuration;
            }
        }

        private void DisableEffect()
        {
            if (this.Parent == null)
            {
                return;
            }

            var behaviorManager = this.Parent.Behaviors;

            var effects = behaviorManager.OfType<Effect>();
            if (effects.Count == 0)
            {
                return;
            }

            var appliedEffects = effects.Where(effect => effect.ID == this.ID);
            foreach (var appliedEffect in appliedEffects)
            {
                // TODO: Are these the only references to the effect?
                behaviorManager.Remove(appliedEffect);
                ////appliedEffect.OnRemoveBehavior();
            }
        }
    }
}
