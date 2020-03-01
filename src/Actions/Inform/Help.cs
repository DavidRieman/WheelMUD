//-----------------------------------------------------------------------------
// <copyright file="Help.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;
    using WheelMUD.Utilities;

    /// <summary>A command to look up help information from the help system.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("help", CommandCategory.Inform)]
    [ActionDescription("Display help text for a command or topic.")]
    [ActionExample("Example: help look")]
    [ActionSecurity(SecurityRole.all)]
    public class Help : GameAction
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
            var terminal = (actionInput.Controller as Session).Terminal;
            var commandTail = actionInput.Tail;

            // If no arguments were given, render the help topics list.
            if (string.IsNullOrWhiteSpace(commandTail))
            {
                sender.Write(Renderer.Instance.RenderHelpTopics(terminal));
                return;
            }

            // Check to see if the help topic is in the help manager.
            HelpTopic helpTopic = HelpManager.Instance.FindHelpTopic(commandTail);
            if (helpTopic != null)
            {
                sender.Write(Renderer.Instance.RenderHelpTopic(terminal, helpTopic));
                return;
            }

            // If not found we check commands.
            var commands = CommandManager.Instance.MasterCommandList;

            // First check for an exact match with a command name.
            var action = commands.Values.FirstOrDefault(c => c.Name.Equals(commandTail, StringComparison.OrdinalIgnoreCase));

            // If no exact match, check for a partial match.
            if (action == null)
            {
                action = commands.Values.FirstOrDefault(c => c.Name.ToLower().StartsWith(commandTail));
            }

            // Show result if a match was found
            if (action != null)
            {
                sender.Write(Renderer.Instance.RenderHelpCommand(terminal, action));
                return;
            }

            sender.Write("No such help topic or command was found.");
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = this.VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // There are currently no arguments nor situations where we expect failure.
            return null;
        }
    }
}