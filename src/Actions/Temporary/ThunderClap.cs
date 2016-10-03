//-----------------------------------------------------------------------------
// <copyright file="ThunderClap.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Shabubu
//   Date      : 5/8/2009 2:21:51 PM
//   Purpose   : Type purpose here.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Events;
    using WheelMUD.Effects;
    using WheelMUD.Interfaces;

    [ExportGameAction]
    [ActionPrimaryAlias("ThunderClap", CommandCategory.Temporary)]
    [ActionDescription("@@@ Temp command.")]
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
        private Thing target = null;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            var contextMessage = new ContextualString(sender.Thing, this.target)
            {
                ToOriginator = "You cast ThunderClap at $ActiveThing.Name!",
                ToReceiver = "$Aggressor.Name casts ThunderClap at you, you only hear a ringing in your ears now.",
                ToOthers = "You hear $Aggressor.Name cast ThunderClap at $ActiveThing.Name!  It was very loud.",
            }; 
            var sm = new SensoryMessage(SensoryType.Hearing, 100, contextMessage);

            var attackEvent = new AttackEvent(this.target, sm, sender.Thing);
            sender.Thing.Eventing.OnCombatRequest(attackEvent, EventScope.ParentsDown);
            if (!attackEvent.IsCancelled)
            {
                var deafenEffect = new AlterSenseEffect()
                {
                    SensoryType = SensoryType.Hearing,
                    AlterAmount = -1000,
                    Duration = new TimeSpan(0, 0, 45),
                };
                
                this.target.Behaviors.Add(deafenEffect);
                sender.Thing.Eventing.OnCombatEvent(attackEvent, EventScope.ParentsDown);
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
            this.target = GameAction.GetPlayerOrMobile(targetName);
            if (this.target == null)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target in the same room?
            if (actionInput.Controller.Thing.Parent.ID != this.target.Parent.ID)
            {
                return "You cannot see " + targetName + ".";
            }

            // Rule: Is the target alive?
            if (this.target.Stats["health"].Value <= 0)
            {
                return this.target.Name + " is dead.";
            }

            return null;
        }
    }
}