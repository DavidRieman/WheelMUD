//-----------------------------------------------------------------------------
// <copyright file="Previous.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions
{
    /// <summary>A command that performs pagination.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("previous", CommandCategory.Inform)]
    [ActionAlias("p", CommandCategory.Inform)]
    [ActionDescription("View the previous page of output.")]
    [ActionSecurity(SecurityRole.player)]
    public class Previous : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (!(actionInput.Controller is Session session)) return;

            if (session.Connection.OutputBuffer.HasMoreData)
            {
                session.Connection.ProcessBuffer(BufferDirection.Backward);
            }
            else
            {
                actionInput.Controller.Write(new OutputBuilder(session.TerminalOptions).
                    SingleLine("There is no more data."));
            }
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