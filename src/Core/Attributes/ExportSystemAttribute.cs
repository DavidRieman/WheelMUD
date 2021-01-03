//-----------------------------------------------------------------------------
// <copyright file="ExportSystemAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using WheelMUD.Interfaces;
    using WheelMUD.Utilities;

    /// <summary>An attribute to export SystemExporters with.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportSystemAttribute : ExportAttribute, IExportWithPriority
    {
        /// <summary>Initializes a new instance of the ExportSystemAttribute class.</summary>
        /// <param name="systemPriority">The priority of this system export; The highest priority version wins.</param>
        public ExportSystemAttribute(int systemPriority)
            : base(typeof(SystemExporter))
        {
            Priority = systemPriority;
        }

        /// <summary>Initializes a new instance of the ExportSystemAttribute class.</summary>
        /// <param name="metadata">The metadata.</param>
        public ExportSystemAttribute(IDictionary<string, object> metadata)
        {
            PropertyTools.SetProperties(this, metadata);
        }

        /// <summary>Gets or sets the priority of the exported instance. Only the highest priority version gets utilized.</summary>
        /// <remarks>See DefaultComposer for detailed usage information.</remarks>
        public int Priority { get; set; }
    }
}