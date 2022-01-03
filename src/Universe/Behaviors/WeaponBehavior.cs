//-----------------------------------------------------------------------------
// <copyright file="WeaponBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Universe
{
    /// <summary>A weapon behavior adds the ability to wield and attack with the host Thing.</summary>
    /// <remarks>
    /// TODO: Add an interface for all behaviors which make statistical adjustments, to assist with recalculations?
    /// TODO: Perhaps remove concepts like 'MinimumStrength' from core behaviors, instead
    /// this could be accomplished with a GameWeaponBehavior, and/or global game system event 
    /// handlers which enforce such rules at time of the wield request's processing...
    /// </remarks>
    public class WeaponBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the WeaponBehavior class.</summary>
        public WeaponBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the WeaponBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public WeaponBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceID;
        }

        /// <summary>Gets or sets the minimum strength to use the weapon.</summary>
        public int MinimumStrength { get; set; }

        /// <summary>Gets or sets a value indicating whether the weapon requires two hands to wield.</summary>
        public bool RequiresTwoHands { get; set; }

        /// <summary>Gets or sets the damage formula for the weapon.</summary>
        public string DamageFormula { get; set; }

        /// <summary>Use the specified weapon.</summary>
        /// <param name="weaponRequest">The request details for equipping the weapon.</param>
        /// <returns>True if the weapon is wielded, else false.</returns>
        public virtual bool Use(UseWeaponRequest weaponRequest)
        {
            return false;
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            MinimumStrength = 0;
            RequiresTwoHands = false;
            DamageFormula = null;
        }
    }
}