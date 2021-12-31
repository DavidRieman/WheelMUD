//-----------------------------------------------------------------------------
// <copyright file="Punch.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

// TODO: FIX: The prompt immediately following taking damage does not reflect the 
// player's new health total.  Although a SetPrompt command does not exist yet, 
// one can change the player's SessionState.Prompt value via the debugger to 
// something like "[health] [maxhealth]>" to test this out.

using System;
using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Effects;

namespace WheelMUD.Actions
{
    /// <summary>A temporary command to attempt some combat.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("punch", CommandCategory.Temporary)]
    [ActionDescription("Temporary test command. Punch a target.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Punch : GameAction
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

        /// <summary>The found target object.</summary>
        private Thing target;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Send the attack event.
            var unbalanceEffect = new UnbalanceEffect
            {
                Duration = new TimeSpan(0, 0, 0, 0, 5000),
            };

            actionInput.Actor.Behaviors.Add(unbalanceEffect);

            // Set up the dice for attack and defense rolls.
            // Currently does not consider equipment, skills or stats.
            var dice = DiceService.Instance;
            var attackDie = dice.GetDie(20);
            var defenseDie = dice.GetDie(20);

            // Roll the dice to determine if the attacker hits.
            var attackRoll = attackDie.Roll();
            var defenseRoll = defenseDie.Roll();

            // Determine amount of damage, if any. Cannot exceed the target's current health.
            var targetHealth = target.Stats["HP"].Value;
            var damage = Math.Max(attackRoll - defenseRoll, 0);
            damage = Math.Min(damage, targetHealth);

            // Choose sensory messaging based on whether or not the hit landed.
            var message = CreateResultMessage(actionInput.Actor, target, attackRoll, defenseRoll, damage);

            var attackEvent = new AttackEvent(target, message, actionInput.Actor);

            // Broadcast combat requests/events to the room they're happening in.
            actionInput.Actor.Eventing.OnCombatRequest(attackEvent, EventScope.ParentsDown);
            if (!attackEvent.IsCanceled)
            {
                target.Stats["HP"].Decrease(damage, actionInput.Actor);
                actionInput.Actor.Eventing.OnCombatEvent(attackEvent, EventScope.ParentsDown);
            }
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            // Find the most appropriate matching target.
            var targetName = actionInput.Tail.Trim().ToLower();
            target = GetPlayerOrMobile(targetName);

            // Rule: Is the target an entity?
            if (target == null)
            {
                return $"You cannot see {targetName}.";
            }

            // Rule: Is the target the initiator?
            if (string.Equals(actionInput.Actor.Name, target.Name, StringComparison.CurrentCultureIgnoreCase))
            {
                return "You can't punch yourself.";
            }

            // Rule: Is the target in the same room?
            if (actionInput.Actor.Parent.Id != target.Parent.Id)
            {
                return $"You cannot see {targetName}.";
            }

            // Rule: Is the target alive?
            if (target.Stats["HP"].Value <= 0)
            {
                return $"{target.Name} is dead.";
            }

            var unbalanceEffect = actionInput.Actor.FindBehavior<UnbalanceEffect>();
            if (unbalanceEffect != null)
            {
                return $"You are too unbalanced to punch right now. Wait {unbalanceEffect.RemainingDuration.Seconds} seconds.";
            }

            return null;
        }

        /// <summary>Choose sensory messaging based on whether or not the hit landed.</summary>
        /// <param name="attacker">The Thing performing the attack.</param>
        /// <param name="target">The Thing being attacked.</param>
        /// <param name="attackRoll">Die roll for the attack.</param>
        /// <param name="defendRoll">Die roll for the defense.</param>
        /// <param name="damage">Amount of damage to be inflicted if the attack is successful.</param>
        /// <returns>A SensoryMessage describing the successful or failed attack.</returns>
        private SensoryMessage CreateResultMessage(Thing attacker, Thing target, int attackRoll, int defendRoll, int damage)
        {
            ContextualString message;

            if (attackRoll > defendRoll)
            {
                message = new ContextualString(attacker, target)
                {
                    ToOriginator = $"You punch {target.Name} for {damage} health.",
                    ToReceiver = $"{attacker.Name} punches you for {damage} health.",
                    ToOthers = $"{attacker.Name} punches {target.Name} for {damage} health.",
                };
            }
            else
            {
                message = new ContextualString(attacker, target)
                {
                    ToOriginator = $"You attempt to punch {target.Name}, but miss.",
                    ToReceiver = $"{attacker.Name} attempts to punch you, but misses.",
                    ToOthers = $"{attacker.Name} attempts to punch {target.Name}, but misses.",
                };
            }

            return new SensoryMessage(SensoryType.Sight, 100, message);
        }
    }
}