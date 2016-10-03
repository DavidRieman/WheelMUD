//-----------------------------------------------------------------------------
// <copyright file="ContextCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: Decenber 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using WheelMUD.Actions;
    using WheelMUD.Core.Attributes;

    /// <summary>A ContextCommand is like a Command but simpler, and can only be executed in temporary/certain contexts.</summary>
    /// <remarks>@@@ TODO: Add description/details or whatnot, and have help system list any currently-applicable context commands.</remarks>
    public class ContextCommand
    {
        /// <summary>Initializes a new instance of the <see cref="ContextCommand"/> class.</summary>
        /// <param name="commandScript">The command script.</param>
        /// <param name="commandKey">The command key.</param>
        /// <param name="availability">The availability.</param>
        /// <param name="securityRole">The security role.</param>
        public ContextCommand(
            GameAction commandScript,
            string commandKey, 
            ContextAvailability availability, 
            SecurityRole securityRole)
        {
            this.CommandScript = commandScript;
            this.CommandKey = commandKey;
            this.Availability = availability;
            this.SecurityRole = securityRole;
        }

        /// <summary>Gets the starting command text used to execute this command.</summary>
        public string CommandKey { get; private set; }

        /// <summary>Gets the command script instance for running this command.</summary>
        public GameAction CommandScript { get; private set; }

        /// <summary>Gets the availability of this command to potential users.</summary>
        public ContextAvailability Availability { get; private set; }

        /// <summary>Gets the security role of this command.</summary>
        public SecurityRole SecurityRole { get; private set; }
    }
}