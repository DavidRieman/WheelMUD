//-----------------------------------------------------------------------------
// <copyright file="PoisonEffect.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Shabubu
//   Date      : 5/8/2009 8:28:07 PM
//   Purpose   : Type purpose here.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Effects
{
    using System;
    using System.Collections.Generic;

    /// <summary>PoisonEffect: Poisons a person.</summary>
    public class PoisonEffect : Effect
    {
        /// <summary>Initializes a new instance of the PoisonEffect class.</summary>
        public PoisonEffect()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the PoisonEffect class.</summary>
        /// <param name="instanceID">ID of the effect instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this effect instance.</param>
        public PoisonEffect(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /* @@@ TODO: Rework against new Effect class and time/tick events.
        /// <summary>@@Temporary code for now...this will change.</summary>
        private int numOfTicks;

        /// <summary>@@Temporary code for now...this will change.</summary>
        private TimeSpan timeBetweenTicks = new TimeSpan(0, 0, 1);

        /// <summary>@@Temporary code for now...this will change.</summary>
        private TimeSpan totalTime;

        /// <summary>@@Temporary code for now...this will change.</summary>
        private IStat health = null;

        /// <summary>@@Temporary code for now...this will change.</summary>
        /// <param name="duration">duration 12</param>
        public override void Apply(TimeSpan duration)
        {
            if (this.totalTime == new TimeSpan(0, 0, 0))
            {
                this.totalTime = duration;
            }

            this.totalTime = this.totalTime - this.timeBetweenTicks;
            this.health = this.Host.Stats["health"];

            if (this.numOfTicks != 0)
            {
                this.health.Decrease(10, this.Host);
                this.numOfTicks -= 1;
                base.Apply(this.timeBetweenTicks);
            }
            else
            {
                this.Remove();
            }
        }
        */

        /// <summary>Sets the default properties of this effect instance.</summary>
        protected override void SetDefaultProperties()
        {
            throw new NotImplementedException();
        }
    }
}