//-----------------------------------------------------------------------------
// <copyright file="Title.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions
{
    /// <summary>A command to set a player's title.</summary>
    /// <remarks>
    /// TODO: Maybe use an App.config flag to decide if this command should be something users do for themselves,
    ///   or an admin only command by default. Of course, a game system may wish to replace this with a system for
    ///   earning specific pre-set titles depending on in-game progress (e.g. dragon-slayer), for the command to
    ///   be available to the player but only selecting from an earned list of titles. SEE Pretitle.cs as well.
    /// </remarks>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("title", CommandCategory.Player)]
    [ActionAlias("set title", CommandCategory.Player)]
    [ActionDescription("Set or view your title.")]
    [ActionSecurity(SecurityRole.player)]
    public class Title : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards> { };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (!(actionInput.Controller is Session session)) return;
            if (actionInput.Controller.Thing == null) return;
            
            if (string.IsNullOrEmpty(actionInput.Tail))
            {
                actionInput.Controller.Write(new OutputBuilder(session.TerminalOptions).
                    SingleLine($"Your current title is '{actionInput.Controller.Thing.Title}'."));
            }
            else
            {
                actionInput.Controller.Thing.Title = actionInput.Tail;
                actionInput.Controller.Write(new OutputBuilder(session.TerminalOptions).
                    SingleLine($"Your title is now '{actionInput.Controller.Thing.Title}'"));
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