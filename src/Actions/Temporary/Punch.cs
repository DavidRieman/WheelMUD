//-----------------------------------------------------------------------------
// <copyright file="Punch.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   @@@ Temporary script to attempt some combat.
//   Created: November 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

// @@@ BUG: The prompt immediately following taking damage does not reflect the 
// player's new health total.  Although a SetPrompt command does not exist yet, 
// one can change the player's SessionState.Prompt value via the debugger to 
// something like "[health] [maxhealth]>" to test this out.

namespace WheelMUD.Actions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Effects;
    using WheelMUD.Interfaces;

    /// <summary>@@@ A temporary command to attempt some combat.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("punch", CommandCategory.Temporary)]
    [ActionDescription("@@@ Temp command.")]
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
        private Thing target = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // Send the attack event.
            IController sender = actionInput.Controller;
            UnbalanceEffect unbalanceEffect = new UnbalanceEffect()
            {
                Duration = new TimeSpan(0, 0, 0, 0, 5000),
            };

            sender.Thing.Behaviors.Add(unbalanceEffect);

            // Set up the dice for attack and defense rolls.
            // Currently does not consider equipment, skills or stats.
            DiceService dice = DiceService.Instance;
            Die attackDie = dice.GetDie(20);
            Die defenseDie = dice.GetDie(20);

            // Roll the dice to determine if the attacker hits.
            int attackRoll = attackDie.Roll();
            int defenseRoll = defenseDie.Roll();

            // Determine amount of damage, if any. Cannot exceed the target's current health.
            int targetHealth = this.target.Stats["HP"].Value;
            int damage = Math.Max(attackRoll - defenseRoll, 0);
            damage = Math.Min(damage, targetHealth);

            // Choose sensory messaging based on whether or not the hit landed.
            SensoryMessage message = this.CreateResultMessage(sender.Thing, this.target, attackRoll, defenseRoll, damage);

            var attackEvent = new AttackEvent(this.target, message, sender.Thing);

            // Broadcast combat requests/events to the room they're happening in.
            sender.Thing.Eventing.OnCombatRequest(attackEvent, EventScope.ParentsDown);
            if (!attackEvent.IsCancelled)
            {
                this.target.Stats["HP"].Decrease(damage, sender.Thing);
                sender.Thing.Eventing.OnCombatEvent(attackEvent, EventScope.ParentsDown);
            }
        }

        /// <summary>Prepare for, and determine if the command's prerequisites have been met.</summary>
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

            // Find the most appropriate matching target.
            string targetName = actionInput.Tail.Trim().ToLower();
            this.target = GameAction.GetPlayerOrMobile(targetName);

            // Rule: Is the target an entity?
            if (this.target == null)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target the initator?
            if (sender.Thing.Name.ToLower() == this.target.Name.ToLower())
            {
                return "You can't punch yourself.";
            }

            // Rule: Is the target in the same room?
            if (sender.Thing.Parent.ID != this.target.Parent.ID)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target alive?
            if (this.target.Stats["HP"].Value <= 0)
            {
                return this.target.Name + " is dead.";
            }

            var unbalanceEffect = sender.Thing.Behaviors.FindFirst<UnbalanceEffect>();
            if (unbalanceEffect != null)
            {
                return "You are too unbalanced to punch right now. Wait " + unbalanceEffect.RemainingDuration.Seconds + " seconds.";
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
                    ToOriginator = @"You punch $ActiveThing.Name for $Damage health.",
                    ToReceiver = @"$Aggressor.Name punches you for $Damage health.",
                    ToOthers = @"$Aggressor.Name punches $ActiveThing.Name for $Damage health.",
                };
            }
            else
            {
                message = new ContextualString(attacker, target)
                {
                    ToOriginator = @"You attempt to punch $ActiveThing.Name, but miss.",
                    ToReceiver = @"$Aggressor.Name attempts to punch you, but misses.",
                    ToOthers = @"$Aggressor.Name attempts to punch $ActiveThing.Name, but misses.",
                };
            }
            
            return new SensoryMessage(SensoryType.Sight, 100, message, new Hashtable { { "Damage", damage } });
        }
    }
}