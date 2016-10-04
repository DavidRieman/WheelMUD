//-----------------------------------------------------------------------------
// <copyright file="RuleConditions.cs" company="WheelMUD Development Team">
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
    /// <summary>The different conditions that can be evaluated by a rule.</summary>
    public enum RuleConditions
    {
        /// <summary>No condition present.</summary>
        None,

        /// <summary>Is Equal Condition.</summary>
        Equality,

        /// <summary>Is Less Than Condition.</summary>
        LessThan,

        /// <summary>Is Less Than Or Equal Condition.</summary>
        LessThanEqual,

        /// <summary>Is Greater Than Condition.</summary>
        GreaterThan,

        /// <summary>Is Greater Than Or Equal Condition.</summary>
        GreaterThanEqual,

        /// <summary>Is Not Equal Condition.</summary>
        NonEquality
    }
}