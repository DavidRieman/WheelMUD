//-----------------------------------------------------------------------------
// <copyright file="Boot.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

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
        private PlayerBehavior TargetPlayerBehavior { get; set; }

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            TargetPlayerBehavior.Parent.FindBehavior<UserControlledBehavior>()?.Session?.WriteLine("You are being booted from the server.");
            TargetPlayerBehavior.LogOut(true);

            // Inform the admin too.
            actionInput.Session?.WriteLine($"{TargetPlayerBehavior.Parent.Name} was forcibly disconnected (but is not automatically banned).");
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
            var targetPlayer = PlayerManager.Instance.FindLoadedPlayerByName(playerName, false);
            TargetPlayerBehavior = targetPlayer?.FindBehavior<PlayerBehavior>();

            if (TargetPlayerBehavior == null)
            {
                return $"A player named \"{playerName}\" could not be found online.";
            }

            return null;
        }
    }
}