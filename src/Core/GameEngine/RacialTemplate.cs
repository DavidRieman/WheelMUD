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

    /// <summary>Describes the most basic characteristics for a race in the current gaming system.</summary>
    public class GameRace : IPersistsWithPlayer
    {
        /// <summary>Gets or sets the name of the race.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the racial description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the racial stats.</summary>
        public List<GameStat> RacialStats { get; set; }

        /// <summary>Gets or sets the racial custom tables.</summary>
        /// <remarks>This is where anything that doesn't have an obvious place goes.</remarks>
        public Dictionary<string, GameTable> RacialCustomTables { get; set; }

        /// <summary>Gets or sets the racial attributes.</summary>
        public List<GameAttribute> RacialAttributes { get; set; }

        /// <summary>Gets or sets the racial skills.</summary>
        public List<GameSkill> RacialSkills { get; set; }

        /// <summary>Gets or sets the racial modifiers.</summary>
        public List<GameModifier> RacialModifiers { get; set; }
    }
}