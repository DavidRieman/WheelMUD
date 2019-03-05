//-----------------------------------------------------------------------------
// <copyright file="Date.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command script to display the ingame and out of game date and time.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to display the in-game and out of game date and time.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("date", CommandCategory.Inform)]
    [ActionDescription("Get the game date and server time.")]
    [ActionSecurity(SecurityRole.player)]
    public class Date : GameAction
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
            sb.Append("The current game time is ");
           // @@@ Broken: sb.AppendLine(bridge.World.TimeSystem.Now);
            sb.AppendLine();
            sb.Append("The real world time is ");
            sb.Append(System.DateTime.Now.ToString());

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