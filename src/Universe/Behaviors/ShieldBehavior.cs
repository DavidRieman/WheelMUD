//-----------------------------------------------------------------------------
// <copyright file="ShieldBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A shield item behavior adds the ability to wear an item and otherwise be used like a shield.
//   Updated: June 2009 by Karak: implements IItemBehavior; ID, Clone
//   Updated: November 2009 by bengecko - AbstractItemBehavior
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>A shield item behavior adds the ability to wear an item and otherwise be used like a shield.</summary>
    public class ShieldBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the ShieldBehavior class.</summary>
        public ShieldBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the ShieldBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public ShieldBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Use the specified shield.</summary>
        /// <param name="shieldRequest">The request details for wearing the shield.</param>
        /// <returns>True if the shield is put on, else false.</returns>
        public virtual bool Use(UseShieldRequest shieldRequest)
        {
            return false;
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            throw new NotImplementedException();
        }
    }
}