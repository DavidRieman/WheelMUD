//-----------------------------------------------------------------------------
// <copyright file="Drink.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Universe;

namespace WheelMUD.Actions
{
    /// <summary>A command script to allow the drinking of "drinkable" items.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("drink", CommandCategory.Item)]
    [ActionAlias("sip", CommandCategory.Item)]
    [ActionAlias("quaff", CommandCategory.Item)]
    [ActionDescription("Drink a liquid from a specified container.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Drink : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.RequiresAtLeastOneArgument
        };

        private DrinkableBehavior drinkableBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            drinkableBehavior.Drink(actionInput.Actor);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var actor = actionInput.Actor;
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // Rule: Do we have an item matching in our inventory?
            // TODO: Support drinking from, for instance, a fountain sitting in the room.
            var itemIdentifier = actionInput.Tail.Trim();
            var thingToDrink = actor.FindChild(itemIdentifier.ToLower());
            if (thingToDrink == null)
            {
                return $"You do not hold '{actionInput.Tail}'.";
            }

            // Rule: Is the item drinkable?
            drinkableBehavior = thingToDrink.FindBehavior<DrinkableBehavior>();
            return drinkableBehavior == null ? $"{itemIdentifier} is not drinkable" : null;
        }
    }
}