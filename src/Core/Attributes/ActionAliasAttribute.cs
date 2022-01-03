﻿//-----------------------------------------------------------------------------
// <copyright file="ActionAliasAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Core
{
    /// <summary>An action alias attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ActionAliasAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the ActionAliasAttribute class.</summary>
        /// <param name="alias">An alias for the action.</param>
        /// <param name="category">Category that this action is part of.</param>
        public ActionAliasAttribute(string alias, CommandCategory category)
        {
            Alias = alias.ToLower();
            Category = category;
        }

        /// <summary>Gets the alias attribute for the action.</summary>
        public string Alias { get; private set; }

        /// <summary>Gets the Category that this action is filed under.</summary>
        public CommandCategory Category { get; private set; }
    }
}