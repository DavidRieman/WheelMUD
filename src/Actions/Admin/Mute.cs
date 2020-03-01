//-----------------------------------------------------------------------------
// <copyright file="Mute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Effects;
    using WheelMUD.Interfaces;

    /// <summary>An administrative action to block a player from communicating verbally.</summary>
    /// <remarks>
    /// Generally used to halt speech that is not in line with the game's terms of service (e.g. abusive speech).
    /// TODO: Track when the state was instantiated, and build in a permanent option (just a "massive" duration?),
    ///       reason tracking, and a way to list all mute characters with their mute reasons, las IP address, and
    ///       so on for detecting trends like players creating new characters to get back at it, etc?
    /// </remarks>
    [ExportGameAction]
    [ActionPrimaryAlias("mute", CommandCategory.Admin)]
    [ActionAlias("silence", CommandCategory.Admin)]
    [ActionDescription("Prevent a player from communicating.")]
    [ActionSecurity(SecurityRole.fullAdmin | SecurityRole.minorAdmin)]
    public class Mute : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument,
        };

        /// <summary>The player to mute.</summary>
        private Thing playerToMute;

        /// <summary>How long the mute effect should last.</summary>
        private TimeSpan muteDuration;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;

            // Strings to be displayed when the effect is applied/removed.
            var muteString = new ContextualString(sender.Thing, this.playerToMute)
            {
                ToOriginator = $"You mute {this.playerToMute.Name} for duration {this.muteDuration}.",
                ToReceiver = $"You are now mute. Please reflect on recent choices.",
            };
            var unmuteString = new ContextualString(sender.Thing, this.playerToMute)
            {
                ToOriginator = $"{this.playerToMute.Name} is no longer mute.",
                ToReceiver = $"You are no longer mute."
            };

            // Turn the above sets of strings into sensory messages.
            var muteMessage = new SensoryMessage(SensoryType.Sight, 100, muteString);
            var unmuteMessage = new SensoryMessage(SensoryType.Sight, 100, unmuteString);

            // Create the effect.
            var muteEffect = new MutedEffect(sender.Thing, this.muteDuration, muteMessage, unmuteMessage);

            // Apply the effect.
            this.playerToMute.Behaviors.Add(muteEffect);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = this.VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            string playerName = actionInput.Params[0];
            this.playerToMute = PlayerManager.Instance.FindLoadedPlayerByName(playerName, false);
            if (this.playerToMute == null)
            {
                return string.Format("The player named \"{0}\" could not be found.", playerName);
            }

            // Parse the duration if it was provided. Otherwise, set a default duration.
            // For simplicity, we just accept time in minutes. Perhaps a future update
            // can provide more flexible parsing of time strings to this and other commands.
            if (actionInput.Params.Length > 1)
            {
                string timeString = actionInput.Tail.Substring(actionInput.Params[0].Length);

                if (double.TryParse(timeString, out double timeInMinutes))
                {
                    this.muteDuration = TimeSpan.FromMinutes(timeInMinutes);
                }
                else
                {
                    return "Could not determine how long you want the mute to last.";
                }
            }
            else
            {
                // Default time if none was specified.
                this.muteDuration = TimeSpan.FromSeconds(15);
            }

            return null;
        }
    }
}
