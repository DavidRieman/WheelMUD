//-----------------------------------------------------------------------------
// <copyright file="Prompt.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WarriorRogueMage.Actions
{
    /// <summary>An action to get or set your command prompt display.</summary>
    [CoreExports.GameAction(100)]
    [ActionPrimaryAlias("prompt", CommandCategory.Configure)]
    [ActionDescription("Sets your prompt display.  Enter an empty value for further options.")]
    [ActionSecurity(SecurityRole.player)]
    public class Prompt : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAPlayer,
        };

        private PlayerBehavior playerBehavior;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            if (!string.IsNullOrEmpty(actionInput.Tail))
            {
                playerBehavior.Prompt = actionInput.Tail;
                return;
            }

            // No new prompt supplied, so we display help and current values to the player
            var output = new OutputBuilder();

            // Create an array of values available to the player
            output.AppendLine($"{"Token".PadRight(15)}{"Current Value".PadRight(15)}{"Description".PadRight(40)}");
            output.AppendLine($"{"-----".PadRight(15)}{"-------------".PadRight(15)}{"-----------".PadRight(40)}");

            // Discover all methods with the promptable attribute in the adapter
            foreach (var m in playerBehavior.GetType().GetMethods())
            {
                var promptAttr = m.GetCustomAttributes(typeof(PlayerPromptableAttribute), false);
                if (promptAttr.Length > 0)
                {
                    var tokenInfo = (PlayerPromptableAttribute)promptAttr[0];

                    // Invoke the method to get current values
                    var currentValue = (string)m.Invoke(playerBehavior, new object[] { });

                    output.AppendLine($"{tokenInfo.Token.PadRight(15)}{currentValue.PadRight(15)}{tokenInfo.Description.PadRight(40)}");
                }
            }

            output.AppendLine($"Current prompt is '{playerBehavior.Prompt}'");
            output.AppendLine($"Parsed prompt is '{playerBehavior.BuildPrompt(session.TerminalOptions)}'");
            session.Write(output);
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

            playerBehavior = actionInput.Actor.FindBehavior<PlayerBehavior>();

            return null;
        }
    }
}