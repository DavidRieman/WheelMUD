//-----------------------------------------------------------------------------
// <copyright file="Attributes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to show detailed information about your attributes.
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

    /// <summary>An action to show detailed information about your attributes.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("attributes", CommandCategory.Inform)]
    [ActionAlias("attrib", CommandCategory.Inform)]
    [ActionAlias("stats", CommandCategory.Inform)]
    [ActionDescription("Show detailed information about your attributes.")]
    [ActionSecurity(SecurityRole.player)]
    public class Attributes : GameAction
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
            var sb = new StringBuilder();

            foreach (KeyValuePair<string, GameStat> kvp in sender.Thing.Stats)
            {
                sb.Append(kvp.Value.Name.PadRight(20));
                sb.Append(kvp.Value.Value);
                sb.Append(Environment.NewLine);
            }

            foreach (KeyValuePair<string, GameAttribute> kvp in sender.Thing.Attributes)
            {
                sb.Append(kvp.Value.Name.PadRight(20));
                sb.Append(kvp.Value.Value);
                sb.Append(Environment.NewLine);
            }

            sender.Write(sb.ToString().Trim());
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