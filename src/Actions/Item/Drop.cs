//-----------------------------------------------------------------------------
// <copyright file="Drop.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command to drop an object from the player inventory to the room.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("drop", CommandCategory.Item)]
    [ActionDescription("Drop an object from your inventory.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    internal class Drop : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
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
            var contextMessage = new ContextualString(actionInput.Controller.Thing, thingToDrop.Parent)
            {
                ToOriginator = $"You drop up {thingToDrop.Name}.",
                ToReceiver = $"{actionInput.Controller.Thing.Name} drops {thingToDrop.Name} in you.",
                ToOthers = $"{actionInput.Controller.Thing.Name} drops {thingToDrop.Name}."
            };
            var dropMessage = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            var actor = actionInput.Controller.Thing;
            if (movableBehavior.Move(dropLocation, actor, null, dropMessage))
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

            var targetName = actionInput.Tail.Trim().ToLower();

            // Rule: if 2 params
            if (actionInput.Params.Length == 2)
            {
                int.TryParse(actionInput.Params[0], out numberToDrop);

                targetName = numberToDrop == 0 ? actionInput.Tail.ToLower() :
                    actionInput.Tail.Remove(0, actionInput.Params[0].Length).Trim().ToLower();
            }

            // Rule: Did the initiator look to drop something?
            if (targetName == string.Empty)
            {
                return "What did you want to drop?";
            }

            dropLocation = actionInput.Controller.Thing.Parent;

            // Rule: Is the target an item in the entity's inventory?
            thingToDrop = actionInput.Controller.Thing.Children.Find(t => t.Name.Equals(targetName, StringComparison.CurrentCultureIgnoreCase));
            if (thingToDrop == null)
            {
                return $"You do not hold {targetName}.";
            }

            // Rule: The target thing must be movable.
            movableBehavior = thingToDrop.Behaviors.FindFirst<MovableBehavior>();
            return movableBehavior == null ? $"{thingToDrop.Name} does not appear to be movable." : null;
        }
    }
}