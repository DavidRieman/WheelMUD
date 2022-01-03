//-----------------------------------------------------------------------------
// <copyright file="Commands.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command to list all commands. Can list by category.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("commands", CommandCategory.Inform)]
    [ActionDescription("List the available commands.")]
    [ActionSecurity(SecurityRole.all)]
    public class Commands : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            var requestedCategory = actionInput.Tail.ToLower();

            // Get a command array of all commands available to the actor.
            var commands = CommandManager.Instance.GetCommandsFor(actionInput.Actor);

            if (requestedCategory == "all")
            {
                session.Write(Renderer.Instance.RenderCommandsList(session.TerminalOptions, commands, "All"));
            }
            else if (Enum.TryParse(requestedCategory, true, out CommandCategory category))
            {
                var commandsInCategory = from c in commands where c.Category.HasFlag(category) select c;
                session.Write(Renderer.Instance.RenderCommandsList(session.TerminalOptions, commandsInCategory, category.ToString()));
            }
            else
            {
                session.Write(Renderer.Instance.RenderCommandsCategories(session.TerminalOptions, commands));
            }
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