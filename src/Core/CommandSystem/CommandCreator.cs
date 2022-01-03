//-----------------------------------------------------------------------------
// <copyright file="CommandCreator.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Linq;

namespace WheelMUD.Core
{
    /// <summary>A delegate used for calling CommandScripts Execute methods.</summary>
    /// <param name="actionInput">The full input specified for executing the command.</param>
    public delegate void CommandScriptExecuteDelegate(ActionInput actionInput);

    /// <summary>A delegate used for calling CommandScripts Execute methods.</summary>
    /// <param name="actionInput">The full input specified for executing the command.</param>
    /// <returns>Returns a string describing the error for the user if any of the guards failed, else null.</returns>
    public delegate string CommandScriptGuardsDelegate(ActionInput actionInput);

    /// <summary>Creates ScriptingCommands based on the provided Actions.</summary>
    public class CommandCreator
    {
        /// <summary>The response given for a player's attempted but unrecognized command.</summary>
        private static readonly string unknownCommandResponse = "Huh?";

        /// <summary>Prevents a default instance of the CommandCreator class from being created.</summary>
        private CommandCreator()
        {
        }

        /// <summary>Gets the singleton instance of the CommandCreator class.</summary>
        public static CommandCreator Instance { get; } = new CommandCreator();

        /// <summary>Creates a scripting command from action input.</summary>
        /// <param name="actionInput">The action input to transform into a ScriptingCommand instance.</param>
        /// <returns>A new ScriptingCommand instance for the specified input, if found, else null.</returns>
        public ScriptingCommand Create(ActionInput actionInput)
        {
            // TODO: Build targeting into command selection when there are multiple valid targets; IE if multiple
            //     openable things registered an "open" context command, then if the user said "open door" then we
            //     want to select the context command attached to the closest match for "door" as our command; if
            //     there are still multiple targets, we can start a conflict resolution prompt, etc.  Individual non-
            //     context commands may also wish to use such targeting code, so it should be built to be reusable.
            ScriptingCommand command = TryCreateMasterCommand(actionInput, 0) ??
                                       TryCreateMasterCommand(actionInput, 1) ??
                                       TryCreateContextCommand(actionInput, 0) ??
                                       TryCreateContextCommand(actionInput, 1);

            if (command == null)
            {
                actionInput.Session?.WriteLine(unknownCommandResponse);
            }

            return command;
        }

        private ScriptingCommand TryCreateMasterCommand(ActionInput actionInput, int lastKeywordIndex)
        {
            string commandAlias = GetCommandAlias(actionInput, lastKeywordIndex);

            // If this isn't actually a command, bail now.
            ////if (string.IsNullOrEmpty(commandAlias) || !this.masterCommandList.ContainsKey(commandAlias))
            if (string.IsNullOrEmpty(commandAlias) || !CommandManager.Instance.MasterCommandList.ContainsKey(commandAlias))
            {
                return null;
            }

            // Create a new instance of the specified GameAction.
            Command command = CommandManager.Instance.MasterCommandList[commandAlias];
            var commandScript = (GameAction)command.Constructor.Invoke(null);

            // Track the execute and guards delegates of this instance for calling soon, with the user's input.
            var executeDelegate = new CommandScriptExecuteDelegate(commandScript.Execute);
            var guardsDelegate = new CommandScriptGuardsDelegate(commandScript.Guards);

            return new ScriptingCommand(command.Name, executeDelegate, guardsDelegate, command.SecurityRole, actionInput);
        }

        private ScriptingCommand TryCreateContextCommand(ActionInput actionInput, int lastKeywordIndex)
        {
            string commandAlias = GetCommandAlias(actionInput, lastKeywordIndex);
            if (string.IsNullOrEmpty(commandAlias))
            {
                return null;
            }

            // Find the first valid command of this name and of applicable context, if any.
            Thing sender = actionInput.Actor;
            var contextCommand = CommandManager.Instance.GetContextCommands(sender, commandAlias).FirstOrDefault();
            if (contextCommand == null)
            {
                return null;
            }

            var executeDelegate = new CommandScriptExecuteDelegate(contextCommand.CommandScript.Execute);
            var guardsDelegate = new CommandScriptGuardsDelegate(contextCommand.CommandScript.Guards);
            return new ScriptingCommand(contextCommand.CommandKey, executeDelegate, guardsDelegate, SecurityRole.all, actionInput);
        }

        /// <summary>Get a potential command alias from the actionInput and up to lastKeywordIndex additional keywords.</summary>
        /// <param name="actionInput">The action input provided by a user issuing a command.</param>
        /// <param name="lastKeywordIndex">The last keyword to append as a potential part of a command alias.</param>
        /// <returns>The potential command alias, if there were enough keywords provided to test an alias this size.</returns>
        private string GetCommandAlias(ActionInput actionInput, int lastKeywordIndex)
        {
            string commandAlias = actionInput.Noun;
            if (lastKeywordIndex > 0)
            {
                if (actionInput.Params.Length < lastKeywordIndex)
                {
                    // Since we test all shorter aliases first, don't wast time with longer aliases
                    // than the user actually provided.
                    return null;
                }

                // Otherwise prepare to scan action input for a potential command that's longer than
                // just one word; IE one could implement "turn off" and "turn on" commands as two
                // separate commands, provided there is no "turn" command present.  If the user then
                // passes "turn off flashlight" then we'll find "turn off" as the command.
                for (int i = 1; i <= lastKeywordIndex; i++)
                {
                    if (actionInput.Params.Length > lastKeywordIndex)
                    {
                        commandAlias += " " + actionInput.Params[0].ToLower();
                    }
                }
            }

            return commandAlias;
        }
    }
}