//-----------------------------------------------------------------------------
// <copyright file="LiquidSourceBehavior.cs" company="WheelMUD Development Team">
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

    /// <summary>LiquidSourceBehavior adds the ability to act like a liquid source (River, fountain, faucet, etc).</summary>
    public class LiquidSourceBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the LiquidSourceBehavior class.</summary>
        public LiquidSourceBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the LiquidSourceBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public LiquidSourceBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets or sets the ID of the liquid that this produces.</summary>
        public string LiquidName { get; set; }

        /// <summary>Gets or sets a value indicating whether liquid is flowing.</summary>
        public bool IsFlowing { get; set; }

        /// <summary>Gets or sets the sound that the liquid makes when flowing.</summary>
        public string FlowingSound { get; set; }

        /// <summary>Creates a new item of the type of liquid defined in this behavior.</summary>
        /// <param name="quantity">The amount of the liquid to create.</param>
        /// <param name="units">Units for the quantity.</param>
        /// <returns>A new Liquid item.</returns>
        public Thing GenerateLiquid(double quantity, string units)
        {
            // @@@ TODO: Remove LiquidSourceBehavior and implement appropriate Spawners and
            //     maybe an item template for a Thing with ContainerBehavior and an appropriate
            //     Spawner to fill up to a maximum amount of water, periodically.
            return null;
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.LiquidName = "water";
            this.IsFlowing = true;
            this.FlowingSound = "bubbling";
        }
    }
}