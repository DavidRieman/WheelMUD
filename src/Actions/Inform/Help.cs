//-----------------------------------------------------------------------------
// <copyright file="Help.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions
{
    /// <summary>A command to look up help information from the help system.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("help", CommandCategory.Inform)]
    [ActionDescription("Display help text for a command or topic.")]
    [ActionExample("help look - retrieves help for the \"look\" command.")]
    [ActionSecurity(SecurityRole.all)]
    public class Help : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (!(actionInput.Controller is Session session)) return;

            var commandTail = actionInput.Tail;

            // If no arguments were given, render the help topics list.
            if (string.IsNullOrWhiteSpace(commandTail))
            {
                actionInput.Controller.Write(Renderer.Instance.RenderHelpTopics(session.TerminalOptions));
                return;
            }

            // Check to see if the help topic is in the help manager.
            var helpTopic = HelpManager.Instance.FindHelpTopic(commandTail);
            if (helpTopic != null)
            {
                actionInput.Controller.Write(Renderer.Instance.RenderHelpTopic(session.TerminalOptions, helpTopic));
                return;
            }

            // If not found we check commands.
            var commands = CommandManager.Instance.MasterCommandList;

            // First check for an exact match with a command name.
            var action = commands.Values.FirstOrDefault(c => c.Name.Equals(commandTail, StringComparison.OrdinalIgnoreCase)) ??
                         commands.Values.FirstOrDefault(c => c.Name.ToLower().StartsWith(commandTail));

            // Show result if a match was found
            if (action != null)
            {
                actionInput.Controller.Write(Renderer.Instance.RenderHelpCommand(session.TerminalOptions, action));
                return;
            }

            actionInput.Controller.Write(new OutputBuilder().AppendLine("No such help topic or command was found."));
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}