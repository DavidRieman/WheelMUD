//-----------------------------------------------------------------------------
// <copyright file="More.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Permissive License.  All other rights reserved.
// </copyright>
// <summary>
//   A command that performs pagination.
//   Created: November 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Core.Enums;
    using WheelMUD.Core.Interfaces;

    /// <summary>
    /// A command that performs pagination.
    /// </summary>
    [ActionPrimaryAlias("more", CommandCategory.Inform)]
    [ActionAlias("m", CommandCategory.Inform)]
    [ActionDescription("View the next page of output.")]
    [ActionSecurity(SecurityRole.player)]
    public class Done : Action
    {
        /// <summary>The list of common guards that must always be passed for instances of this action.</summary>
        private static readonly List<CommonGuards> commonGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="sender">Sender of the command.</param>
        /// <param name="bridge">The system to script bridge.</param>
        /// <param name="command">The full command requested.</param>
        public override void Execute(IController sender, ISystemToScriptBridge bridge, ICommand command)
        {
            ISession session = (ISession) sender;

            if (session.Connection.OutputBuffer.HasMoreData)
            {
                session.Connection.ProcessBuffer(BufferDirection.Forward);
            }
            else
            {
                sender.Write("There is no more data");
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="sender">Sender of the command.</param>
        /// <param name="bridge">The system to script bridge.</param>
        /// <param name="command">The full command requested.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(IController sender, ISystemToScriptBridge bridge, ICommand command)
        {
            string commonFailure = VerifyCommonGuards(sender, bridge, command, commonGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            return null;
        }
    }
}