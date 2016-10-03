//-----------------------------------------------------------------------------
// <copyright file="AFK.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An action to set your status as 'inactive' to other players.
//   @@@ TODO: Implement.  Other actions should automatically unset.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>Sets your status as AFK to other players.  Optional reason can be specified (eg. AFK Back in 5).</summary>
    /// <remarks>Added a length limiter on the AFK reason to ensure it doesn't gum out anything displaying the message.</remarks>
    [ExportGameAction]
    [ActionPrimaryAlias("AFK", CommandCategory.Configure)]
    [ActionAlias("inactive", CommandCategory.Configure)]
    [ActionDescription("Sets your status as afk to other players.  Optional reason can be specified (eg. AFK Back in 5).")]
    [ActionSecurity(SecurityRole.player)]
    public class AFK : GameAction
    {
        /// <summary>Putting this up here for ease of locating and changing</summary>
        private const int MAXREASONLENGTH = 25;

        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.InitiatorMustBeAPlayer,
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;

            PlayerBehavior playerBehavior = actionInput.Controller.Thing.Behaviors.FindFirst<PlayerBehavior>();
            if (playerBehavior != null)
            {
                if (playerBehavior.IsAFK)
                {
                    sender.Write("Your are no longer AFK.");

                    playerBehavior.IsAFK = false;
                    playerBehavior.WhenWentAFK = null;
                    playerBehavior.AFKReason = string.Empty;
                }
                else
                {
                    string afkReason = actionInput.Tail;

                    if (!string.IsNullOrEmpty(afkReason))
                    {
                        sender.Write(string.Format("You have set your status to AFK: {0}.", afkReason));
                    }
                    else
                    {
                        sender.Write("You have set your status to AFK.");
                    }

                    playerBehavior.IsAFK = true;

                    // Store in Universal time in order to convert to others local time
                    playerBehavior.WhenWentAFK = DateTime.Now.ToUniversalTime();
                    playerBehavior.AFKReason = afkReason;
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

            // Rule: Prevent talking to yourself.
            if (actionInput.Tail.Length > MAXREASONLENGTH)
            {
                return string.Format("That's too wordy.  Let's keep it to {0} characters.", MAXREASONLENGTH);
            }

            return null;
        }
    }
}