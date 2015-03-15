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
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>
    /// A command to drop an object from the player inventory to the room.
    /// </summary>
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
        private Thing thing = null;

        /// <summary>The quantity of the item that we are to 'drop'.</summary>
        private int numberToDrop = 0;

        /// <summary>The current place that the player is within.</summary>
        private Thing parent = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Generate an item changed owner event.
            IController sender = actionInput.Controller;
            ContextualStringBuilder csb = new ContextualStringBuilder(sender.Thing, this.parent);
            csb.Append(@"$ActiveThing.Name drops $Thing.Name.", ContextualStringUsage.WhenNotBeingPassedToOriginator);
            csb.Append(@"You drop $Thing.Name.", ContextualStringUsage.OnlyWhenBeingPassedToOriginator);
            SensoryMessage message = new SensoryMessage(SensoryType.Sight, 100, csb);
            var changeOwnerEvent = new ChangeOwnerEvent(sender.Thing, message, sender.Thing, this.parent, this.thing);

            // Broadcast as a request and see if anything wants to prevent the event.
            this.parent.Eventing.OnMovementRequest(changeOwnerEvent, EventScope.ParentsDown);
            if (!changeOwnerEvent.IsCancelled)
            {
                // Always have to remove an item before adding it because of the event observer system.
                // @@@ TODO: Test, this may be broken now...
                this.thing.Parent.Remove(this.thing);
                this.parent.Add(this.thing);

                //// @@@ BUG: Saving currently throws a NotImplementedException. Disabled for now...
                this.thing.Save();
                this.parent.Save();

                // Broadcast the event.
                this.parent.Eventing.OnMovementEvent(changeOwnerEvent, EventScope.ParentsDown);
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

            this.parent = sender.Thing.Parent;

            // Rule: Is the target an item in the entity's inventory?
            this.thing = sender.Thing.Children.Find(t => t.Name.Equals(targetName, StringComparison.CurrentCultureIgnoreCase));
            if (this.thing == null)
            {
                return "You do not hold " + targetName + ".";
            }

            return null;
        }
    }
}