//-----------------------------------------------------------------------------
// <copyright file="Drink.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command script to allow the drinking of "drinkable" items.
//   Created: January 2007 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;
    using WheelMUD.Universe;

    /// <summary>A command script to allow the drinking of "drinkable" items.</summary>
    [ExportGameAction]
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
        
        /// <summary>The drinkable item we are to 'drink' from.</summary>
        private Thing thingToDrink = null;

        private DrinkableBehavior drinkableBehavior = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            sender.Write("You take a drink from " + this.thingToDrink.Name + ".");
            this.drinkableBehavior.Drink(sender.Thing);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }
            
            // Rule: Do we have an item matching in our inventory?
            // @@@ TODO: Support drinking from, for instance, a fountain sitting in the room.
            string itemIdentifier = actionInput.Tail.Trim();
            this.thingToDrink = sender.Thing.FindChild(itemIdentifier.ToLower());
            if (this.thingToDrink == null)
            {
                return "You do not hold " + actionInput.Tail.Trim() + ".";
            }

            // Rule: Is the item drinkable?
            this.drinkableBehavior = this.thingToDrink.Behaviors.FindFirst<DrinkableBehavior>();
            if (this.drinkableBehavior == null)
            {
                return itemIdentifier + " is not drinkable";
            }

            return null;
        }
    }
}