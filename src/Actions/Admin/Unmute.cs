//-----------------------------------------------------------------------------
// <copyright file="Unmute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
// Removes the mute effect from someone prior to its normal expiration.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Effects;

    /// <summary>Removes the mute effect from someone prior to its normal expiration.</summary>
    [ExportGameAction]
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
            while ((effect = this.playerToUnmute.Behaviors.FindFirst<MutedEffect>()) != null)
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
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // Make sure the target exists.
            string playerName = actionInput.Params[0];
            this.playerToUnmute = PlayerManager.Instance.FindPlayerByName(playerName, false);
            if (this.playerToUnmute == null)
            {
                return string.Format("The player named \"{0}\" could not be found.", playerName);
            }

            // Make sure the player has a MuteEffect applied.
            if (!this.playerToUnmute.HasBehavior<MutedEffect>())
            {
                return string.Format("The player {0} was not muted!", this.playerToUnmute.Name);
            }

            return null;
        }
    }
}