//-----------------------------------------------------------------------------
// <copyright file="Commands.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: David
//   Date      : 5/11/2009 8:56:04 PM
//   Purpose   : List all commands.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>List all commands.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("commands", CommandCategory.Inform)]
    [ActionDescription("List the available commands.")]
    [ActionSecurity(SecurityRole.all)]
    public class Commands : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string requestedCategory = actionInput.Tail.ToLower();

            // Get a command array of all commands available to this controller
            List<Command> commands = CommandManager.Instance.GetCommandsForController(sender);

            var output = new StringBuilder();
            CommandCategory category = CommandCategory.None;
            Enum.TryParse(requestedCategory, true, out category);

            if (requestedCategory == "all" || category != CommandCategory.None)
            {
                if (category == CommandCategory.None)
                {
                    output.AppendLine("All commands:");
                }
                else
                {
                    output.AppendFormat("{0} commands:\n", category.ToString());
                }

                // Sort and then output commands in this category
                commands.Sort((Command a, Command b) => a.Name.CompareTo(b.Name));
                foreach (Command c in commands)
                {
                    if (c.Category.HasFlag(category))
                    {
                        output.AppendFormat("{0}{1}\n", c.Name.PadRight(15), c.Description);
                    }
                }
            }
            else
            {
                // Build a list of categories for the commands available to this player.
                output.AppendLine("Please specify a command category:\nAll");

                foreach (CommandCategory c in Enum.GetValues(typeof(CommandCategory)))
                {
                    if (c != CommandCategory.None)
                    {
                        List<Command> matchingcommands = commands.FindAll(c2 => c2.Category.HasFlag(c));
                        if (matchingcommands.Count() > 0)
                        {
                            output.AppendFormat("{0} ({1})\n", c.ToString(), matchingcommands.Count());
                        }
                    }
                }
            }

            sender.Write(output.ToString());
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // There are currently no arguments nor situations where we expect failure.
            return null;
        }
    }
}