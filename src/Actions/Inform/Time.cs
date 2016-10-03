//-----------------------------------------------------------------------------
// <copyright file="Time.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command to see the current in-game and server times.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to see the current in-game and server times.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("time", CommandCategory.Inform)]
    [ActionAlias("clock", CommandCategory.Inform)]
    [ActionDescription("Get the game time, server time, and local time.")]
    [ActionSecurity(SecurityRole.player)]
    internal class Time : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            StringBuilder sb = new StringBuilder();
            sb.Append("Game time is: ");
           // @@@ Broken: sb.AppendLine(bridge.World.TimeSystem.Now);
            sb.Append("Real world server time is: ");
            sb.AppendLine(DateTime.Now.ToString());
            sender.Write(sb.ToString());
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