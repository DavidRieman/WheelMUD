//-----------------------------------------------------------------------------
// <copyright file="WRMStat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;

namespace WarriorRogueMage
{
    /// <summary>A "Warrior, Rogue, and Mage" statistic.</summary>
    public abstract class WRMStat : GameStat
    {
        /// <summary>Initializes a new instance of the <see cref="WRMStat" /> class.</summary>
        public WRMStat(string name, string abbreviation, string formula, int value, int minValue, int maxValue)
            : base(null, name, abbreviation, formula, value, minValue, maxValue, true)
        {
        }
    }
}