//-----------------------------------------------------------------------------
// <copyright file="BasePrioritizedExportAttribute.cs" company="WheelMUD Development Team">
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
    public class BasePrioritizedExportAttribute : ExportAttribute, IExportWithPriority
    {
        /// <summary>Initializes a new instance of the class.</summary>
        /// <param name="priority">The priority for this export. Highest number among similar exports gets used.</param>
        public BasePrioritizedExportAttribute(int priority, Type definitionType) : base(definitionType)
            => Priority = priority;

        /// <summary>Initializes a new instance of the class.</summary>
        /// <param name="metadata">The metadata.</param>
        public BasePrioritizedExportAttribute(IDictionary<string, object> metadata)
            => PropertyTools.SetProperties(this, metadata);

        /// <summary>Gets or sets the priority of the export. Highest number among similar exports gets used.</summary>
        /// <remarks>See DefaultComposer for detailed usage information.</remarks>
        public int Priority { get; set; }
    }
}
