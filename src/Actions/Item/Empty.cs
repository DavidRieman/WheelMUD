//-----------------------------------------------------------------------------
// <copyright file="Empty.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Universe;
using WheelMUD.Utilities;

namespace WheelMUD.Actions
{
    /// <summary>An action to empty a container.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("empty", CommandCategory.Item)]
    [ActionAlias("pour out", CommandCategory.Item)]
    [ActionDescription("Empties a container. Usage empty container target")]
    [ActionSecurity(SecurityRole.player)]
    public class Empty : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeMobile,
            CommonGuards.RequiresAtLeastOneArgument,
        };

        /// <summary>The container that we are to 'empty'.</summary>
        private Thing sourceContainer;

        /// <summary>The parent that you want to transfer data to.</summary>
        private Thing destinationParent;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (sourceContainer == null || sourceContainer.Count <= 0 || destinationParent == null)
            {
                return;
            }

            // Dump each child out of the targeted container.
            var movedThingNames = (from thing in sourceContainer.Children
                                   let movableBehavior =
thing.Behaviors.FindFirst<MovableBehavior>()
                                   where movableBehavior != null
                                   where movableBehavior.
Move(destinationParent, actionInput.Actor, null, null)
                                   select thing.Name).ToList();

            var commaSeparatedList = movedThingNames.BuildPrettyList();
            var contextMessage = new ContextualString(actionInput.Actor, destinationParent)
            {
                ToOriginator = $"You move {commaSeparatedList} from {sourceContainer.Name} into {destinationParent.Name}",
                ToReceiver = $"{actionInput.Actor.Name} moves {commaSeparatedList} from {sourceContainer.Name} into you.",
                ToOthers = $"{actionInput.Actor.Name} moves {commaSeparatedList} from {sourceContainer.Name} into {destinationParent.Name}.",
            };
            var message = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            var bulkMovementEvent = new BulkMovementEvent(actionInput.Actor, message);
            actionInput.Actor.Eventing.OnMovementEvent(bulkMovementEvent, EventScope.ParentsDown);
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var actor = actionInput.Actor;
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // Break out the specified parameters into their theoretical meaning.
            var sourceContainerName = actionInput.Params[0];
            string destinationParentName = null;
            if (actionInput.Params.Length > 1)
            {
                // TODO: Maybe action input should have a means of automatically stripping words like this or "the" etc?
                if (actionInput.Params.Length > 2 &&
                    (actionInput.Params[1].Equals("into", StringComparison.CurrentCultureIgnoreCase) ||
                     actionInput.Params[1].Equals("onto", StringComparison.CurrentCultureIgnoreCase)))
                {
                    destinationParentName = actionInput.Params[2];
                }
                else
                {
                    destinationParentName = actionInput.Params[1];
                }
            }

            // Rule: The target must be an item in the actor's inventory.
            var thing = actor.Children.Find(t => t.Name.Equals(sourceContainerName, StringComparison.CurrentCultureIgnoreCase));
            if (thing == null)
            {
                return $"You do not hold {sourceContainerName}.";
            }

            // Rule: The targeted thing must be a container of some sort.
            var containerBehavior = thing.Behaviors.FindFirst<ContainerBehavior>();
            if (containerBehavior == null)
            {
                return $"The {thing.Name} is not a container.";
            }

            // Rule: The targeted container must not be empty already.
            sourceContainer = thing;
            if (sourceContainer.Children.Count == 0)
            {
                return $"The {sourceContainer.Name} is already empty.";
            }

            // TODO: Test; Not possible? If so, default to the current container's parent instead of failing?
            Debug.Assert(!string.IsNullOrEmpty(destinationParentName));

            if (destinationParentName.Equals("ground", StringComparison.CurrentCultureIgnoreCase) ||
                destinationParentName.Equals("out", StringComparison.CurrentCultureIgnoreCase))
            {
                // TODO: Test, this may be broken...
                destinationParent = actor.Parent;
            }
            else
            {
                // TODO: Allow targeting of containers in same place, like chests and whatnot?
                var destinationThing = actor.Children.Find(t => t.Name == destinationParentName.ToLower());
                if (destinationThing == null)
                {
                    return $"You do not hold {destinationParentName}.";
                }

                containerBehavior = thing.Behaviors.FindFirst<ContainerBehavior>();
                if (containerBehavior == null)
                {
                    return $"{destinationParentName} is not a container.";
                }

                destinationParent = thing.Parent;
            }

            return null;
        }
    }
}