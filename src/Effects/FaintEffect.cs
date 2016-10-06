//-----------------------------------------------------------------------------
// <copyright file="FaintEffect.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created: 5/8/2009 by Shabubu.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Effects
{
    using System;
    using System.Collections.Generic;

    /// <summary>An effect causing the thing to be unconscious.</summary>
    public class FaintEffect : Effect
    {
        /// <summary>Initializes a new instance of the FaintEffect class.</summary>
        public FaintEffect()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the FaintEffect class.</summary>
        /// <param name="instanceID">ID of the effect instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this effect instance.</param>
        public FaintEffect(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /* 
        /// <summary>Stores the entity that will faint.</summary>
        private Entity entity = null;

        /// <summary>Stores the previous consciousness of the entity.</summary>
        private Consciousness previousConsciousness = Consciousness.Dead;

        /// <summary>Applies the effect.</summary>
        /// <param name="duration">duration of the effect</param>
        public override void Apply(TimeSpan duration)
        {
            this.entity = (Entity)this.Host;
            
            this.previousConsciousness = this.entity.Consciousness;
            this.entity.Consciousness = Consciousness.Unconscious;

            base.Apply(duration);
        }

        /// <summary>This undoes the effect, returning entity to its previous state of consciousness.</summary>
        private void Restore()
        {
            if (this.entity != null)
            {
                if (this.entity.Consciousness == Consciousness.Unconscious)
                {
                    this.entity.Consciousness = this.previousConsciousness;
                }
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