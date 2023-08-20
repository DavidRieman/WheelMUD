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
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("attributes", CommandCategory.Inform)]
    [ActionAlias("attrib", CommandCategory.Inform)]
    [ActionAlias("stats", CommandCategory.Inform)]
    [ActionDescription("Show detailed information about your attributes.")]
    [ActionSecurity(SecurityRole.player)]
    public class Attributes : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            var output = new OutputBuilder();

            foreach (var kvp in actionInput.Actor.Stats)
            {
                output.Append(kvp.Value.Name.PadRight(20));
                output.Append(kvp.Value.Value);
                output.AppendLine();
            }

            foreach (var kvp in actionInput.Actor.Attributes)
            {
                output.Append(kvp.Value.Name.PadRight(20));
                output.Append(kvp.Value.Value);
                output.AppendLine();
            }

            session.Write(output);
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