//-----------------------------------------------------------------------------
// <copyright file="Effects.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to list the description of known effects upon you.
//   @@@ TODO: Implement
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;

    /// <summary>An action to list the description of known effects upon you.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("effects", CommandCategory.Inform)]
    [ActionAlias("effect", CommandCategory.Inform)]
    [ActionDescription("List the description of known effects upon you.")]
    [ActionSecurity(SecurityRole.player)]
    public class Effects : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            // @@@ TODO: Allow admins to check effects on target?
            /* @@@ FIX?
            var sb = new StringBuilder();

            string line = "+" + string.Empty.PadRight(70, '-') + "+";

            // Header
            sb.AppendFormat("{0,72}\r\n", "Effects on " + sender.Thing.Name);
            sb.AppendFormat("{0,72}\r\n", line);

            // Iterate through Effects
            bool hasEffect = false;
            foreach (Behavior behavior in sender.Thing.BehaviorManager.ManagedBehaviors)
            {
                var effect = behavior as Effect;
                if (effect != null)
                {
                    hasEffect = true;
                    sb.AppendFormat(
                        "| {0, -10}{1, 16}{2, 16}{3, 12}{4, -14} |\r\n",
                        "Effect [",
                        effect.Name + " ]",
                        "Duration [",
                        effect.RemainingTime.TotalSeconds,
                        " ] seconds");
                }
            }

            if (!hasEffect)
            {
                sb.AppendFormat("There are no effects effecting " + sender.Thing.Name);
            }

            // Footer
            sb.AppendFormat("{0,72}\r\n", line);

            // Send to Parser
            sender.Write(sb.ToString().Trim());
            */
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