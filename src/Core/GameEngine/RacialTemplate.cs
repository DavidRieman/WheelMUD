//-----------------------------------------------------------------------------
// <copyright file="RacialTemplate.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Class that will hold a game racial template.
//   Author: Fastalanasa
//   Date: May 10, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using WheelMUD.Interfaces;

    /// <summary>
    /// Describes the most basic characteristics for a race in the
    /// current gaming system.
    /// </summary>
    public class GameRace : IPersistsWithPlayer
    {
        /// <summary>
        /// Gets or sets the name of the race.
        /// </summary>
        /// <value>
        /// The name of the race.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the racial description.
        /// </summary>
        /// <value>
        /// The description for this particular race.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the racial stats.
        /// </summary>
        /// <value>
        /// The racial stats.
        /// </value>
        public List<GameStat> RacialStats { get; set; }

        /// <summary>
        /// Gets or sets the racial custom tables.
        /// This is where anything that doesn't have an
        /// obvious place goes.
        /// </summary>
        /// <value>
        /// The racial custom tables.
        /// </value>
        public Dictionary<string, GameTable> RacialCustomTables { get; set; }

        /// <summary>
        /// Gets or sets the racial attributes.
        /// </summary>
        /// <value>
        /// The racial attributes.
        /// </value>
        public List<GameAttribute> RacialAttributes { get; set; }

        /// <summary>
        /// Gets or sets the racial skills.
        /// </summary>
        /// <value>
        /// The racial skills.
        /// </value>
        public List<GameSkill> RacialSkills { get; set; }

        /// <summary>
        /// Gets or sets the racial modifiers.
        /// </summary>
        /// <value>
        /// The racial modifiers.
        /// </value>
        public List<GameModifier> RacialModifiers { get; set; }
    }
}