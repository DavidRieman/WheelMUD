//-----------------------------------------------------------------------------
// <copyright file="Wield.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to wield a weapon in your primary hand.
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Actions
{
    using System.Collections.Generic;
    using WarriorRogueMage.Behaviors;
    using WheelMUD.Actions;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>An action to wield a weapon in your primary hand.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("wield", CommandCategory.Item)]
    [ActionDescription("Wield a weapon in your primary hand.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Wield : GameAction
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

        private Thing itemToWield;

        private WieldableBehavior itemToWieldBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;

            this.itemToWieldBehavior.Wielder = sender.Thing;

            // Create an event handler that intercepts the ChangeOwnerEvent and
            // prevents dropping/trading the item around while it is wielded.
            // A reference is stored in the WieldableBehavior instance so it
            // can be easily removed by the unwield command.
            var interceptor = new CancellableGameEventHandler(this.Eventing_MovementRequest);
            this.itemToWieldBehavior.MovementInterceptor = interceptor;
            this.itemToWield.Eventing.MovementRequest += interceptor;

            var contextMessage = new ContextualString(sender.Thing, this.itemToWield.Parent)
            {
                ToOriginator = "You wield the $WieldedItem.Name.",
                ToOthers = "$ActiveThing.Name wields a $WieldedItem.Name.",
            };

            var sensoryMessage = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            var wieldEvent = new WieldUnwieldEvent(this.itemToWield, true, sender.Thing, sensoryMessage);

            sender.Thing.Eventing.OnCombatRequest(wieldEvent, EventScope.ParentsDown);

            if (!wieldEvent.IsCancelled)
            {
                sender.Thing.Eventing.OnCombatEvent(wieldEvent, EventScope.ParentsDown);
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            IController sender = actionInput.Controller;
            Thing wielder = sender.Thing;
            Thing room = wielder.Parent;

            string itemName = actionInput.Tail.Trim().ToLower();

            // First look for a matching item in inventory and make sure it can
            // be wielded. If nothing was found in inventory, look for a matching
            // wieldable item in the surrounding environment.
            Thing itemInInventory = wielder.FindChild(itemName);

            if (itemInInventory != null)
            {
                this.itemToWieldBehavior = itemInInventory.Behaviors.FindFirst<WieldableBehavior>();

                // Item was found in inventory, but it cannot be wielded.
                if (this.itemToWieldBehavior == null)
                {
                    return "This item cannot be wielded!";
                }

                this.itemToWield = itemInInventory;
            }
            else
            {
                Thing itemInRoom = room.FindChild(itemName);

                // Item was not found in inventory or the room.
                if (itemInRoom == null)
                {
                    return "Unable to find: " + itemName;
                }

                this.itemToWieldBehavior = itemInRoom.Behaviors.FindFirst<WieldableBehavior>();

                // Item was found in the room, but it cannot be wielded.
                if (this.itemToWieldBehavior == null)
                {
                    return "This item cannot be wielded!";
                }

                // Item was found in the room, but it must be picked up first.
                if (this.itemToWieldBehavior.MustBeHeld)
                {
                    return "You are not holding the " + itemInRoom.FullName + ".";
                }

                this.itemToWield = itemInRoom;
            }

            // Make sure the item is not already wielded by someone else.
            // This shouldn't happen (famous last words) if the item is in
            // inventory, but it could happen with stationary wieldable items
            // in the room.
            if (this.itemToWieldBehavior.Wielder != null)
            {
                return string.Format("The {0} is already wielded by {1}.", this.itemToWield.Name, this.itemToWieldBehavior.Wielder.FullName);
            }

            return null;
        }

        /// <summary>Intercepts and cancels a movement request for the wielded item.</summary>
        /// <remarks>This way a player will not accidentally drop a wielded weapon, etc.</remarks>
        /// <param name="root">The root.</param>
        /// <param name="e">The event.</param>
        private void Eventing_MovementRequest(Thing root, CancellableGameEvent e)
        {
            var evt = e as ChangeOwnerEvent;
            if (evt != null)
            {
                if (evt.Thing.ID == this.itemToWield.ID)
                {
                    evt.Cancel(string.Format("The {0} is still wielded!", this.itemToWield.Name));
                }
            }
        }
    }
}
