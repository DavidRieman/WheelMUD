//-----------------------------------------------------------------------------
// <copyright file="Boot.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to disconnect a player from the game.
//   @@@ TODO: Implement
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>An action to disconnect a player from the game.</summary>
    [ExportGameAction]
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
            // @@@ TODO: Inform the player by sending a non-sensory event
            ////connection.Send("You have been booted from the server.");
            this.playerBehavior.LogOut(); 

            // Inform the admin
            IController sender = actionInput.Controller;
            sender.Write(string.Format("The player named \"{0}\" was booted from game.", this.PlayerToBoot.Name));
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

            string playerName = actionInput.Tail;
            this.PlayerToBoot = PlayerManager.Instance.FindPlayerByName(playerName, false);
            if (this.PlayerToBoot != null)
            {
                this.playerBehavior = this.PlayerToBoot.Behaviors.FindFirst<PlayerBehavior>();
            }

            if (this.PlayerToBoot == null || this.playerBehavior == null)
            {
                return string.Format("The player named \"{0}\" specified could not be found.", playerName);
            }

            return null;
        }
    }
}