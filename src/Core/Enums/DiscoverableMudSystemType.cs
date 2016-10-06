//-----------------------------------------------------------------------------
// <copyright file="DiscoverableMudSystemType.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 5/21/2009 9:42:50 PM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Enums
{
    /// <summary>A list of the MUD systems that can be replaced using the pluggable MEF framework.</summary>
    /// <remarks>See: http://mef.codeplex.com</remarks>
    public enum DiscoverableMudSystemType
    {
        /// <summary>Character Creation.</summary>
        CharacterCreation,

        /// <summary>Questing System.</summary>
        Quest,

        /// <summary>Combat System.</summary>
        Combat,

        /// <summary>Skill System.</summary>
        Skill,

        /// <summary>Magic System.</summary>
        Magic
    }
}