//-----------------------------------------------------------------------------
// <copyright file="Jump.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Shabubu
//   Date      : 5/2/2009 2:14:22 AM
//   Purpose   : Type purpose here.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>Action for the actor to jump.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("jump", CommandCategory.Temporary)]
    [ActionDescription("@@@ Temp command.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Jump : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
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
            IController sender = actionInput.Controller;
            Die die = DiceService.Instance.GetDie(3);

            // This is how you send text back to the player
            int height = die.Roll();
            sender.Write("Congrats!  You've jumped " + height + " feet!  You're special boy!  This is for testing, so George quits punching me back.");
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

            // There are currently no arguments nor situations where we expect failure.
            return null;
        }
    }
}