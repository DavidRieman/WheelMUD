//-----------------------------------------------------------------------------
// <copyright file="Unmute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Effects;

namespace WheelMUD.Actions
{
    /// <summary>Removes the mute effect from someone prior to its normal expiration.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("unmute", CommandCategory.Admin)]
    [ActionDescription("Removes the mute effect from someone prior to its normal expiration.")]
    [ActionSecurity(SecurityRole.fullAdmin | SecurityRole.minorAdmin)]
    public class Unmute : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument,
        };

        /// <summary>The player to mute.</summary>
        private Thing playerToUnmute;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Remove all instances of MutedEffect (in case there are more than one).
            MutedEffect effect;
            while ((effect = playerToUnmute.FindBehavior<MutedEffect>()) != null)
            {
                // Cancel the event so it will be ignored by TimeSystem when the time expires.
                effect.UnmuteEvent.Cancel("Mute cancelled.");
                effect.Unmute();
            }
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

            // Make sure the target exists.
            var playerName = actionInput.Params[0];
            playerToUnmute = PlayerManager.Instance.FindLoadedPlayerByName(playerName, false);
            if (playerToUnmute == null)
            {
                return $"The player named \"{playerName}\" could not be found.";
            }

            // Make sure the player has a MuteEffect applied.
            if (!playerToUnmute.HasBehavior<MutedEffect>())
            {
                return $"The player {playerToUnmute.Name} was not muted!";
            }

            return null;
        }
    }
}