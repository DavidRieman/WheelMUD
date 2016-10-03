//-----------------------------------------------------------------------------
// <copyright file="Fill.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to fill a liquid container from a large source of liquid.
//   @@@ TODO: Implement.  For example, from fountains or pools in the room.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;
    using WheelMUD.Universe;

    /// <summary>An action to fill a liquid container from a large source of liquid.</summary>
    [ExportGameAction]
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
            if (this.sourceContainer != null && this.destinationContainer != null)
            {
                IController sender = actionInput.Controller;
                var sourceHoldsLiquidBehavior = this.sourceContainer.Behaviors.FindFirst<HoldsLiquidBehavior>();
                var destinationHoldsLiquidBehavior = this.destinationContainer.Behaviors.FindFirst<HoldsLiquidBehavior>();
                if (sourceHoldsLiquidBehavior == null)
                {
                    sender.Write("The source does not hold any liquid.");
                    return;
                }
                else if (destinationHoldsLiquidBehavior == null)
                {
                    sender.Write("The destination cannot hold any liquid.");
                    return;
                }

                // @@@ TODO: Get and display more details from FillFrom about failure cases?
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
            this.parent = sender.Thing.Parent;

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
                    this.destinationContainerName += actionInput.Params[j] + ' ';
                }

                this.destinationContainerName = this.destinationContainerName.Trim();

                // Source Container name is everything from the marker to the end.
                this.sourceContainerName = string.Empty;
                for (int i = itemMarker + 1; i < actionInput.Params.Length; i++)
                {
                    this.sourceContainerName += actionInput.Params[i] + ' ';
                }

                this.sourceContainerName = this.sourceContainerName.Trim();

                // Rule: Do we have an item matching the one specified in our inventory?
                // If not then does the room have a container with the name.
                // @@@ This Find pattern is probably broken...
                this.destinationContainer = sender.Thing.Children.Find(t => t.Name == this.destinationContainerName.ToLower());
                if (this.destinationContainer == null)
                {
                    this.destinationContainer = this.parent.Children.Find(t => t.Name == this.destinationContainerName.ToLower());
                    if (this.destinationContainer == null)
                    {
                        return "You cannot see " + this.destinationContainerName;
                    }
                }

                // Rule: Is the item specified as a container actually a container?
                this.containerBehavior = this.destinationContainer.Behaviors.FindFirst<ContainerBehavior>();
                if (this.containerBehavior == null)
                {
                    return this.destinationContainerName + " is not able to hold things.";
                }
                else
                {
                    var holdsLiquidBehavior = this.destinationContainer.Behaviors.FindFirst<HoldsLiquidBehavior>();
                    if (holdsLiquidBehavior == null)
                    {
                        return string.Format(
                            "It does not appear that the {0} will hold liquid.",
                            this.destinationContainerName);
                    }
                }

                // Rule: Is the item open?
                var openableBehavior = this.destinationContainer.Behaviors.FindFirst<OpensClosesBehavior>();
                if (openableBehavior != null && !openableBehavior.IsOpen)
                {
                    return string.Format("You cannot fill the {0} as it is closed.", this.destinationContainerName);
                }

                // Rule: Do we have an item matching the one specified in our inventory?
                // If not then does the room have a container with the name.
                // @@@ This search method is probably broken.
                this.sourceContainer = sender.Thing.Children.Find(t => t.Name == this.sourceContainerName.ToLower());
                if (this.sourceContainer == null)
                {
                    this.sourceContainer = this.parent.Children.Find(t => t.Name == this.sourceContainerName.ToLower());

                    if (this.sourceContainer == null)
                    {
                        return "You cannot see " + this.sourceContainerName;
                    }
                }

                // Rule: Is the item specified as a container actually a container?
                ContainerBehavior containerBehavior = this.sourceContainer.Behaviors.FindFirst<ContainerBehavior>();
                if (containerBehavior == null)
                {
                    return string.Format("The {0} does not hold anything to fill the {1} with.", this.sourceContainerName, this.destinationContainerName);
                }

                // @@@ TODO: HoldsLiquidBehavior?

                // Rule: Is the item open?
                OpensClosesBehavior opensClosesBehavior = this.sourceContainer.Behaviors.FindFirst<OpensClosesBehavior>();
                if (!opensClosesBehavior.IsOpen)
                {
                    return string.Format("You cannot fill from the {0} as it is closed.", this.sourceContainerName);
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