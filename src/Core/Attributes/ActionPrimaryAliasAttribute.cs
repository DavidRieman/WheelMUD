//-----------------------------------------------------------------------------
// <copyright file="ActionPrimaryAliasAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A primary action alias attribute.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Attributes
{
    using System;

    /// <summary>The attribute that goes above the primary alias for an action.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionPrimaryAliasAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the ActionPrimaryAliasAttribute class.</summary>
        /// <param name="alias">The actual alias that a user types in.</param>
        /// <param name="category">The category that this action is under.</param>
        public ActionPrimaryAliasAttribute(string alias, CommandCategory category)
        {
            this.Alias = alias;
            this.Category = category;
        }

        /// <summary>Gets the alias attribute for the action.</summary>
        public string Alias { get; private set; }

        /// <summary>Gets the category that this action is filed under.</summary>
        public CommandCategory Category { get; private set; }
    }
}