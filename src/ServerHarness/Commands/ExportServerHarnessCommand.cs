//-----------------------------------------------------------------------------
// <copyright file="ExportServerHarnessCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ServerHarness
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>An [ExportServerHarnessCommand] attribute to mark server harness commands for export through MEF.</summary>
    /// <remarks>Allows for custom additions and overrides without having to change the base server harness code itself.</remarks>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportServerHarnessCommandAttribute : ExportAttribute
    {
        /// <summary>Initializes a new instance of the ExportGameAttributeAttribute class.</summary>
        public ExportServerHarnessCommandAttribute()
            : base(typeof(IServerHarnessCommand))
        {
        }
    }
}