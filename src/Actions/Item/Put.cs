//-----------------------------------------------------------------------------
// <copyright file="Put.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A script to allow the moving of items from an inventory to a container.
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

    /// <summary>A command for moving items from an inventory to a container.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("put", CommandCategory.Item)]
    [ActionDescription("Put an object into a container.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Put : GameAction
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

        /// <summary>The thing we are to 'put'.</summary>
        private Thing thing = null;

        /// <summary>The new parent we are to 'put' item(s) into.</summary>
        private Thing newParent = null;

        /// <summary>The quantity of the item to 'put'.</summary>
        private int numberToPut = 0;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // @@@ TODO: Test, may be broken now... especially for only putting SOME of a stack...
            this.thing.Parent.Remove(this.thing);
            this.newParent.Add(this.thing);

            Thing cont = (Thing)this.newParent;
            cont.Save();

            this.thing.Save();

            // Bengecko - Reworked after reading more design docs on forums
            string message = string.Format("You put {0} ({1}) in to the {2} ({3})", this.thing.FullName, this.thing.ID, cont.Name, cont.ID);
            actionInput.Controller.Write(message);
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

            // Rule: We need at least 3 parameters.
            if (actionInput.Params.Length < 2)
            {
                return "The correct syntax is put <amount> [item] in [container].";
            }

            // Rule: Is the first parameter numeric?
            int.TryParse(actionInput.Params[0], out this.numberToPut);
            int itemParam = 0;
            string itemName = string.Empty;

            // Rule: If we have an amount then we shift our itemParam along one.
            if (this.numberToPut > 0)
            {
                itemParam = 1;
            }

            // Rule: Did the initiator supply a good command
            //       IE put object in container.
            if (!actionInput.Tail.ToLower().Contains("in") && !actionInput.Tail.ToLower().Contains("into"))
            {
                return "The correct syntax is put <amount> [item] in [container].";
            }

            // Find the "in" keyword in the params.
            int itemMarker = 0;
            for (int i = 0; i < actionInput.Params.Length; i++)
            {
                if (actionInput.Params[i].ToLower() == "in" || actionInput.Params[i].ToLower() == "into")
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
            string containerName = string.Empty;
            for (int i = itemMarker + 1; i < actionInput.Params.Length; i++)
            {
                containerName += actionInput.Params[i] + ' ';
            }

            containerName = containerName.Trim();

            // Rule: Do we have an item matching the one specified in our inventory or otherwise 
            // local (such as in our current location)?
            Thing foundItem = sender.Thing.FindLocalThing(containerName.ToLower());
            if (foundItem == null)
            {
                return "You cannot see " + containerName + ".";
            }

            // Rule: Is the found thing capable of containing other things?
            this.newParent = foundItem;
            var containerBehavior = foundItem.Behaviors.FindFirst<ContainerBehavior>();
            if (this.newParent == null || containerBehavior == null)
            {
                return containerName + " is not able to hold " + itemName + ".";
            }

            // Rule: Is the container open?
            // @@@ If it has OpenableBehavior, is it currently opened?
            //if (((Container)foundItem).OpenState == OpenState.Closed)
            //{
            //    return containerName + " is closed.";
            //}

            // Rule: @@@ If this item has a CapacityBehavior (or maybe just ContainerBehavior), does it have room left?
            
            // Rule: Do we have a matching item in our inventory?
            this.thing = sender.Thing.Children.Find(i => i.Name == itemName.ToLower());
            if (this.thing == null)
            {
                return "You do not hold " + itemName + ".";
            }

            return null;
        }
    }
}