//-----------------------------------------------------------------------------
// <copyright file="ActionDescriptionAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Action description attribute.
//   Created: April 2009 by Fastalanasa
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Attributes
{
    using System;

    /// <summary>Action description attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionDescriptionAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the ActionDescriptionAttribute class.</summary>
        /// <param name="description">The description of the action.</param>
        public ActionDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        /// <summary>Gets the description of this action.</summary>
        public string Description { get; private set; }
    }
}