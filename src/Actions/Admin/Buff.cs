//-----------------------------------------------------------------------------
// <copyright file="Buff.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Effects;

namespace WheelMUD.Actions
{
    /// <summary>Allows changing the current value of an attribute, but not min, max, etc.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("buff", CommandCategory.Admin)]
    [ActionDescription("Usage: buff (target) (stat) (value/min/max) (amount) [minutes]")] // TODO: Add to full help: Example: buff fred HP max 10 5 [increase fred's max HP by 10 for 5 minutes]
    [ActionSecurity(SecurityRole.fullAdmin)]
    public class Buff : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAPlayer,
            CommonGuards.RequiresAtLeastTwoArguments
        };

        private Thing target;
        private GameStat stat;
        private string modType;
        private int modAmount;
        private TimeSpan duration;

        private string[] validModTypes = {
            "value",
            "min",
            "max"
        };

        /// <summary>Executes the command.</summary>
        /// <remarks>
        /// TODO: Optionally allow the admin to create a new attribute if the target didn't
        /// already have the attribute available to modify.
        /// </remarks>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Strings to be displayed when the effect is applied/removed.
            var buffString = new ContextualString(actionInput.Actor, target)
            {
                ToOriginator = $"The '{stat.Name}' stat of {target.Name} has changed by {modAmount}.",
                ToReceiver = $"Your '{stat.Name}' stat has changed by {modAmount}."
            };
            var unbuffString = new ContextualString(actionInput.Actor, target)
            {
                ToReceiver = $"Your '{stat.Abbreviation}' stat goes back to normal."
            };

            // Turn the above sets of strings into sensory messages.
            var sensoryMessage = new SensoryMessage(SensoryType.Sight, 100, buffString);
            var expirationMessage = new SensoryMessage(SensoryType.Sight, 100, unbuffString);

            // Remove all existing effects on stats with the same abbreviation
            // to prevent the effects from being stacked, at least for now.
            foreach (var effect in target.Behaviors.OfType<StatEffect>())
            {
                if (effect.Stat.Abbreviation == stat.Abbreviation)
                {
                    actionInput.Actor.Behaviors.Remove(effect);
                }
            }

            // Create the effect, based on the type of modification.
            StatEffect statEffect = null;
            switch (modType)
            {
                case "value":
                    statEffect = new StatEffect(actionInput.Actor, stat, modAmount, 0, 0, duration, sensoryMessage, expirationMessage);
                    break;
                case "min":
                    statEffect = new StatEffect(actionInput.Actor, stat, 0, modAmount, 0, duration, sensoryMessage, expirationMessage);
                    break;
                case "max":
                    statEffect = new StatEffect(actionInput.Actor, stat, 0, 0, modAmount, duration, sensoryMessage, expirationMessage);
                    break;
            }

            // Apply the effect.
            if (statEffect != null)
            {
                target.Behaviors.Add(statEffect);
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

            if (actionInput.Params.Length < 5)
            {
                return "See 'help buff' for usage details.";
            }

            var args = actionInput.Params;
            var targetString = args[0];
            var statString = args[1];
            modType = args[2].ToLower().Trim();
            var amountString = args[3];
            string durationString = null;
            if (args.Length > 4)
            {
                durationString = args[4];
            }

            // Find the player.
            target = PlayerManager.Instance.FindLoadedPlayerByName(targetString, true);
            if (target == null)
            {
                return $"Could not find a target named {targetString}.";
            }

            // Make sure a valid stat was specified.
            if (!target.Stats.TryGetValue(statString, out stat))
            {
                return $"{target.Name} does not have a stat called {statString}.";
            }

            // Make sure the mod type ('value', 'min', or 'max') is valid.
            if (!validModTypes.Contains(modType))
            {
                return $"'{modType}' is unrecognized. Try modifying the stat's 'value', 'min', or 'max'.";
            }

            // Parse the mod amount and make sure it is an integer.
            if (!int.TryParse(amountString, out modAmount))
            {
                return $"The amount '{amountString}' was not valid.";
            }

            // Parse the duration string and make sure it is valid.
            // Treat it as a number of minutes.
            if (string.IsNullOrEmpty(durationString))
            {
                duration = TimeSpan.FromMinutes(5);
            }
            else
            {
                if (double.TryParse(durationString, out var minutes))
                {
                    duration = TimeSpan.FromMinutes(minutes);
                }
                else
                {
                    return "The duration specified was not valid.";
                }
            }

            return null;
        }
    }
}
