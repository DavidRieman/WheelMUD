//-----------------------------------------------------------------------------
// <copyright file="ExportCharacterCreationStateMachineAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.ConnectionStates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using WheelMUD.Utilities;

    /// <summary>Class that exports attributes for the <see cref="CharacterCreationStateMachine"/> class.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportCharacterCreationStateMachineAttribute : ExportAttribute, IExportWithPriority
    {
        /// <summary>Initializes a new instance of the <see cref="ExportCharacterCreationStateMachineAttribute"/> class.</summary>
        /// <param name="stateMachinePriority">The state machine priority.</param>
        public ExportCharacterCreationStateMachineAttribute(int stateMachinePriority)
            : base(typeof(CharacterCreationStateMachine))
        {
            Priority = stateMachinePriority;
        }

        /// <summary>Initializes a new instance of the <see cref="ExportCharacterCreationStateMachineAttribute"/> class.</summary>
        /// <param name="metadata">The metadata.</param>
        public ExportCharacterCreationStateMachineAttribute(IDictionary<string, object> metadata)
        {
            PropertyTools.SetProperties(this, metadata);
        }

        /// <summary>Gets or sets the priority of the exported state; the state with the highest priority will be the default initial state.</summary>
        /// <remarks>See DefaultComposer for detailed usage information.</remarks>
        public int Priority { get; set; }
    }
}