//-----------------------------------------------------------------------------
// <copyright file="ExportServerHarnessCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using WheelMUD.Utilities.Interfaces;

namespace ServerHarness
{
    /// <summary>An [ExportServerHarnessCommand] attribute to mark server harness commands for export through MEF.</summary>
    /// <remarks>Allows for custom additions and overrides without having to change the base server harness code itself.</remarks>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportServerHarnessCommandAttribute : ExportAttribute, IExportWithPriority
    {
        /// <summary>Initializes a new instance of the ExportGameAttributeAttribute class.</summary>
        /// <param name="priority">The import priority of this class; Highest priority for instances of a given command name wins, followed by latest binary for tie breaks.</param>
        public ExportServerHarnessCommandAttribute(int priority)
            : base(typeof(IServerHarnessCommand))
        {
            Priority = priority;
        }

        /// <summary>Gets or sets the priority of the exported command; the command with the highest priority will be the one loaded.</summary>
        /// <remarks>See DefaultComposer for detailed usage information.</remarks>
        public int Priority { get; set; }
    }
}