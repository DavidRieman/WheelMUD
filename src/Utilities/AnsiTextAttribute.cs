//-----------------------------------------------------------------------------
// <copyright file="AnsiTextAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    /// <summary>ANSI code for text output attributes.</summary>
    /// <remarks>
    /// See AnsiHandler for typical usage scenarios.
    /// Dim (2) and Blink (5) are intentionally omitted from WheelMUD Core, as support is not universal and might not be recommended to use even if they were.
    /// </remarks>
    public enum AnsiTextAttribute : int
    {
        /// <summary>Normal attribute. Resets all other active attributes.</summary>
        Normal = 0,

        /// <summary>Bold attribute.</summary>
        Bold = 1,

        /// <summary>Underline attribute.</summary>
        Underline = 4,

        /// <summary>Hidden attribute.</summary>
        /// <remarks>
        /// This may be useful to set when prompting for passwords or other sensitive info. Beware the support for this
        /// might not be universal across all Telnet clients, but it shouldn't hurt to try to use it anyways.
        /// </remarks>
        Hidden = 8
    }
}
