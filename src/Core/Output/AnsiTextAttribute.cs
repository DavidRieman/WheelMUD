//-----------------------------------------------------------------------------
// <copyright file="AnsiTextAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>ANSI code for text output attributes.</summary>
    /// <remarks>See AnsiHandler for typical usage scenarios.</remarks>
    public enum AnsiTextAttribute : int
    {
        /// <summary>Normal attribute. Resets all other active attributes.</summary>
        Normal = 0,

        /// <summary>Bold attribute.</summary>
        Bold = 1,

        /// <summary>Underline attribute.</summary>
        Underline = 4
    }
}
