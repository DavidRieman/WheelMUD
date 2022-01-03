//-----------------------------------------------------------------------------
// <copyright file="WRMAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;

namespace WarriorRogueMage
{
    /// <summary>The attribute that represents the Warrior, Rogue, and Mage player roles.</summary>
    public abstract class WRMAttribute : GameAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="WRMAttribute"/> class.</summary>
        /// <param name="name">The name of this attribute.</param>
        /// <param name="abbreviation">The abbreviation for this attribute.</param>
        /// <param name="formula">The formula that this attribute will use, if any.</param>
        /// <param name="startValue">The start value for this attribute.</param>
        /// <param name="minValue">The minimum value for this attribute.</param>
        /// <param name="maxValue">The maximum value for this attribute.</param>
        public WRMAttribute(string name, string abbreviation, string formula, int startValue, int minValue, int maxValue)
            : base(null, name, abbreviation, formula, startValue, minValue, maxValue, true)
        {
        }
    }
}