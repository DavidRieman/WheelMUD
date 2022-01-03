//-----------------------------------------------------------------------------
// <copyright file="OtherGendersExample.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;

namespace WarriorRogueMage
{
    /// <summary>Neuter Gender.</summary>
    /// <remarks>
    /// Technically we probably don't want these specific choices for players but so far this is here as an example
    /// of having a game element which has some options defined in Core and others expanded by the specific game.
    /// TODO: Consider replacing our default char creation gender selection with a pronoun set selection step.
    ///       See: https://github.com/DavidRieman/WheelMUD/issues/68
    /// </remarks>
    [ExportGameGender]
    public class GenderNeuter : GameGender
    {
        public GenderNeuter() : base("Neuter", "N")
        {
        }
    }
}