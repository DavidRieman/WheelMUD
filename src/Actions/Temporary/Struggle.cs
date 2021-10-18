//-----------------------------------------------------------------------------
// <copyright file="Struggle.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Effects;
using WheelMUD.Server;

namespace WheelMUD.Actions
{
    /// <summary>Temporary script to to get out of a web.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("struggle", CommandCategory.Temporary)]
    [ActionDescription("Temporary test command. Struggles out of a web.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Struggle : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious
        };

        private ImmobileEffect immobileEffect;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Set up the dice
            var die = DiceService.Instance.GetDie(10);

            // Die has to be > 5 to get out
            // If this were non-temporary, it should be using events instead of Write.
            var userControlledBehavior = actionInput.Actor.Behaviors.FindFirst<UserControlledBehavior>();
            if (die.Roll() > 5)
            {
                actionInput.Actor.Behaviors.Remove(immobileEffect);
                userControlledBehavior.Controller.Write(new OutputBuilder().
                    AppendLine("You manager to struggle free"));
            }
            else
            {
                userControlledBehavior.Controller.Write(new OutputBuilder().
                    AppendLine("You fail to struggle from the web"));
            }

            actionInput.Actor.Behaviors.Add(new UnbalanceEffect()
            {
                Duration = new TimeSpan(0, 0, 0, 0, 500),
            });
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

            // Rule: The initiator must be currently immobilized.
            immobileEffect = actionInput.Actor.Behaviors.FindFirst<ImmobileEffect>();
            if (immobileEffect == null)
            {
                return "You are not immobile, so what is the point of struggling?";
            }

            return null;
        }
    }
}