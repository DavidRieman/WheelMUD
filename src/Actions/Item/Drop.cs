//-----------------------------------------------------------------------------
// <copyright file="Drop.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command to drop an object from the player inventory to the room.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("drop", CommandCategory.Item)]
    [ActionDescription("Drop an object from your inventory.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Drop : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new()
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile,
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>The thing that we are to 'drop'.</summary>
        private Thing thingToDrop;

        /// <summary>The movable behavior of the thing we are to 'drop'.</summary>
        private MovableBehavior movableBehavior;

        /// <summary>The quantity of the item that we are to 'drop'.</summary>
        private int numberToDrop;

        /// <summary>The current place that the player is within.</summary>
        private Thing dropLocation;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var contextMessage = new ContextualString(actionInput.Actor, thingToDrop.Parent)
            {
                ToOriginator = $"You drop {thingToDrop.Name}.",
                ToReceiver = $"{actionInput.Actor.Name} drops {thingToDrop.Name} in you.",
                ToOthers = $"{actionInput.Actor.Name} drops {thingToDrop.Name}."
            };
            var dropMessage = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            if (movableBehavior.Move(dropLocation, thingToDrop, null, dropMessage))
            {
                // TODO: Transactionally save actors if applicable.
                //actor.Save();
                //dropLocation.Save();
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

            // Check to see if the first word is a number, which will be treated as the quantity to drop, if so.
            int.TryParse(actionInput.Params[0], out numberToDrop);

            var targetName = actionInput.Tail;

            // If there are multiple parameters and the first one is a positive number, treat it as a quantity.
            if (actionInput.Params.Length > 1)
            {
                int.TryParse(actionInput.Params[0], out numberToDrop);
                if (numberToDrop >= 1)
                {
                    targetName = actionInput.Tail.Remove(0, actionInput.Params[0].Length);
                }
            }

            // Rule: Did the initiator look to drop something?
            if (targetName == string.Empty)
            {
                return "What did you want to drop?";
            }

            dropLocation = actionInput.Actor.Parent;

            // Rule: Is the target an item in the entity's inventory?
            thingToDrop = actionInput.Actor.FindChild(targetName);
            if (thingToDrop == null)
            {
                return $"You do not hold '{targetName}'.";
            }

            // Rule: The target thing must be movable.
            movableBehavior = thingToDrop.FindBehavior<MovableBehavior>();
            return movableBehavior == null ? $"{thingToDrop.Name} does not appear to be movable." : null;
        }
    }
}