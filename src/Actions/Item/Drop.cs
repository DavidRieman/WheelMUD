//-----------------------------------------------------------------------------
// <copyright file="Drop.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command to drop an object from the player inventory to the room.
//   Created: January 2007 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to drop an object from the player inventory to the room.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("drop", CommandCategory.Item)]
    [ActionDescription("Drop an object from your inventory.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    internal class Drop : GameAction
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

        /// <summary>The thing that we are to 'drop'.</summary>
        private Thing thingToDrop = null;

        /// <summary>The movable behavior of the thing we are to 'drop'.</summary>
        private MovableBehavior movableBehavior;

        /// <summary>The quantity of the item that we are to 'drop'.</summary>
        private int numberToDrop = 0;

        /// <summary>The current place that the player is within.</summary>
        private Thing dropLocation = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Generate an item changed owner event.
            IController sender = actionInput.Controller;

            var contextMessage = new ContextualString(sender.Thing, this.thingToDrop.Parent)
            {
                ToOriginator = "You drop up $Thing.Name.",
                ToReceiver = "$ActiveThing.Name drops $Thing.Name in you.",
                ToOthers = "$ActiveThing.Name drops $Thing.Name.",
            };
            var dropMessage = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            var actor = actionInput.Controller.Thing;
            if (this.movableBehavior.Move(this.dropLocation, actor, null, dropMessage))
            {
                actor.Save();
                this.dropLocation.Save();
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

            string targetName = actionInput.Tail.Trim().ToLower();

            // Rule: if 2 params
            if (actionInput.Params.Length == 2)
            {
                int.TryParse(actionInput.Params[0], out this.numberToDrop);

                if (this.numberToDrop == 0)
                {
                    targetName = actionInput.Tail.ToLower();
                }
                else
                {
                    targetName = actionInput.Tail.Remove(0, actionInput.Params[0].Length).Trim().ToLower();
                }
            }

            // Rule: Did the initiator look to drop something?
            if (targetName == string.Empty)
            {
                return "What did you want to drop?";
            }

            this.dropLocation = sender.Thing.Parent;

            // Rule: Is the target an item in the entity's inventory?
            this.thingToDrop = sender.Thing.Children.Find(t => t.Name.Equals(targetName, StringComparison.CurrentCultureIgnoreCase));
            if (this.thingToDrop == null)
            {
                return "You do not hold " + targetName + ".";
            }

            // Rule: The target thing must be movable.
            this.movableBehavior = this.thingToDrop.Behaviors.FindFirst<MovableBehavior>();
            if (this.movableBehavior == null)
            {
                return this.thingToDrop.Name + " does not appear to be movable.";
            }

            return null;
        }
    }
}