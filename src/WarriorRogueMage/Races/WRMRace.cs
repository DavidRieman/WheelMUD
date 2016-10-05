//-----------------------------------------------------------------------------
// <copyright file="WRMRace.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;

    /// <summary>
    /// WRM-specific race class; implements race-related details specific to the WRM game system.
    /// Provides a base class for individual WRM races (which may have their own specific event handlers and such).
    /// </summary>
    public class WRMRace : GameRace
    {
        public WRMRace(string name, string description, params Talent[] racialTalents) : base()
        {
            this.Name = name;
            this.Description = description;
            this.RacialTalents = racialTalents.ToList();
        }

        public List<Talent> RacialTalents { get; private set; }
    }
}