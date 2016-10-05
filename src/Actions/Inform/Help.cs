//-----------------------------------------------------------------------------
// <copyright file="Help.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Shabubu - 5/6/2009 2:35:52 AM
//   Modified by: bengecko - 12/05/2009
//   Heavily modified by: Pure - 03/10/2010
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to look up help information from the help system.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("help", CommandCategory.Inform)]
    [ActionDescription("Display help text for a command or topic.")]
    [ActionExample("Example: help look")]
    [ActionSecurity(SecurityRole.all)]
    public class Help : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>A value indicating whether MXP may be used during this command's output.</summary>
        private bool useMXP;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            string commandTail = actionInput.Tail.ToLower().Trim();

            if (commandTail == string.Empty)
            {
                commandTail = "0";
            }

            // First we check to see if the help topic is in the help manager
            HelpTopic helpTopic = HelpManager.Instance.FindHelpTopic(commandTail);
            if (helpTopic != null)
            {
                if (this.useMXP)
                {
                    var sb = new StringBuilder();
                    foreach (string line in helpTopic.Contents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                    {
                        sb.AppendLine("<%mxpopenline%>" + line);
                    }

                    sender.Write("<%mxpsecureline%><!element see '<send href=\"help &cref;\">' att='cref' open>" + sb.ToString().Trim(Environment.NewLine.ToCharArray()));
                }
                else
                {
                    // @@@ TODO: Output without MXP syntax!
                }
            }
            else
            {
                // If not found we check commands
                var commands = CommandManager.Instance.MasterCommandList;

                // First check for an exact match with a command name.
                var action = commands.Values.FirstOrDefault(c => c.Name.ToLower() == commandTail);

                // If no exact match, check for a partial match.
                if (action == null)
                {
                    action = commands.Values.FirstOrDefault(c => c.Name.ToLower().StartsWith(commandTail));
                }

                // Show result if a match was found
                if (action != null)
                {
                    if (action.Description != null)
                    {
                        sender.Write(action.Description);
                    }
                        
                    if (action.Example != null)
                    {
                        sender.Write(action.Example);
                    }
                        
                    return;
                }

                // Display help splash screen if available
                string name = Utilities.Configuration.GetDataStoragePath();
                string path = Path.Combine(Path.GetDirectoryName(name), "Files");

                try
                {
                    var sr = new StreamReader(Path.Combine(path, "Help.ans"), Encoding.GetEncoding(437));
                    string data = sr.ReadToEnd();
                    sender.Write(data);
                    sr.Close();
                }
                catch (Exception)
                {
                    sender.Write("No such help topic.");
                }
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

            var session = actionInput.Controller as Session;
            if (session != null && session.Terminal != null)
            {
                this.useMXP = session.Terminal.UseMXP;
            }

            // There are currently no arguments nor situations where we expect failure.
            return null;
        }
    }
}