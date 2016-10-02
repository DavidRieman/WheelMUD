//-----------------------------------------------------------------------------
// <copyright file="ExportCharacterCreationStateMachineAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: September 2010 by Karak.
//   http://codebetter.com/blogs/glenn.block/archive/2009/12/04/building-hello-mef-part-ii-metadata-and-why-being-lazy-is-a-good-thing.aspx
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using WheelMUD.Utilities;

    /// <summary>Class that exports attributes for the <see cref="CharacterCreationStateMachine"/> class.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportCharacterCreationStateMachineAttribute : ExportAttribute
    {
        /// <summary>Initializes a new instance of the <see cref="ExportCharacterCreationStateMachineAttribute"/> class.</summary>
        /// <param name="stateMachinePriority">The state machine priority.</param>
        public ExportCharacterCreationStateMachineAttribute(int stateMachinePriority)
            : base(typeof(CharacterCreationStateMachine))
        {
            this.StateMachinePriority = stateMachinePriority;
        }

        /// <summary>Initializes a new instance of the <see cref="ExportCharacterCreationStateMachineAttribute"/> class.</summary>
        /// <param name="metadata">The metadata.</param>
        public ExportCharacterCreationStateMachineAttribute(IDictionary<string, object> metadata)
        {
            PropertyTools.SetProperties(this, metadata);
        }

        /// <summary>Gets or sets the priority of the exported state; the state with the highest priority will be the default initial state.</summary>
        /// <remarks>Do not exceed ConnectedState's priority (100 ATM) unless the state you're exporting is intended to replace ConnectedState.</remarks>
        public int StateMachinePriority { get; set; }
    }
}