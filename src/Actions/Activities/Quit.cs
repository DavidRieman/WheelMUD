//-----------------------------------------------------------------------------
// <copyright file="Quit.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command to quit the game, and disconnect gracefully.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("quit", CommandCategory.Activities)]
    [ActionAlias("qq", CommandCategory.Activities)]
    [ActionDescription("Leave the game.")]
    [ActionSecurity(SecurityRole.player)]
    public class Quit : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAPlayer,
        };

        /// <summary>The player behavior of the player issuing the command.</summary>
        private PlayerBehavior playerBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            playerBehavior.LogOut();
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // This initiator is already guaranteed by VerifyCommonGuards to be a player, hence no null check.
            playerBehavior = actionInput.Actor.FindBehavior<PlayerBehavior>();
            return null;
        }
    }
}