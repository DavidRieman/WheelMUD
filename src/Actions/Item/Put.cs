//-----------------------------------------------------------------------------
// <copyright file="Put.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Server;
using WheelMUD.Universe;

namespace WheelMUD.Actions
{
    /// <summary>A command for moving items from an inventory to a container.</summary>
    [ExportGameAction(0)]
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
        private Thing thing;

        /// <summary>The new parent we are to 'put' item(s) into.</summary>
        private Thing newParent;

        /// <summary>The quantity of the item to 'put'.</summary>
        private int numberToPut;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // TODO: Move item from one owner to another transactionally, if applicable.
            // TODO: Test, may be broken now... especially for only putting SOME of a stack...
            thing.Parent.Remove(thing);
            newParent.Add(thing);

            actionInput.Controller.Write(new OutputBuilder().AppendLine($"You put {thing.FullName} in {newParent.Name}."));
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

            // Rule: We need at least 3 parameters.
            if (actionInput.Params.Length < 2)
            {
                return "The correct syntax is put <amount> [item] in [container].";
            }

            // Rule: Is the first parameter numeric?
            int.TryParse(actionInput.Params[0], out numberToPut);
            var itemParam = 0;
            var itemName = string.Empty;

            // Rule: If we have an amount then we shift our itemParam along one.
            if (numberToPut > 0)
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
            var itemMarker = 0;
            for (var i = 0; i < actionInput.Params.Length; i++)
            {
                if (actionInput.Params[i].ToLower() == "in" || actionInput.Params[i].ToLower() == "into")
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
            var containerName = string.Empty;
            for (var i = itemMarker + 1; i < actionInput.Params.Length; i++)
            {
                containerName += actionInput.Params[i] + ' ';
            }

            containerName = containerName.Trim();

            // Rule: Do we have an item matching the one specified in our inventory or otherwise 
            // local (such as in our current location)?
            var foundItem = actionInput.Controller.Thing.FindLocalThing(containerName.ToLower());
            if (foundItem == null)
            {
                return $"You cannot see {containerName}.";
            }

            // Rule: Is the found thing capable of containing other things?
            newParent = foundItem;
            var containerBehavior = foundItem.Behaviors.FindFirst<ContainerBehavior>();
            if (newParent == null || containerBehavior == null)
            {
                return $"{containerName} is not able to hold {itemName}.";
            }

            // Rule: Is the container open?
            // TODO: If it has OpenableBehavior, is it currently opened?
            //if (((Container)foundItem).OpenState == OpenState.Closed)
            //{
            //    return containerName + " is closed.";
            //}

            // TODO: Rule: If this item has a CapacityBehavior (or maybe just ContainerBehavior), does it have room left?

            // Rule: Do we have a matching item in our inventory?
            thing = actionInput.Controller.Thing.Children.Find(i => i.Name == itemName.ToLower());
            if (thing == null)
            {
                return $"You do not hold {itemName}.";
            }

            return null;
        }
    }
}