//-----------------------------------------------------------------------------
// <copyright file="ExportGameAttributeAttribute.cs" company="WheelMUD Development Team">
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
    /// <summary>An [ExportGameAttribute] attribute to mark GameAttributes for export through MEF.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportGameAttributeAttribute : ExportAttribute, IExportWithPriority
    {
        /// <summary>Initializes a new instance of the ExportGameAttributeAttribute class.</summary>
        public ExportGameAttributeAttribute(int priority)
            : base(typeof(GameAttribute)) => Priority = priority;

        /// <summary>Initializes a new instance of the ExportGameAttributeAttribute class.</summary>
        /// <param name="metadata">The metadata.</param>
        public ExportGameAttributeAttribute(IDictionary<string, object> metadata) => PropertyTools.SetProperties(this, metadata);

        /// <summary>Gets or sets the priority of the exported game attribute; the attribute with the highest priority will be the used attribute.</summary>
        /// <remarks>See DefaultComposer for detailed usage information.</remarks>
        public int Priority { get; set; }
    }
}