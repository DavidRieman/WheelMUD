//-----------------------------------------------------------------------------
// <copyright file="GameStat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   A game stat.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using WheelMUD.Interfaces;

    /// <summary>Represents a game-wide stat.</summary>
    /// <remarks>This is used as a template to let the system know what are the stats for the MUD.</remarks>
    public class GameStat : BaseStat, IPersistsWithPlayer
    {
        /// <summary>Initializes a new instance of the <see cref="GameStat"/> class.</summary>
        /// <param name="controller">The controller.</param>
        /// <param name="name">The stat name.</param>
        /// <param name="abbreviation">The ID that will be used to allow the gaming system to recognize this stat.</param>
        /// <param name="formula">The formula that may be needed to calculate the value of this stat.</param>
        /// <param name="value">The stat value.</param>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="visible">If set to <c>true</c> make this stat [visible].</param>
        public GameStat(IController controller, string name, string abbreviation, string formula, int value, int minValue, int maxValue, bool visible)
            : base(controller, name, abbreviation, formula, value, minValue, maxValue, visible)
        {
        }
    }
}
