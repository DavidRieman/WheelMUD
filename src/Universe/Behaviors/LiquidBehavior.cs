//-----------------------------------------------------------------------------
// <copyright file="LiquidBehavior.cs" company="WheelMUD Development Team">
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

    /// <summary>LiquidBehavior adds the ability to act like a liquid.</summary>
    public class LiquidBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the LiquidBehavior class.</summary>
        public LiquidBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the LiquidBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public LiquidBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets or sets the viscosity of the liquid.</summary>
        public string Viscosity { get; set; }

        /// <summary>Gets or sets the units of measurement for the quanity of the liquid.</summary>
        public string UnitsOfMeasurement { get; set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.Viscosity = "medium";
            this.UnitsOfMeasurement = "pint";
        }
    }
}