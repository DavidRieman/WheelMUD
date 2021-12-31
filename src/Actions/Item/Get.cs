//-----------------------------------------------------------------------------
// <copyright file="Get.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Universe;

namespace WheelMUD.Actions
{
    /// <summary>Action to pick something up from the room, or move something from an inventory container to their inventory.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("get", CommandCategory.Item)]
    [ActionAlias("take", CommandCategory.Item)]
    [ActionDescription("Get an object from a room or from a container.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Get : GameAction
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

        /// <summary>The thing that we wish to 'get'.</summary>
        private Thing thingToGet;

        /// <summary>The movable behavior of the thing we are to 'get'.</summary>
        private MovableBehavior movableBehavior;

        /// <summary>The quantity of the item that we wish to 'get'.</summary>
        private int numberToGet;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Remove the item from its current container.
            // We have to do this before we attempt to add it because of the event subscriptions.
            // TODO: Test, this may be broken now...
            var actor = actionInput.Actor;
            if (numberToGet <= 0)
            {
                numberToGet = 1;
            }

            // TODO: Prevent item duplication from specifying large numbers, or races for same item, etc.
            // TODO: Fix Implementation of numberToGet.
            var contextMessage = new ContextualString(actor, thingToGet.Parent)
            {
                ToOriginator = $"You pick up {thingToGet}.",
                ToReceiver = $"{actor.Name} takes {thingToGet} from you.",
                ToOthers = $"{actor.Name} picks up {thingToGet.Name}.",
            };
            var getMessage = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            if (movableBehavior.Move(actor, actor, getMessage, null))
            {
                // TODO: Transactionally move owners if applicable.
                //actor.Save();
                //actor.Parent.Save();
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
            int.TryParse(actionInput.Params[0], out numberToGet);

//            var itemTargetingIndex = numberToGet > 0 ? 1 : 0;
            var itemName = string.Empty;

            // If the actor is trying to "get ___ from ___" it will have "from" in string but with spaces; If the
            // command is just like "get from" though, we may as well let them try to get an object called "from".
            // Unless we figure out where else to take from, assume we are taking from the actor parent (room) by default.
            Thing takeFrom = actionInput.Actor.Parent;
            if (actionInput.Tail.Contains(" from "))
            {
                // Find the "from" keyword in the parameters.
                var fromIndex = Array.FindIndex(actionInput.Params, s => "from".Equals(s, StringComparison.OrdinalIgnoreCase));

                // Item name is everything from number (if present) to the "from" position.
                for (var j = numberToGet > 0 ? 1 : 0; j < fromIndex; j++)
                {
                    itemName += actionInput.Params[j] + ' ';
                }

                itemName = itemName.Trim();

                // Container targeting words are everything after the "from" word.
                var targetFromName = string.Empty;
                for (var i = fromIndex + 1; i < actionInput.Params.Length; i++)
                {
                    targetFromName += actionInput.Params[i] + ' ';
                }

                targetFromName = targetFromName.Trim();

                // Search for an item matching the one specified in our own inventory, else in the same room.
                Thing foundContainer = actionInput.Actor.FindChild(targetFromName) ?? takeFrom.FindChild(targetFromName);
                if (foundContainer == null)
                {
                    return $"You cannot see {targetFromName}.";
                }

                // The found 'from' thing needs to actually be a container to try to take from it; Do not let them just
                // try to steal from other players or whatnot!
                var containerBehavior = foundContainer.FindBehavior<ContainerBehavior>();
                if (containerBehavior == null)
                {
                    return $"{foundContainer.Name} is not a container you can get things from.";
                }

                // TODO: Removed OpensClosesBehavior check here... Test to ensure that 'get' is blocked by the
                //       OpensClosesBehavior receiving and cancelling the relevant events and message is good...

                takeFrom = foundContainer;
            }
            else
            {
                // Getting from the room, with basic "get red stick" or "get 5 red sticks" syntax.
                itemName = numberToGet <= 0 ? actionInput.Tail : actionInput.Tail.Substring(actionInput.Tail.IndexOf(' ') + 1);
            }

            // Ensure we can find the targeted thing, from the targeted location.
            thingToGet = takeFrom.FindChild(itemName.ToLower());
            if (thingToGet == null)
            {
                return $"{takeFrom.Name} does not contain {itemName}.";
            }

            // The targeted thing must be movable for us to try to move it. (Other effects may cancel our attempts to
            // move it though.)
            movableBehavior = thingToGet.FindBehavior<MovableBehavior>();
            if (movableBehavior == null)
            {
                return $"{thingToGet.Name} does not appear to be movable.";
            }

            return null;
        }
    }
}