//-----------------------------------------------------------------------------
// <copyright file="Boot.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions
{
    /// <summary>An action to disconnect a player from the game.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("boot", CommandCategory.Admin)]
    [ActionDescription("Disconnect a player from the game.")]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class Boot : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>The player behavior of the player to boot.</summary>
        private PlayerBehavior playerBehavior;

        /// <summary>Gets or sets the player to boot from the game.</summary>
        private Thing PlayerToBoot { get; set; }

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // TODO: Inform the player by sending a non-sensory event
            ////connection.Send("You have been booted from the server.");
            playerBehavior.LogOut();

            // Inform the admin
            actionInput.Controller.Write(new OutputBuilder().
                AppendLine($"The player named \"{PlayerToBoot.Name}\" was booted from game."));
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

            var playerName = actionInput.Tail;
            PlayerToBoot = PlayerManager.Instance.FindLoadedPlayerByName(playerName, false);
            if (PlayerToBoot != null)
            {
                playerBehavior = PlayerToBoot.Behaviors.FindFirst<PlayerBehavior>();
            }

            if (PlayerToBoot == null || playerBehavior == null)
            {
                return $"The player named \"{playerName}\" specified could not be found.";
            }

            return null;
        }
    }
}