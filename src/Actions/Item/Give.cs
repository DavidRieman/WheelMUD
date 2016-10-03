//-----------------------------------------------------------------------------
// <copyright file="Give.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that lets a character give items to a character or a mob.
//   Created: April 2009 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>Allows the caller to attempt to give something from their inventory to another entity.</summary>
    /// <remarks>
    /// Some accepted command form examples are as follows:
    ///  "give sword to Karak"
    ///  "give sword Karak"
    ///  "give 10 coins to Karak"
    ///  "give 10 coins Karak"
    /// </remarks>
    [ExportGameAction]
    [ActionPrimaryAlias("give", CommandCategory.Item | CommandCategory.Player)]
    [ActionDescription("Give an item to a character or monster.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Give : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive, 
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile,
            CommonGuards.RequiresAtLeastTwoArguments
        };

        /// <summary>The thing to be given.</summary>
        private Thing thing = null;

        private MovableBehavior movableBehavior;
        
        /// <summary>The quantity of item to be given (if specified, from a stack of items).</summary>
        private int numberToGive = 0;

        /// <summary>The target recipient, to receive the item.</summary>
        private Thing target = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Remove the item from the character's posession.
            // @@@ TODO: Test, this may be broken now... esp for numberToGive != max
            IController sender = actionInput.Controller;
            if (this.numberToGive > 0 && this.thing != null)
            {
                this.thing.RemoveFromParents();
            }

            var contextMessage = new ContextualString(sender.Thing, this.target)
            {
                ToOriginator = string.Format("You gave $Item.Name to {0}.", this.target),
                ToReceiver = "$ActiveThing.Name gave you $Item.Name.",
                ToOthers = string.Format("$ActiveThing.Name gave $Item.Name to {0}.", this.target),
            };
            var message = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            // Try to move the thing from the sender to the target; this handles eventing and whatnot for us.
            if (!this.movableBehavior.Move(this.target, sender.Thing, null, message))
            {
                sender.Write(string.Format("Failed to give {0} to {1}.", this.thing.Name, this.target.Name));
            }
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
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

            // Check to see if the first word is a number.
            // If so, shunt up the positions of our other params.
            int itemParam = 0;
            int numberWords = actionInput.Params.Length;
            if (int.TryParse(actionInput.Params[0], out this.numberToGive))
            {
                itemParam = 1;
                
                // If the user specified a number, but it is less than 1, error!
                if (this.numberToGive < 1)
                {
                    return "You can't give less than 1 of something.";
                }

                // Rule: We should now have at least 3 items in our words array.
                // (IE "give 10 coins to Karak" or "give 10 coins Karak")
                if (numberWords < 3)
                {
                    return "You must specify something to give, and a target.";
                }
            }

            // The next parameter should be the item name (possibly pluralized).
            string itemName = actionInput.Params[itemParam];

            // Do we have an item matching the name in our inventory?
            this.thing = sender.Thing.FindChild(itemName.ToLower());
            if (this.thing == null)
            {
                return "You do not hold " + itemName + ".";
            }

            // The final argument should be the target name.
            string targetName = actionInput.Params[actionInput.Params.Length - 1];

            // Rule: Do we have a target?
            if (string.IsNullOrEmpty(targetName))
            {
                return "You must specify someone to give that to.";
            }

            // @@@ Shared targeting code should be used, and this rule should be implemented like:
            //     if (this.target == sender.Thing) ...
            // Rule: The giver cannot also be the receiver.
            if (targetName == "me")
            {
                return "You can't give something to yourself.";
            }

            // Rule: Is the target an entity?
            this.target = GameAction.GetPlayerOrMobile(targetName);
            if (this.target == null)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target in the same room?
            if (sender.Thing.Parent.ID != this.target.Parent.ID)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: The thing being given must be movable.
            this.movableBehavior = this.thing.Behaviors.FindFirst<MovableBehavior>();

            return null;
        }
    }
}