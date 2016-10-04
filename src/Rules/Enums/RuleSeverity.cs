//-----------------------------------------------------------------------------
// <copyright file="RuleSeverity.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 2/19/2011
//   Purpose   : A central repository for game rule enums.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Rules
{
    /// <summary>Values for validation rule severities.</summary>
    public enum RuleSeverity
    {
        /// <summary>A serious game rule violation that should cause an object to be considered invalid.</summary>
        Error,

        /// <summary>A game rule violation that should be displayed to the user, but which should not make an object be invalid.</summary>
        Warning,

        /// <summary>A game rule result that should be displayed to the user, but which is less severe than a warning.</summary>
        Information,

        /// <summary>A game rule result that should not be displayed to the user, and where the rule was successful.</summary>
        Success
    }
}