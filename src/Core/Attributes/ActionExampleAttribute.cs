//-----------------------------------------------------------------------------
// <copyright file="ActionExampleAttribute.cs" company="WheelMUD Development Team">
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

    /// <summary>Action example attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionExampleAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the ActionExampleAttribute class.</summary>
        /// <param name="example">The example of the action.</param>
        public ActionExampleAttribute(string example)
        {
            this.Example = example;
        }

        /// <summary>Gets the description of this action.</summary>
        public string Example { get; private set; }
    }
}