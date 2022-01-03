﻿//-----------------------------------------------------------------------------
// <copyright file="Relinquish.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>An action to relinquish control of a mobile or player.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("relinquish", CommandCategory.Admin)]
    [ActionAlias("relinquish control", CommandCategory.Admin)]
    [ActionAlias("dispossess", CommandCategory.Admin)]
    [ActionDescription("Release control of a mobile or player.")]
    [ActionSecurity(SecurityRole.mobile)]
    public class Relinquish : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // TODO: Implement.
            // TODO: This or similar might also want to be available to players, depending on how security for actions
            //       gets done during possession or for pets... Perhaps to be applied through context commands?
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