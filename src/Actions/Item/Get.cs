//-----------------------------------------------------------------------------
// <copyright file="Get.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
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
            var actor = actionInput.Controller.Thing;
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
            // TODO: Is TryParse meant to be used this way? Character analysis may be better. I worry that
            //       this might be throwing a caught exception upon each fail, which is a typical case here.
            int.TryParse(actionInput.Params[0], out numberToGet);

            var itemParam = 0;
            var itemName = string.Empty;

            // Rule: If we have to get a number of something, shunt up the positions of our other params.
            if (numberToGet > 0)
            {
                itemParam = 1;
            }

            // Rule: is the player using the command to get something from a container
            //       or to get something from the room?
            Thing targetParent = actionInput.Controller.Thing.Parent;
            if (actionInput.Tail.ToLower().Contains("from"))
            {
                // Find the from keyword in the params.
                var itemMarker = 0;
                for (var i = 0; i < actionInput.Params.Length; i++)
                {
                    if (actionInput.Params[i].ToLower() == "from")
                    {
                        itemMarker = i;
                    }
                }

                // Item name is everything from number (if present) to the from marker.
                for (var j = itemParam; j < itemMarker; j++)
                {
                    itemName += actionInput.Params[j] + ' ';
                }

                itemName = itemName.Trim();

                // Container name is everything from the marker to the end.
                var targetFromName = string.Empty;
                for (var i = itemMarker + 1; i < actionInput.Params.Length; i++)
                {
                    targetFromName += actionInput.Params[i] + ' ';
                }

                targetFromName = targetFromName.Trim();

                // Rule: Do we have an item matching the one specified in our inventory?
                // If not then does the room have a container with the name.
                Thing foundContainer = actionInput.Controller.Thing.FindChild(targetFromName.ToLower());
                if (foundContainer == null)
                {
                    foundContainer = targetParent.FindChild(targetFromName.ToLower());

                    if (foundContainer == null)
                    {
                        return $"You cannot see {targetFromName}.";
                    }
                }

                // Rule: Is the 'from' thing specified as a container actually a container?
                var containerBehavior = foundContainer.Behaviors.FindFirst<ContainerBehavior>();
                if (containerBehavior == null)
                {
                    return $"{foundContainer.Name} is not able to hold {itemName}.";
                }

                // TODO: Removed OpensClosesBehavior check here... Test to ensure that 'get' is blocked by the
                //       OpensClosesBehavior receiving and cancelling the relevant events and message is good...

                targetParent = foundContainer;
            }
            else
            {
                // From the room.
                // Item name is everything from number (if present) to the from marker.
                for (var j = itemParam; j < actionInput.Params.Length; j++)
                {
                    itemName += actionInput.Params[j] + ' ';
                }

                itemName = itemName.Trim();
            }

            // Rule: Do we have an item matching in the container?
            thingToGet = targetParent.FindChild(itemName.ToLower());
            if (thingToGet == null)
            {
                return $"{targetParent.Name} does not contain {itemName}.";
            }

            // Rule: The targeted thing must be movable.
            movableBehavior = thingToGet.Behaviors.FindFirst<MovableBehavior>();
            if (movableBehavior == null)
            {
                return $"{thingToGet.Name} does not appear to be movable.";
            }

            return null;
        }
    }
}