//-----------------------------------------------------------------------------
// <copyright file="Repeat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that performs pagination.
//   Created: November 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Enums;
    using WheelMUD.Interfaces;

    /// <summary>A command that performs pagination.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("repeat", CommandCategory.Inform)]
    [ActionAlias("r", CommandCategory.Inform)]
    [ActionDescription("Repeat the page of output.")]
    [ActionSecurity(SecurityRole.player)]
    public class Repeat : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards> { };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            Session session = sender as Session;
            if (session == null)
            {
                sender.Write("There is no valid session for this command.");
            }
            else if (session.Connection.OutputBuffer.HasMoreData)
            {
                session.Connection.ProcessBuffer(BufferDirection.Repeat);
            }
            else
            {
                sender.Write("There is no more data");
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

            return null;
        }
    }
}