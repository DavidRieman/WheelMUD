//-----------------------------------------------------------------------------
// <copyright file="Empty.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to empty a liquid container.
//   @@@ TODO: Implement.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;
    using WheelMUD.Universe;

    /// <summary>An action to empty a liquid container.</summary>
    [ExportGameAction]
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
        private Thing sourceContainer = null;

        /// <summary>The parent that you want to tranfer data to.</summary>
        private Thing destinationParent = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            if (this.sourceContainer == null || this.sourceContainer.Count <= 0 || this.destinationParent == null)
            {
                return;
            }

            // Dump each child out of the targeted container.
            List<string> movedThingNames = new List<string>();
            foreach (Thing thing in this.sourceContainer.Children)
            {
                var movableBehavior = thing.Behaviors.FindFirst<MovableBehavior>();
                if (movableBehavior != null)
                {
                    // Try to move the item without any messaging since we'll be sending a bulk message later.
                    if (movableBehavior.Move(this.destinationParent, sender.Thing, null, null))
                    {
                        movedThingNames.Add(thing.Name);
                    }
                }
            }

            string commaSeparatedList = BuildCommaSeparatedList(movedThingNames);
            var contextMessage = new ContextualString(sender.Thing, this.destinationParent)
            {
                ToOriginator = string.Format("You move {0} from {1} into {2}", commaSeparatedList, this.sourceContainer.Name, this.destinationParent.Name),
                ToReceiver = string.Format("$ActiveThings.Name moves {0} from {1} into you.", commaSeparatedList, this.sourceContainer.Name),
                ToOthers = string.Format("$ActiveThings.Name moves {0} from {1} into {2}.", commaSeparatedList, this.sourceContainer.Name, this.destinationParent.Name),
            };
            var message = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            var bulkMovementEvent = new BulkMovementEvent(sender.Thing, message);
            sender.Thing.Eventing.OnMovementEvent(bulkMovementEvent, EventScope.ParentsDown);
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

            // Break out the specified parameters into their theoretical meaning.
            string sourceContainerName = actionInput.Params[0];
            string destinationParentName = null;
            if (actionInput.Params.Length > 1)
            {
                // @@@ Hmm, maybe action input should have a means of automatically stripping words like this or "the" etc?
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

            // Rule: The target must be an item in the command sender's inventory.
            Thing thing = sender.Thing.Children.Find(t => t.Name.Equals(sourceContainerName, StringComparison.CurrentCultureIgnoreCase));
            if (thing == null)
            {
                return string.Format("You do not hold {0}.", sourceContainerName);
            }

            // Rule: The targeted thing must be a container of some sort.
            ContainerBehavior containerBehavior = thing.Behaviors.FindFirst<ContainerBehavior>();
            if (containerBehavior == null)
            {
                return string.Format("The {0} is not a container.", thing.Name);
            }

            // Rule: The targeted container must not be empty already.
            this.sourceContainer = thing;
            if (this.sourceContainer.Children.Count == 0)
            {
                return string.Format("The {0} is already empty.", this.sourceContainer.Name);
            }

            // @@@ Not possible? If so, default to the current container's parent instead of failing?
            Debug.Assert(!string.IsNullOrEmpty(destinationParentName));

            if (destinationParentName.Equals("ground", StringComparison.CurrentCultureIgnoreCase) ||
                destinationParentName.Equals("out", StringComparison.CurrentCultureIgnoreCase))
            {
                // @@@ TODO: Test, this may be broken...
                this.destinationParent = sender.Thing.Parent;
            }
            else
            {
                // @@@ TODO: Allow targeting of containers in same place, like chests and whatnot?
                Thing destinationThing = sender.Thing.Children.Find(t => t.Name == destinationParentName.ToLower());
                if (destinationThing == null)
                {
                    return string.Format("You do not hold {0}.", destinationParentName);
                }

                containerBehavior = thing.Behaviors.FindFirst<ContainerBehavior>();
                if (containerBehavior == null)
                {
                    return string.Format("{0} is not a container.", destinationParentName);
                }

                this.destinationParent = thing.Parent;
            }

            return null;
        }

        // @@@ Test and move to a string utilities class or extension method of some sort?
        private static string BuildCommaSeparatedList(IEnumerable<string> things)
        {
            string result = string.Empty;
            int countLeft = things.Count();
            foreach (string thing in things)
            {
                countLeft--;
                result += thing;
                if (countLeft > 1)
                {
                    result += ", ";
                } 
                else if (countLeft == 1)
                {
                    result += ", and ";
                }
            }

            return result;
        }
    }
}