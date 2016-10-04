//-----------------------------------------------------------------------------
// <copyright file="ArmorBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An armor item behavior adds the ability to wear an item and gain bonuses from it.
//   Updated: June 2009 by Karak: implements IItemBehavior; ID, Clone
//   Updated: November 2009 by bengecko - AbstractItemBehavior
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>Armor behavior adds the ability to wear this Thing as armor and gain some continuous statistical adjustment from it.</summary>
    /// <remarks>@@@ TODO: Add an interface for all behaviors which make statistical adjustments, to assist with recalculations?</remarks>
    public class ArmorBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the ArmorBehavior class.</summary>
        public ArmorBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the ArmorBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public ArmorBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Use the specified armor.</summary>
        /// <param name="armorRequest">The request details for using the armor.</param>
        /// <returns>True if successfully donned, else false.</returns>
        public virtual bool Use(UseArmorRequest armorRequest)
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