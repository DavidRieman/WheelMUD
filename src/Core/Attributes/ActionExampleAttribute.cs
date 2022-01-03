//-----------------------------------------------------------------------------
// <copyright file="ActionExampleAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Core
{
    /// <summary>Action example attribute.</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActionExampleAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the ActionExampleAttribute class.</summary>
        /// <param name="example">The example of the action.</param>
        public ActionExampleAttribute(string example)
        {
            Example = example;
        }

        /// <summary>Gets the description of this action.</summary>
        public string Example { get; private set; }
    }
}