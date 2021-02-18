//-----------------------------------------------------------------------------
// <copyright file="Unwield.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
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

    /// <summary>An action to unwield a weapon.</summary>
    [ExportGameAction(100)]
    [ActionPrimaryAlias("unwield", CommandCategory.Item)]
    [ActionDescription("Unwield a weapon.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Unwield : GameAction
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

        private Thing itemToUnwield;

        private WieldableBehavior itemToUnwieldBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;

            itemToUnwieldBehavior.Wielder = null;

            // Remove the event handler that prevents dropping the item while wielded.
            var interceptor = itemToUnwieldBehavior.MovementInterceptor;
            itemToUnwield.Eventing.MovementRequest -= interceptor;

            var contextMessage = new ContextualString(sender.Thing, itemToUnwield.Parent)
            {
                ToOriginator = $"You unwield {itemToUnwield.Name}.",
                ToOthers = $"{sender.Thing.Name} unwields {itemToUnwield.Name}.",
            };
            var sensoryMessage = new SensoryMessage(SensoryType.Sight, 100, contextMessage);

            var unwieldEvent = new WieldUnwieldEvent(itemToUnwield, true, sender.Thing, sensoryMessage);

            sender.Thing.Eventing.OnCombatRequest(unwieldEvent, EventScope.ParentsDown);

            if (!unwieldEvent.IsCancelled)
            {
                sender.Thing.Eventing.OnCombatEvent(unwieldEvent, EventScope.ParentsDown);
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            Thing wielder = sender.Thing;

            string commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            string itemName = actionInput.Tail.Trim().ToLower();

            // First look for a matching item in inventory and make sure it can
            // be wielded. If nothing was found in inventory, look for a matching
            // wieldable item in the surrounding environment.
            itemToUnwield = wielder.FindChild(item => item.Name.ToLower() == itemName && item.HasBehavior<WieldableBehavior>() && item.Behaviors.FindFirst<WieldableBehavior>().Wielder == sender.Thing);

            if (itemToUnwield == null)
            {
                itemToUnwield = wielder.Parent.FindChild(item => item.Name.ToLower() == itemName && item.HasBehavior<WieldableBehavior>() && item.Behaviors.FindFirst<WieldableBehavior>().Wielder == sender.Thing);
            }

            if (itemToUnwield == null)
            {
                return "You are not wielding the " + itemName + ".";
            }

            itemToUnwieldBehavior = itemToUnwield.Behaviors.FindFirst<WieldableBehavior>();

            return null;
        }
    }
}
