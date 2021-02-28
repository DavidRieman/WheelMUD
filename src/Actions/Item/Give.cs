//-----------------------------------------------------------------------------
// <copyright file="Give.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Interfaces;

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>Command to give something from your inventory to another entity.</summary>
    /// <remarks>
    /// Some accepted command form examples are as follows:
    ///  "give sword to Karak"
    ///  "give sword Karak"
    ///  "give 10 coins to Karak"
    ///  "give 10 coins Karak"
    /// </remarks>
    [ExportGameAction(0)]
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
            // TODO: Test, this may be broken now... esp for numberToGive != max
            IController sender = actionInput.Controller;
            if (numberToGive > 0 && thing != null)
            {
                thing.RemoveFromParents();
            }

            var contextMessage = new ContextualString(sender.Thing, target)
            {
                ToOriginator = $"You gave {thing.Name} to {target}.",
                ToReceiver = $"{sender.Thing.Name} gave you {thing.Name}.",
                ToOthers = $"{sender.Thing.Name} gave {thing.Name} to {target.Name}.",
            };
            var message = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            // Try to move the thing from the sender to the target; this handles eventing and whatnot for us.
            if (!movableBehavior.Move(target, sender.Thing, null, message))
            {
                sender.Write($"Failed to give {thing.Name} to {target.Name}.");
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
            if (int.TryParse(actionInput.Params[0], out numberToGive))
            {
                itemParam = 1;

                // If the user specified a number, but it is less than 1, error!
                if (numberToGive < 1)
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
            thing = sender.Thing.FindChild(itemName.ToLower());
            if (thing == null)
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

            // TODO: Shared targeting code should be used, and this rule should be implemented like:
            //       if (target == sender.Thing) ...
            // Rule: The giver cannot also be the receiver.
            if (targetName == "me")
            {
                return "You can't give something to yourself.";
            }

            // Rule: Is the target an entity?
            target = GetPlayerOrMobile(targetName);
            if (target == null)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target in the same room?
            if (sender.Thing.Parent.Id != target.Parent.Id)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: The thing being given must be movable.
            movableBehavior = thing.Behaviors.FindFirst<MovableBehavior>();

            return null;
        }
    }
}