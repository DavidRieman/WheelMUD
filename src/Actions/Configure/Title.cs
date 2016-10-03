//-----------------------------------------------------------------------------
// <copyright file="Title.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A command to set a player's title.
//   Created: February 2007 by Saquivor.
//   Modified: December 2009 by bengecko
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>A command to set a player's title.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("title", CommandCategory.Player)]
    [ActionAlias("set title", CommandCategory.Player)]
    [ActionDescription("Set or view your title.")]
    [ActionSecurity(SecurityRole.player)]
    public class Title : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards> { };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            if (sender != null && sender.Thing != null)
            {
                Thing player = sender.Thing;
                if (player != null)
                {
                    if (string.IsNullOrEmpty(actionInput.Tail))
                    {
                        sender.Write(string.Format("Your current title is \"{0}\".", player.Title));
                    }
                    else
                    {
                        player.Title = actionInput.Tail;
                        player.Save();

                        sender.Write("Title modified.");
                    }
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

            return null;
        }
    }
}