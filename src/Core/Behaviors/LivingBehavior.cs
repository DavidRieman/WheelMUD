﻿//-----------------------------------------------------------------------------
// <copyright file="LivingBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace WheelMUD.Core
{
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
            ID = instanceID;
        }

        /// <summary>Gets or sets the consciousness of the entity.</summary>
        public virtual Consciousness Consciousness { get; set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            Consciousness = Consciousness.Awake;
        }
    }
}