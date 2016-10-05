//-----------------------------------------------------------------------------
// <copyright file="OtherGendersExample.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created by Karak, 2011-06-05
//   (Technically we probably don't want these choices for players but here's an example
//   of having a game element which has some elements defined in Core and others expanded
//   by the game engine.)
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    using WheelMUD.Core;

    /// <summary>Eunuch Gender.</summary>
    [ExportGameGender]
    public class GenderEunuch : GameGender
    {
        public GenderEunuch() : base("Eunuch", "E")
        {
        }
    }

    /// <summary>Neuter Gender.</summary>
    [ExportGameGender]
    public class GenderNeuter : GameGender
    {
        public GenderNeuter() : base("Neuter", "N")
        {
        }
    }
}