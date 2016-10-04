//-----------------------------------------------------------------------------
// <copyright file="ContainerBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: June 2009 by Karak.
//   Updated: November 2009 by bengecko - AbstractItemBehavior
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>A container behavior adds the ability to contain other things.</summary>
    public class ContainerBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the <see cref="ContainerBehavior"/> class.</summary>
        public ContainerBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ContainerBehavior"/> class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public ContainerBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets or sets a value indicating whether the container is able to hold liquid without leaking.</summary>
        public bool HoldsLiquid { get; set; }

        /// <summary>Gets or sets the amount of space in the container.</summary>
        public int Volume { get; set; }

        /// <summary>Gets or sets the units of measurement for the volume.</summary>
        public string VolumeUnitOfMeasurement { get; set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.HoldsLiquid = false;
            this.Volume = 0;
            this.VolumeUnitOfMeasurement = null;
        }
    }
}