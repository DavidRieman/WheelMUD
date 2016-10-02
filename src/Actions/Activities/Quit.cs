//-----------------------------------------------------------------------------
// <copyright file="Quit.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Allows a player to quit.
//   Created: November 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to quit the game, and disconnect gracefully.</summary>
    [ExportGameAction]
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
            this.playerBehavior.LogOut();
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // This initiator is already guaranteed by VerifyCommonGuards to be a player, hence no null check.
            this.playerBehavior = sender.Thing.Behaviors.FindFirst<PlayerBehavior>();

            return null;
        }
    }
}