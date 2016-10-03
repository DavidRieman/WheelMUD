//-----------------------------------------------------------------------------
// <copyright file="Get.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that allows a player to get items from the room they are in or from a 
//   container within their inventory.
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

    /// <summary>Action to pick something up from the room, or move something from a container within their inventory to their inventory.</summary>
    [ExportGameAction]
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
        private int numberToGet = 0;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Remove the item from its current container.
            // We have to do this before we attempt to add it because of the event subscriptions.
            // @@@ TODO: Test, this may be broken now...
            var actor = actionInput.Controller.Thing;
            if (this.numberToGet <= 0)
            {
                this.numberToGet = 1;
            }

            // @@@ TODO: Prevent item duplication from specifying large numbers, or races for same item, etc.
            // @@@ TODO: Fix Implementation of numberToGet
            var contextMessage = new ContextualString(actor, this.thingToGet.Parent)
            {
                ToOriginator = "You pick up $Thing.Name.",
                ToReceiver = "$ActiveThing.Name takes $Thing.Name from you.",
                ToOthers = "$ActiveThing.Name picks up $Thing.Name.",
            };
            var getMessage = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            if (this.movableBehavior.Move(actor, actor, getMessage, null))
            {
                actor.Save();
                actor.Parent.Save();
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
            // @@@ Is TryParse meant to be used this way? Character analysis may be better. I worry that
            //     this might be throwing a caught exception upon each fail, which is a typical case here.
            int.TryParse(actionInput.Params[0], out this.numberToGet);

            int itemParam = 0;
            string itemName = string.Empty;

            // Rule: If we have to get a number of something, shunt up the positions of our other params.
            if (this.numberToGet > 0)
            {
                itemParam = 1;
            }

            // Rule: is the player using the command to get something from a container
            //       or to get something from the room?
            Thing targetParent = sender.Thing.Parent;
            if (actionInput.Tail.ToLower().Contains("from"))
            {
                // Find the from keyword in the params.
                int itemMarker = 0;
                for (int i = 0; i < actionInput.Params.Length; i++)
                {
                    if (actionInput.Params[i].ToLower() == "from")
                    {
                        itemMarker = i;
                    }
                }

                // Item name is everything from number (if present) to the from marker.
                for (int j = itemParam; j < itemMarker; j++)
                {
                    itemName += actionInput.Params[j] + ' ';
                }

                itemName = itemName.Trim();

                // Container name is everything from the marker to the end.
                string targetFromName = string.Empty;
                for (int i = itemMarker + 1; i < actionInput.Params.Length; i++)
                {
                    targetFromName += actionInput.Params[i] + ' ';
                }

                targetFromName = targetFromName.Trim();

                // Rule: Do we have an item matching the one specified in our inventory?
                // If not then does the room have a container with the name.
                Thing foundContainer = sender.Thing.FindChild(targetFromName.ToLower());
                if (foundContainer == null)
                {
                    foundContainer = targetParent.FindChild(targetFromName.ToLower());

                    if (foundContainer == null)
                    {
                        return string.Format("You cannot see {0}.", targetFromName);
                    }
                }

                // Rule: Is the 'from' thing specified as a container actually a container?
                ContainerBehavior containerBehavior = foundContainer.Behaviors.FindFirst<ContainerBehavior>();
                if (containerBehavior == null)
                {
                    return string.Format("{0} is not able to hold {1}.", foundContainer.Name, itemName);
                }

                // @@@ Removed OpensClosesBehavior check here... Test to ensure that 'get' is blocked by the
                //     OpensClosesBehavior receiving and cancelling the relevant events and message is good...

                targetParent = foundContainer;
            }
            else
            {
                // From the room.
                // Item name is everything from number (if present) to the from marker.
                for (int j = itemParam; j < actionInput.Params.Length; j++)
                {
                    itemName += actionInput.Params[j] + ' ';
                }

                itemName = itemName.Trim();
            }

            // Rule: Do we have an item matching in the container?
            this.thingToGet = targetParent.FindChild(itemName.ToLower());
            if (this.thingToGet == null)
            {
                return string.Format("{0} does not contain {1}.", targetParent.Name, itemName);
            }

            // Rule: The targeted thing must be movable.
            this.movableBehavior = this.thingToGet.Behaviors.FindFirst<MovableBehavior>();
            if (this.movableBehavior == null)
            {
                return this.thingToGet.Name + " does not appear to be movable.";
            }
            
            return null;
        }
    }
}