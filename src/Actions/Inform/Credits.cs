//-----------------------------------------------------------------------------
// <copyright file="Credits.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using WheelMUD.Core;
using WheelMUD.Interfaces;
using WheelMUD.Utilities;

namespace WheelMUD.Actions
{
    /// <summary>Action description here.</summary>
    [ExportGameAction(0)]
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
        private static readonly object cacheLockObject = new object();

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            var parameters = actionInput.Params;

            // Ensure two 'credits' commands at the same time do not race for shared cache, etc.
            lock (cacheLockObject)
            {
                if (cachedContents == null || parameters.Length > 0 && parameters[0].ToLower() == "reload")
                {
                    using var reader = new StreamReader(Path.Combine(GameConfiguration.DataStoragePath, "Credits.txt"));
                    var ab = new AnsiBuilder();
                    string s;
                    while ((s = reader.ReadLine()) != null)
                    {
                        if (!s.StartsWith(";"))
                        {
                            ab.AppendLine(s);
                        }
                    }

                    cachedContents = ab.ToString();
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
            return commonFailure;

            // There are currently no arguments nor situations where we expect failure.
        }
    }
}