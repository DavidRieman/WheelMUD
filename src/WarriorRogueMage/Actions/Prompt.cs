//-----------------------------------------------------------------------------
// <copyright file="Prompt.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to get or set your command prompt display.
// </summary>
// <history>
//   May 8, 2012 - By JFed
//   Moved to WarriorRogueMage.Actions from WheelMUD.Actions.
// </history>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using WheelMUD.Actions;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;

    /// <summary>An action to get or set your command prompt display.</summary>
    [ExportGameAction]
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
            var sender = actionInput.Controller;

            if (!string.IsNullOrEmpty(actionInput.Tail))
            {
                // Save tail as player's new prompt
                try
                {
                    this.playerBehavior.Prompt = actionInput.Tail;
                    sender.Thing.Save();
                    sender.Write("New prompt saved.");
                }
                catch (Exception)
                {
                    sender.Write("Error, prompt not saved.");
                }

                return;
            }

            // No new prompt supplied, so we display help and current values to the player
            var output = new StringBuilder();

            // Create an array of values available to the player
            output.AppendFormat("{0}{1}{2}\n", "Token".PadRight(15), "Current Value".PadRight(15), "Description".PadRight(40));
            output.AppendFormat("{0}{1}{2}\n", "-----".PadRight(15), "-------------".PadRight(15), "-----------".PadRight(40));

            // Discover all methods with the promptable attribute in the adapter
            foreach (var m in this.playerBehavior.GetType().GetMethods())
            {
                var promptAttr = m.GetCustomAttributes(typeof(PlayerPromptableAttribute), false);
                if (promptAttr.Length > 0)
                {
                    var tokenInfo = (PlayerPromptableAttribute)promptAttr[0];

                    // Invoke the method to get current values
                    var currentValue = (string)m.Invoke(this.playerBehavior, new object[] { });

                    output.AppendFormat("{0}{1}{2}\n", tokenInfo.Token.PadRight(15), currentValue.PadRight(15), tokenInfo.Description.PadRight(40));
                }
            }

            output.AppendFormat("\nCurrent prompt is '{0}'\n", this.playerBehavior.Prompt);
            output.AppendFormat("Parsed prompt is '{0}'\n", this.playerBehavior.BuildPrompt());
            sender.Write(output.ToString());
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

            this.playerBehavior = actionInput.Controller.Thing.Behaviors.FindFirst<PlayerBehavior>();

            return null;
        }
    }
}