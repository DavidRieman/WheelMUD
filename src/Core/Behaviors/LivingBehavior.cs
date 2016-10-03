//-----------------------------------------------------------------------------
// <copyright file="LivingBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A behavior defining traits specific to living beings (IE players and mobiles).
//   Created: September 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>A behavior defining traits specific to living beings (IE players and mobiles).</summary>
    public class LivingBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the LivingBehavior class.</summary>
        public LivingBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the LivingBehavior class.</summary>
        /// <param name="instanceID">The instance ID.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public LivingBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets or sets the consciousness of the entity.</summary>
        public virtual Consciousness Consciousness { get; set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.Consciousness = Consciousness.Awake;
        }
    }
}