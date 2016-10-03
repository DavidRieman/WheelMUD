//-----------------------------------------------------------------------------
// <copyright file="GenderTypes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Feverdream
//   Date      : 4/26/2010 10:00:26 PM
//   Purpose   : Flagged Enum to denote possible Genders.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Enums
{
    using System;
    
    /// <summary>All of the different Genders possible. Used for Mobs, Players, and maybe other Things.</summary>
    [Flags]
    public enum GenderTypes : byte
    {
        /// <summary>No gender, either by 'ouch', magick, or by birth.</summary>
        Nueter = 0,

        /// <summary>Male: X/Y chromosome pair.</summary>
        Male = 1,

        /// <summary>Female: X/X chromosome pair.</summary>
        Female = 2,

        /// <summary>Rare Hermaphroditic Gender.  Statisticaly ~1 in 2000 individuals when naturally occurring.</summary>
        Intergender = GenderTypes.Male | GenderTypes.Female
    }
}