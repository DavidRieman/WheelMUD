//-----------------------------------------------------------------------------
// <copyright file="Jump.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>Action for the actor to jump.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("jump", CommandCategory.Temporary)]
    [ActionDescription("Temporary test command. Jump.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Jump : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new()
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeBalanced,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeMobile
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var die = DiceService.Instance.GetDie(3);

            // This is how you send text back to the player
            var height = die.Roll();
            actionInput.Session?.WriteLine($"You've jumped {height} feet!");
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            return VerifyCommonGuards(actionInput, ActionGuards);
        }
    }
}