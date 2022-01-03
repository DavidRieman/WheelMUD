//-----------------------------------------------------------------------------
// <copyright file="Wield.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WarriorRogueMage.Behaviors;
using WheelMUD.Core;

namespace WarriorRogueMage.Actions
{
    /// <summary>An action to wield a weapon in your primary hand.</summary>
    [CoreExports.GameAction(100)]
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
            Thing wielder = actionInput.Actor;
            itemToWieldBehavior.Wielder = wielder;

            // Create an event handler that intercepts the ChangeOwnerEvent and
            // prevents dropping/trading the item around while it is wielded.
            // A reference is stored in the WieldableBehavior instance so it
            // can be easily removed by the unwield command.
            var interceptor = new CancellableGameEventHandler(Eventing_MovementRequest);
            itemToWieldBehavior.MovementInterceptor = interceptor;
            itemToWield.Eventing.MovementRequest += interceptor;

            var contextMessage = new ContextualString(wielder, itemToWield.Parent)
            {
                ToOriginator = $"You wield {itemToWield.Name}.",
                ToOthers = $"{wielder.Name} wields {itemToWield.Name}.",
            };

            var sensoryMessage = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            var wieldEvent = new WieldUnwieldEvent(itemToWield, true, wielder, sensoryMessage);

            wielder.Eventing.OnCombatRequest(wieldEvent, EventScope.ParentsDown);

            if (!wieldEvent.IsCanceled)
            {
                wielder.Eventing.OnCombatEvent(wieldEvent, EventScope.ParentsDown);
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

            Thing wielder = actionInput.Actor;
            Thing room = wielder.Parent;

            string itemName = actionInput.Tail.Trim().ToLower();

            // First look for a matching item in inventory and make sure it can
            // be wielded. If nothing was found in inventory, look for a matching
            // wieldable item in the surrounding environment.
            Thing itemInInventory = wielder.FindChild(itemName);

            if (itemInInventory != null)
            {
                itemToWieldBehavior = itemInInventory.FindBehavior<WieldableBehavior>();

                // Item was found in inventory, but it cannot be wielded.
                if (itemToWieldBehavior == null)
                {
                    return "This item cannot be wielded!";
                }

                itemToWield = itemInInventory;
            }
            else
            {
                Thing itemInRoom = room.FindChild(itemName);

                // Item was not found in inventory or the room.
                if (itemInRoom == null)
                {
                    return "Unable to find: " + itemName;
                }

                itemToWieldBehavior = itemInRoom.FindBehavior<WieldableBehavior>();

                // Item was found in the room, but it cannot be wielded.
                if (itemToWieldBehavior == null)
                {
                    return "This item cannot be wielded!";
                }

                // Item was found in the room, but it must be picked up first.
                if (itemToWieldBehavior.MustBeHeld)
                {
                    return "You are not holding the " + itemInRoom.FullName + ".";
                }

                itemToWield = itemInRoom;
            }

            // Make sure the item is not already wielded by someone else.
            // This shouldn't happen (famous last words) if the item is in
            // inventory, but it could happen with stationary wieldable items
            // in the room.
            if (itemToWieldBehavior.Wielder != null)
            {
                return string.Format("The {0} is already wielded by {1}.", itemToWield.Name, itemToWieldBehavior.Wielder.FullName);
            }

            return null;
        }

        /// <summary>Intercepts and cancels a movement request for the wielded item.</summary>
        /// <remarks>This way a player will not accidentally drop a wielded weapon, etc.</remarks>
        /// <param name="root">The root.</param>
        /// <param name="e">The event.</param>
        private void Eventing_MovementRequest(Thing root, CancellableGameEvent e)
        {
            if (e is ChangeOwnerEvent changeOwnerEvent)
            {
                if (changeOwnerEvent.Thing.Id == itemToWield.Id)
                {
                    changeOwnerEvent.Cancel(string.Format("The {0} is still wielded!", itemToWield.Name));
                }
            }
        }
    }
}
