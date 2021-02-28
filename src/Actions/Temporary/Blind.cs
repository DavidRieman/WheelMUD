//-----------------------------------------------------------------------------
// <copyright file="Blind.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions.Temporary
{
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>A temporary command to blind an entity. (Just blinds yourself for now.)</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("blind", CommandCategory.Temporary)]
    [ActionDescription("Temporary test command. Blinds an entity.")]
    [ActionSecurity(SecurityRole.player | SecurityRole.mobile)]
    public class Blind : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAlive,
            CommonGuards.InitiatorMustBeConscious,
            CommonGuards.InitiatorMustBeBalanced
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            /* TODO: Add an AltersSensesEffect or whatnot instead of modifying permanent senses!
            if (sender.Thing.Senses.Contains(SensoryType.Sight))
            {
                sender.Thing.Senses[SensoryType.Sight].Enabled = false;
            }

            sender.Write("You blind yourself");
            */
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

            return null;
        }
    }
}