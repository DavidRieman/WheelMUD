//-----------------------------------------------------------------------------
// <copyright file="ThunderClap.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Effects;

namespace WheelMUD.Actions
{
    /// <summary>Test command to deafen a target.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("ThunderClap", CommandCategory.Temporary)]
    [ActionDescription("Temporary test command. Deafens a target.")]
    [ActionSecurity(SecurityRole.player)]
    public class ThunderClap : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.RequiresAtLeastOneArgument
        };

        /// <summary>The target of the ThunderClap action.</summary>
        private Thing target;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var contextMessage = new ContextualString(actionInput.Actor, target)
            {
                ToOriginator = $"You cast ThunderClap at {target.Name}!",
                ToReceiver = $"{actionInput.Actor.Name} casts ThunderClap at you. You only hear a ringing in your ears now.",
                ToOthers = $"You hear {actionInput.Actor.Name} cast ThunderClap at {target.Name}! It was very loud.",
            };
            var sm = new SensoryMessage(SensoryType.Hearing, 100, contextMessage);

            var attackEvent = new AttackEvent(target, sm, actionInput.Actor);
            actionInput.Actor.Eventing.OnCombatRequest(attackEvent, EventScope.ParentsDown);
            if (!attackEvent.IsCancelled)
            {
                var deafenEffect = new AlterSenseEffect()
                {
                    SensoryType = SensoryType.Hearing,
                    AlterAmount = -1000,
                    Duration = new TimeSpan(0, 0, 45),
                };

                target.Behaviors.Add(deafenEffect);
                actionInput.Actor.Eventing.OnCombatEvent(attackEvent, EventScope.ParentsDown);
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

            string targetName = actionInput.Tail.Trim().ToLower();

            // Rule: Is the target an entity?
            target = GetPlayerOrMobile(targetName);
            if (target == null)
            {
                return $"You cannot see {targetName}.";
            }

            // Rule: Is the target in the same room?
            if (actionInput.Actor.Parent.Id != target.Parent.Id)
            {
                return $"You cannot see {targetName}.";
            }

            // Rule: Is the target alive?
            if (target.Stats["health"].Value <= 0)
            {
                return $"{target.Name} is dead.";
            }

            return null;
        }
    }
}