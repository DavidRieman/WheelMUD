//-----------------------------------------------------------------------------
// <copyright file="Time.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>A command to see the current in-game and server times.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("time", CommandCategory.Inform)]
    [ActionAlias("date", CommandCategory.Inform)]
    [ActionAlias("clock", CommandCategory.Inform)]
    [ActionDescription("Get the game time, server time, and local time.")]
    [ActionSecurity(SecurityRole.player)]
    internal class Time : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            // TODO: Fix: sender.Write($"The current game time is: {bridge.World.TimeSystem.Now}");
            session.WriteLine($"The real world server time is: {DateTime.Now.ToString(CultureInfo.InvariantCulture)}");
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