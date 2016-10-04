//-----------------------------------------------------------------------------
// <copyright file="RuleAction.cs" company="WheelMUD Development Team">
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
    /// <summary>The different actions that this rule can take.</summary>
    public enum RuleAction
    {
        Assign,

        Increase,

        Decrease,

        Multiply,

        Divide
    }
}