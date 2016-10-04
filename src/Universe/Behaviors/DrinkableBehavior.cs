//-----------------------------------------------------------------------------
// <copyright file="DrinkableBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: May 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>This behavior allows the attached thing to be drinkable.</summary>
    public class DrinkableBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the DrinkableBehavior class.</summary>
        public DrinkableBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the DrinkableBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public DrinkableBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Have a thing try to drink from this drinkable thing.</summary>
        /// <param name="drinker">The thing trying to take a drink.</param>
        public void Drink(Thing drinker)
        {
            // @@@ Implement - destroy parent thing (reduce count) by an appropriate amount (1 for now?)
            // @@@ Also, check for parent item has a drink reaction (IE behavior for implementing potion-
            //     like effects upon the drinking of it), and implement said reactions.
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}