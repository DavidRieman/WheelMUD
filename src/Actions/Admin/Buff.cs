//-----------------------------------------------------------------------------
// <copyright file="Buff.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Allows changing the current value of a stat.
//   Created: March 2012 by James McManus
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Effects;
    using WheelMUD.Interfaces;

    /// <summary>Allows changing the current value of an attribute, but not min, max, etc.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("buff", CommandCategory.Admin)]
    [ActionDescription("Usage: buff (target) (stat) (value/min/max) (amount) [minutes]\r\nExample: buff fred HP max 10 5 [increase fred's max HP by 10 for 5 minutes]")]
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

        private string[] validModTypes = new string[]
        {
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
            IController sender = actionInput.Controller;
            var originator = sender.Thing;

            // Strings to be displayed when the effect is applied/removed.
            var buffString = new ContextualString(sender.Thing, this.target)
            {
                ToOriginator = string.Format("\r\nThe '{0}' stat of {1} has changed by {2}.\r\n", this.stat.Name, this.target.Name, this.modAmount),
                ToReceiver = string.Format("\r\nYour '{0}' stat has changed by {1}.\r\n", this.stat.Name, this.modAmount)
            };
            var unbuffString = new ContextualString(sender.Thing, this.target)
            {
                ToReceiver = string.Format("\r\nYour '{0}' stat goes back to normal.", this.stat.Abbreviation)
            };

            // Turn the above sets of strings into sensory messages.
            var sensoryMessage = new SensoryMessage(SensoryType.Sight, 100, buffString);
            var expirationMessage = new SensoryMessage(SensoryType.Sight, 100, unbuffString);

            // Remove all existing effects on stats with the same abbreviation
            // to prevent the effects from being stacked, at least for now.
            foreach (var effect in this.target.Behaviors.OfType<StatEffect>())
            {
                if (effect.Stat.Abbreviation == this.stat.Abbreviation)
                {
                    sender.Thing.Behaviors.Remove(effect);
                }
            }

            // Create the effect, based on the type of modification.
            StatEffect statEffect = null;
            switch (this.modType)
            {
                case "value":
                    statEffect = new StatEffect(sender.Thing, this.stat, this.modAmount, 0, 0, this.duration, sensoryMessage, expirationMessage);
                    break;
                case "min":
                    statEffect = new StatEffect(sender.Thing, this.stat, 0, this.modAmount, 0, this.duration, sensoryMessage, expirationMessage);
                    break;
                case "max":
                    statEffect = new StatEffect(sender.Thing, this.stat, 0, 0, this.modAmount, this.duration, sensoryMessage, expirationMessage);
                    break;
            }

            // Apply the effect.
            if (statEffect != null)
            {
                this.target.Behaviors.Add(statEffect);
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

            if (actionInput.Params.Length < 5)
            {
                return "See 'help buff' for usage details.";
            }

            var args = actionInput.Params;

            string targetString = args[0];

            string statString = args[1];
            
            this.modType = args[2].ToLower().Trim();
            
            string amountString = args[3];

            string durationString = null;
            if (args.Length > 4)
            {
                durationString = args[4];
            }

            // Find the player.
            this.target = PlayerManager.Instance.FindPlayerByName(targetString, true);
            if (this.target == null)
            {
                return string.Format("Could not find a target named {0}.", targetString);
            }

            // Make sure a valid stat was specified.
            if (!this.target.Stats.TryGetValue(statString, out this.stat))
            {
                return string.Format("{0} does not have a stat called {1}.", this.target.Name, statString);
            }

            // Make sure the mod type ('value', 'min', or 'max') is valid.
            if (!this.validModTypes.Contains(this.modType))
            {
                return string.Format("'{0}' is unrecognized. Try modifying the stat's 'value', 'min', or 'max'.", this.modType);
            }

            // Parse the mod amount and make sure it is an integer.
            if (!int.TryParse(amountString, out this.modAmount))
            {
                return string.Format("The amount '{0}' was not valid.", amountString);
            }

            // Parse the duration string and make sure it is valid.
            // Treat it as a number of minutes.
            if (string.IsNullOrEmpty(durationString))
            {
                this.duration = TimeSpan.FromMinutes(5);
            }
            else
            {
                double minutes;
                if (double.TryParse(durationString, out minutes))
                {
                    this.duration = TimeSpan.FromMinutes(minutes);
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
