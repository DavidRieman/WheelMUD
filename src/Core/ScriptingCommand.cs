//-----------------------------------------------------------------------------
// <copyright file="ScriptingCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A scripting command.
//   Organized: April 2009 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using WheelMUD.CommandSystem;
    using WheelMUD.Core.Attributes;

    /// <summary>A scripting command.</summary>
    public class ScriptingCommand
    {
        /// <summary>Initializes a new instance of the ScriptingCommand class.</summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="executeDelegate">The execution delegate.</param>
        /// <param name="guardsDelegate">The guards delegate.</param>
        /// <param name="securityRole">The required security role in order to execute this command.</param>
        /// <param name="actionInput">The action input to pass through to the delegates.</param>
        public ScriptingCommand(
            string name,
            CommandScriptExecuteDelegate executeDelegate,
            CommandScriptGuardsDelegate guardsDelegate,
            SecurityRole securityRole,
            ActionInput actionInput)
        {
            this.Name = name;
            this.ExecuteDelegate = executeDelegate;
            this.GuardsDelegate = guardsDelegate;
            this.SecurityRole = securityRole;
            this.ActionInput = actionInput;
        }

        /// <summary>Gets the name of the scripting command.</summary>
        public string Name { get; private set; }

        /// <summary>Gets the execution delegate of the scripting command.</summary>
        public CommandScriptExecuteDelegate ExecuteDelegate { get; private set; }

        /// <summary>Gets the guards delegate of the scripting command.</summary>
        public CommandScriptGuardsDelegate GuardsDelegate { get; private set; }

        /// <summary>Gets the required security role in order to execute this command.</summary>
        public SecurityRole SecurityRole { get; private set; }

        /// <summary>Gets the input arguments of the scripting command.</summary>
        public ActionInput ActionInput { get; private set; }
    }
}