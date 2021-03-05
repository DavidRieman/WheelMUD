//-----------------------------------------------------------------------------
// <copyright file="Attributes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.Actions
{
    /// <summary>An action to show detailed information about your attributes.</summary>
    [ExportGameAction(0)]
    [ActionPrimaryAlias("attributes", CommandCategory.Inform)]
    [ActionAlias("attrib", CommandCategory.Inform)]
    [ActionAlias("stats", CommandCategory.Inform)]
    [ActionDescription("Show detailed information about your attributes.")]
    [ActionSecurity(SecurityRole.player)]
    public class Attributes : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (!(actionInput.Controller is Session session)) return;
            
            var ab = new OutputBuilder(session.TerminalOptions);

            foreach (var kvp in actionInput.Controller.Thing.Stats)
            {
                ab.Append(kvp.Value.Name.PadRight(20));
                ab.Append(kvp.Value.Value);
                ab.AppendLine();
            }

            foreach (var kvp in actionInput.Controller.Thing.Attributes)
            {
                ab.Append(kvp.Value.Name.PadRight(20));
                ab.Append(kvp.Value.Value);
                ab.AppendLine();
            }

            actionInput.Controller.Write(new OutputBuilder(session.TerminalOptions).SingleLine(ab.ToString()));
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