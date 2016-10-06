// <copyright file="StepStatus.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Enumeration of possible step statuses. This type is passed to the
//   character creation state machine to decide on the course of action
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    /// <summary>Enumeration of possible step statuses.</summary>
    /// <remarks>This type is passed to the character creation state machine to decide on the course of action.</remarks>
    public enum StepStatus
    {
        /// <summary>The step was a success.</summary>
        Success,

        /// <summary>The step was a failure.</summary>
        Failure,
    }
}