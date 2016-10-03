//-----------------------------------------------------------------------------
// <copyright file="Credits.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Lists the credits for the game.
//   Created: May 2009 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>Action description here.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("credits", CommandCategory.Inform)]
    [ActionAlias("immlist", CommandCategory.Inform)]
    [ActionAlias("wizlist", CommandCategory.Inform)]
    [ActionDescription("Display help text for a command or topic.")]
    [ActionSecurity(SecurityRole.all)]
    public class Credits : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
        };

        /// <summary>Cache these contents to reduce file I/O.</summary>
        private static string cachedContents = null;

        /// <summary>The synchronization locking object.</summary>
        private static object cacheLockObject = new object();
        
        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            var parameters = actionInput.Params;
            
            // Ensure two 'credits' commands at the same time do not race for shared cache, etc.
            lock (cacheLockObject)
            {
                if (cachedContents == null || (parameters.Length > 0 && parameters[0].ToLower() == "reload"))
                {
                    StreamReader reader = new StreamReader("Files\\Credits.txt");
                    StringBuilder stringBuilder = new StringBuilder();
                    string s;
                    while ((s = reader.ReadLine()) != null)
                    {
                        if (!s.StartsWith(";"))
                        {
                            stringBuilder.AppendLine(s);
                        }
                    }

                    cachedContents = stringBuilder.ToString();
                }

                sender.Write(cachedContents);
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

            // There are currently no arguments nor situations where we expect failure.
            return null;
        }
    }
}