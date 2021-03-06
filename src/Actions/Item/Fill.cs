﻿//-----------------------------------------------------------------------------
// <copyright file="Fill.cs" company="WheelMUD Development Team">
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
        private Thing sourceContainer;

        /// <summary>The name container that we are to 'empty'.</summary>
        private string sourceContainerName;

        /// <summary>The percentage of the item that we are to 'drop'.</summary>
        /// bengecko - Concept only at this point
        ////private double percentToEmpty = 0;

        /// <summary>The current room that the player is within.</summary>
        private Thing parent;

        /// <summary>The name of the container that you want to tranfer data to.</summary>
        private string destinationContainerName;

        /// <summary>The container that you want to tranfer data to.</summary>
        private Thing destinationContainer;

        private ContainerBehavior containerBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (sourceContainer != null && destinationContainer != null)
            {
                var sourceHoldsLiquidBehavior = sourceContainer.Behaviors.FindFirst<HoldsLiquidBehavior>();
                var destinationHoldsLiquidBehavior = destinationContainer.Behaviors.FindFirst<HoldsLiquidBehavior>();
                if (sourceHoldsLiquidBehavior == null)
                {
                    actionInput.Controller.Write(new OutputBuilder().
                        AppendLine("The source does not hold any liquid."));
                    return;
                }

                if (destinationHoldsLiquidBehavior == null)
                {
                    actionInput.Controller.Write(new OutputBuilder().
                        AppendLine("The destination cannot hold any liquid."));
                    return;
                }

                // TODO: Get and display more details from FillFrom about failure cases?
                destinationHoldsLiquidBehavior.FillFrom(actionInput.Controller, sourceHoldsLiquidBehavior);
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

            var itemParam = 0;
            parent = actionInput.Controller.Thing.Parent;

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

                // Destination container
                for (var j = itemParam; j < itemMarker; j++)
                {
                    destinationContainerName += actionInput.Params[j] + ' ';
                }

                destinationContainerName = destinationContainerName.Trim();

                // Source Container name is everything from the marker to the end.
                sourceContainerName = string.Empty;
                for (var i = itemMarker + 1; i < actionInput.Params.Length; i++)
                {
                    sourceContainerName += actionInput.Params[i] + ' ';
                }

                sourceContainerName = sourceContainerName.Trim();

                // Rule: Do we have an item matching the one specified in our inventory?
                // If not then does the room have a container with the name.
                // TODO: Fix: This Find pattern is probably broken...
                destinationContainer = actionInput.Controller.Thing.Children.Find(t => t.Name == destinationContainerName.ToLower());
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
                sourceContainer = actionInput.Controller.Thing.Children.Find(t => t.Name == sourceContainerName.ToLower());
                if (sourceContainer == null)
                {
                    sourceContainer = parent.Children.Find(t => t.Name == sourceContainerName.ToLower());

                    if (sourceContainer == null)
                    {
                        return "You cannot see " + sourceContainerName;
                    }
                }

                // Rule: Is the item specified as a container actually a container?
                var containerBehavior = sourceContainer.Behaviors.FindFirst<ContainerBehavior>();
                if (containerBehavior == null)
                {
                    return
                        $"The {sourceContainerName} does not hold anything to fill the {destinationContainerName} with.";
                }

                // TODO: HoldsLiquidBehavior?

                // Rule: Is the item open?
                var opensClosesBehavior = sourceContainer.Behaviors.FindFirst<OpensClosesBehavior>();
                if (!opensClosesBehavior.IsOpen)
                {
                    return $"You cannot fill from the {sourceContainerName} as it is closed.";
                }
            }
            else
            {
                return "You must use \"from\" to specify the source, as in, \"fill wine skin from fountain\".";
            }

            return null;
        }
    }
}