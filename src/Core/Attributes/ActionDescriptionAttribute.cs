//-----------------------------------------------------------------------------
// <copyright file="ActionDescriptionAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Core
{
    /// <summary>Action description attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionDescriptionAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the ActionDescriptionAttribute class.</summary>
        /// <param name="description">The description of the action.</param>
        public ActionDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>Gets the description of this action.</summary>
        public string Description { get; private set; }
    }
}