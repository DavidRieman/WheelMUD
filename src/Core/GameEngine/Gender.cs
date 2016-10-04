//-----------------------------------------------------------------------------
// <copyright file="GameGender.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : May 22, 2011
//   Purpose   : This represents a player, Mob, or NPC gender.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>This represents a player, Mob, or NPC gender.</summary>
    public class GameGender
    {
        /// <summary>Initializes a new instance of the <see cref="GameGender"/> class.</summary>
        /// <param name="name">The name for this gender.</param>
        /// <param name="abbreviation">The abbreviation.</param>
        public GameGender(string name, string abbreviation)
        {
            this.Name = name;
            this.Abbreviation = abbreviation;
        }

        /// <summary>Gets the abbreviation for this gender.</summary>
        public string Abbreviation { get; private set; }

        /// <summary>Gets the name for this gender.</summary>
        public string Name { get; private set; }
    }
}