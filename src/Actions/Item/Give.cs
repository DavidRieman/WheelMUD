//-----------------------------------------------------------------------------
// <copyright file="Give.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>Command to give something from your inventory to another entity.</summary>
    /// <remarks>
    /// Some accepted command form examples are as follows:
    ///  "give sword to Karak"
    ///  "give sword Karak"
    ///  "give 10 coins to Karak"
    ///  "give 10 coins Karak"
    /// </remarks>
    [CoreExports.GameAction(0)]
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
        private Thing thing;

        private MovableBehavior movableBehavior;

        /// <summary>The quantity of item to be given (if specified, from a stack of items).</summary>
        private int numberToGive;

        /// <summary>The target recipient, to receive the item.</summary>
        private Thing target;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Remove the item from the character's possession.
            // TODO: Test, this may be broken now... esp for numberToGive != max
            if (numberToGive > 0 && thing != null)
            {
                thing.RemoveFromParents();
            }

            var contextMessage = new ContextualString(actionInput.Actor, target)
            {
                ToOriginator = $"You gave {thing.Name} to {target}.",
                ToReceiver = $"{actionInput.Actor.Name} gave you {thing.Name}.",
                ToOthers = $"{actionInput.Actor.Name} gave {thing.Name} to {target.Name}.",
            };
            var message = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            // Try to move the thing from the sender to the target; this handles eventing and whatnot for us.
            if (!movableBehavior.Move(target, actionInput.Actor, null, message))
            {
                actionInput.Session?.WriteLine($"Failed to give {thing.Name} to {target.Name}.");
            }
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // Check to see if the first word is a number.
            // If so, shunt up the positions of our other params.
            var itemParam = 0;
            var numberWords = actionInput.Params.Length;
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
            var itemName = actionInput.Params[itemParam];

            // Do we have an item matching the name in our inventory?
            thing = actionInput.Actor.FindChild(itemName.ToLower());
            if (thing == null)
            {
                return $"You do not hold '{itemName}'.";
            }

            // The final argument should be the target name.
            var targetName = actionInput.Params[^1];

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
            if (actionInput.Actor.Parent.Id != target.Parent.Id)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: The thing being given must be movable.
            movableBehavior = thing.FindBehavior<MovableBehavior>();

            return null;
        }
    }
}