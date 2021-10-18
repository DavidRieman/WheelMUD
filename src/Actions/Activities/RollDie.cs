//-----------------------------------------------------------------------------
// <copyright file="RollDie.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>Temporary script to test the dice system.</summary>
    /// <remarks>
    /// TODO: Adjust to be a handy command for role-playing and so on; Generate a room sensory event?
    /// TODO: Allow custom-sided die, app-configurable default sides (like 20 would be a better default for some games)?
    /// </remarks>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("roll", CommandCategory.Activities)]
    [ActionDescription("Roll a die or dice.")]
    [ActionSecurity(SecurityRole.player)]
    public class RollDie : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (actionInput.Session == null) return;

            var die = DiceService.Instance.GetDie(6);
            actionInput.Session.WriteLine($"You roll a {die.Roll()}.");
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