//-----------------------------------------------------------------------------
// <copyright file="GameRace.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Interfaces;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>Describes the most basic characteristics for a race in the current gaming system.</summary>
    public class GameRace : IPersistsWithPlayer, INamed
    {
        /// <summary>Gets or sets the name of the race.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the racial description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the racial stats.</summary>
        public List<GameStat> RacialStats { get; set; }

        /// <summary>Gets or sets the racial attributes.</summary>
        public List<GameAttribute> RacialAttributes { get; set; }

        /// <summary>Gets or sets the racial skills.</summary>
        public List<GameSkill> RacialSkills { get; set; }

        /// <summary>Gets or sets the racial modifiers.</summary>
        public List<GameModifier> RacialModifiers { get; set; }
    }
}