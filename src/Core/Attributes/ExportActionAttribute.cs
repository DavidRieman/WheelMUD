//-----------------------------------------------------------------------------
// <copyright file="ExportActionAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using WheelMUD.Actions;
    using WheelMUD.Interfaces;
    using WheelMUD.Utilities;

    /// <summary>An attribute to export GameActions with.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportGameActionAttribute : ExportAttribute, IExportWithPriority
    {
        /// <summary>Initializes a new instance of the ExportGameActionAttribute class.</summary>
        /// <param name="priority">The import priority of this action; Highest priority for instances of a given action name wins, followed by latest binary for tie breaks.</param>
        public ExportGameActionAttribute(int priority)
            : base(typeof(GameAction))
        {
            this.Priority = priority;
        }

        /// <summary>Initializes a new instance of the class.</summary>
        /// <param name="metadata">The metadata.</param>
        public ExportGameActionAttribute(IDictionary<string, object> metadata)
        {
            PropertyTools.SetProperties(this, metadata);
        }

        /// <summary>Gets or sets the priority of the exported game action; the action with the highest priority will be the used action.</summary>
        /// <remarks>
        /// Default exports (those that ship with WheelMUD core libraries) are priority 0, while an individual game system
        /// may export things at priority 100, and one-off versions created specifically for a MUD instance should have a
        /// higher priority than that. You could disable a customized version simply by setting the priority negative.
        /// </remarks>
        public int Priority { get; set; }
    }
}