//-----------------------------------------------------------------------------
// <copyright file="Commands.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to list all commands. Can list by category.</summary>
    [ExportGameAction(0)]
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
            var terminal = (actionInput.Controller as Session).Terminal;

            // Get a command array of all commands available to this controller
            var commands = CommandManager.Instance.GetCommandsForController(sender);

            if (requestedCategory == "all")
            {
                sender.Write(Renderer.Instance.RenderCommandsList(terminal, commands, "All"));
            }
            else if (Enum.TryParse(requestedCategory, true, out CommandCategory category))
            {
                var commandsInCategory = from c in commands where c.Category.HasFlag(category) select c;
                sender.Write(Renderer.Instance.RenderCommandsList(terminal, commandsInCategory, category.ToString()));
            }
            else
            {
                sender.Write(Renderer.Instance.RenderCommandsCategories(terminal, commands));
            }
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