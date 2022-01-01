//-----------------------------------------------------------------------------
// <copyright file="ExportGameStatAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using WheelMUD.Utilities;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>An [ExportGameStat] attribute to mark GameStats for export through MEF.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportGameStatAttribute : ExportAttribute, IExportWithPriority
    {
        /// <summary>Initializes a new instance of the ExportGameStatAttribute class.</summary>
        public ExportGameStatAttribute(int priority)
            : base(typeof(GameStat)) => Priority = priority;

        /// <summary>Initializes a new instance of the ExportGameStatAttribute class.</summary>
        /// <param name="metadata">The metadata.</param>
        public ExportGameStatAttribute(IDictionary<string, object> metadata) => PropertyTools.SetProperties(this, metadata);

        /// <summary>Gets or sets the priority of the exported game stat.</summary>
        /// <remarks>See DefaultComposer for detailed usage information.</remarks>
        public int Priority { get; set; }
    }
}