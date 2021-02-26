//-----------------------------------------------------------------------------
// <copyright file="Fill.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;
    using WheelMUD.Universe;

    /// <summary>An action to fill a liquid container from a large source of liquid.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("fill", CommandCategory.Item)]
    [ActionDescription("Fill a liquid container from a large source of liquid.")]
    [ActionSecurity(SecurityRole.player)]
    public class Fill : GameAction
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

        /// <summary>The container that we are to 'empty'.</summary>
        private Thing sourceContainer = null;

        /// <summary>The name container that we are to 'empty'.</summary>
        private string sourceContainerName = null;

        /// <summary>The percentage of the item that we are to 'drop'.</summary>
        /// bengecko - Concept only at this point
        ////private double percentToEmpty = 0;

        /// <summary>The current room that the player is within.</summary>
        private Thing parent = null;

        /// <summary>The name of the container that you want to tranfer data to.</summary>
        private string destinationContainerName = null;

        /// <summary>The container that you want to tranfer data to.</summary>
        private Thing destinationContainer = null;

        private ContainerBehavior containerBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (sourceContainer != null && destinationContainer != null)
            {
                IController sender = actionInput.Controller;
                var sourceHoldsLiquidBehavior = sourceContainer.Behaviors.FindFirst<HoldsLiquidBehavior>();
                var destinationHoldsLiquidBehavior = destinationContainer.Behaviors.FindFirst<HoldsLiquidBehavior>();
                if (sourceHoldsLiquidBehavior == null)
                {
                    sender.Write("The source does not hold any liquid.");
                    return;
                }

                if (destinationHoldsLiquidBehavior == null)
                {
                    sender.Write("The destination cannot hold any liquid.");
                    return;
                }

                // TODO: Get and display more details from FillFrom about failure cases?
                destinationHoldsLiquidBehavior.FillFrom(sender, sourceHoldsLiquidBehavior);
            }
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

            int itemParam = 0;
            parent = sender.Thing.Parent;

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

                // Destination container
                for (int j = itemParam; j < itemMarker; j++)
                {
                    destinationContainerName += actionInput.Params[j] + ' ';
                }

                destinationContainerName = destinationContainerName.Trim();

                // Source Container name is everything from the marker to the end.
                sourceContainerName = string.Empty;
                for (int i = itemMarker + 1; i < actionInput.Params.Length; i++)
                {
                    sourceContainerName += actionInput.Params[i] + ' ';
                }

                sourceContainerName = sourceContainerName.Trim();

                // Rule: Do we have an item matching the one specified in our inventory?
                // If not then does the room have a container with the name.
                // TODO: Fix: This Find pattern is probably broken...
                destinationContainer = sender.Thing.Children.Find(t => t.Name == destinationContainerName.ToLower());
                if (destinationContainer == null)
                {
                    destinationContainer = parent.Children.Find(t => t.Name == destinationContainerName.ToLower());
                    if (destinationContainer == null)
                    {
                        return $"You cannot see {destinationContainerName}.";
                    }
                }

                // Rule: Is the item specified as a container actually a container?
                this.containerBehavior = destinationContainer.Behaviors.FindFirst<ContainerBehavior>();
                if (this.containerBehavior == null)
                {
                    return $"{destinationContainerName} is not able to hold things.";
                }

                var holdsLiquidBehavior = destinationContainer.Behaviors.FindFirst<HoldsLiquidBehavior>();
                if (holdsLiquidBehavior == null)
                {
                    return $"It does not appear that the {destinationContainerName} will hold liquid.";
                }

                // Rule: Is the item open?
                var openableBehavior = destinationContainer.Behaviors.FindFirst<OpensClosesBehavior>();
                if (openableBehavior is {IsOpen: false})
                {
                    return $"You cannot fill the {destinationContainerName} as it is closed.";
                }

                // Rule: Do we have an item matching the one specified in our inventory?
                // If not then does the room have a container with the name.
                // TODO: Investigate; This search method is probably broken.
                sourceContainer = sender.Thing.Children.Find(t => t.Name == sourceContainerName.ToLower());
                if (sourceContainer == null)
                {
                    sourceContainer = parent.Children.Find(t => t.Name == sourceContainerName.ToLower());

                    if (sourceContainer == null)
                    {
                        return "You cannot see " + sourceContainerName;
                    }
                }

                // Rule: Is the item specified as a container actually a container?
                ContainerBehavior containerBehavior = sourceContainer.Behaviors.FindFirst<ContainerBehavior>();
                if (containerBehavior == null)
                {
                    return $"The {sourceContainerName} does not hold anything to fill the {destinationContainerName} with.";
                }

                // TODO: HoldsLiquidBehavior?

                // Rule: Is the item open?
                OpensClosesBehavior opensClosesBehavior = sourceContainer.Behaviors.FindFirst<OpensClosesBehavior>();
                if (!opensClosesBehavior.IsOpen)
                {
                    return $"You cannot fill from the {sourceContainerName} as it is closed.";
                }
            }
            else
            {
                return "You must use [from] to specify the source, as in, [fill wine skin from fountain].";
            }

            return null;
        }
    }
}