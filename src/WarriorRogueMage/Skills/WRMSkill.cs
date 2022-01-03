//-----------------------------------------------------------------------------
// <copyright file="WRMSkill.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;

namespace WarriorRogueMage.Skills
{
    /// <summary>A WRM game skill.</summary>
    public class WRMSkill : GameSkill
    {
        /// <summary>Initializes a new instance of the <see cref="WRMSkill"/> class.</summary>
        /// <param name="name">The name of the skill.</param>
        /// <param name="controllingAttribute">The controlling attribute for this skill.</param>
        /// <param name="description">The description for this skill.</param>
        public WRMSkill(string name, string controllingAttribute, string description)
        {
            Name = name;
            Description = description;
            ControllingAttribute = controllingAttribute;
        }

        /// <summary>Initializes a new instance of the <see cref="WRMSkill"/> class.</summary>
        /// <param name="name">The name of the skill.</param>
        /// <param name="controllingAttribute">The controlling attribute for this skill.</param>
        /// <param name="description">The description for this skill.</param>
        /// <param name="playerThing">The player thing that will be the parent of this skill.</param>
        public WRMSkill(string name, string controllingAttribute, string description, Thing playerThing)
        {
            Name = name;
            Description = description;
            ControllingAttribute = controllingAttribute;
            PlayerThing = playerThing;
        }

        /// <summary>Initializes a new instance of the <see cref="WRMSkill"/> class.</summary>
        /// <param name="playerThing">The player thing that will be the parent of this skill.</param>
        public WRMSkill(Thing playerThing)
        {
            PlayerThing = playerThing;
        }
    }
}