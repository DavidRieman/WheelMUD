//-----------------------------------------------------------------------------
// <copyright file="FlammableBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: November 2009 by bengecko.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>FlammableBehavior adds the ability to an object to burn.</summary>
    public class FlammableBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the FlammableBehavior class.</summary>
        public FlammableBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the FlammableBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public FlammableBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets or sets a value indicating whether the item is consumed when ignited.</summary>
        public bool IsConsumed { get; set; }

        /// <summary>Gets or sets the damage formula for the fire from the item.</summary>
        public string DamageFormula { get; set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.IsConsumed = true;
            this.DamageFormula = null;
        }
    }
}