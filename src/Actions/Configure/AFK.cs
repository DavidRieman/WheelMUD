//-----------------------------------------------------------------------------
// <copyright file="AFK.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>Sets your status as AFK to other players.  Optional reason can be specified (eg. AFK Back in 5).</summary>
    /// <remarks>Added a length limiter on the AFK reason to ensure it doesn't gum out anything displaying the message.</remarks>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("afk", CommandCategory.Configure)]
    [ActionAlias("inactive", CommandCategory.Configure)]
    [ActionDescription("Sets your status as afk to other players.  Optional reason can be specified (eg. AFK Back in 5).")]
    [ActionSecurity(SecurityRole.player)]
    public class AFK : GameAction
    {
        /// <summary>Putting this up here for ease of locating and changing</summary>
        private const int MAXREASONLENGTH = 25;

        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new()
        {
            CommonGuards.InitiatorMustBeAPlayer,
        };

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            var playerBehavior = actionInput.Actor.FindBehavior<PlayerBehavior>();

            if (playerBehavior != null)
            {
                if (playerBehavior.IsAFK)
                {
                    session.WriteLine("You are no longer AFK.");
                    playerBehavior.IsAFK = false;
                    playerBehavior.WhenWentAFK = null;
                    playerBehavior.AFKReason = string.Empty;
                }
                else
                {
                    var afkReason = actionInput.Tail;
                    playerBehavior.IsAFK = true;

                    // Store in Universal time in order to convert to others local time.
                    playerBehavior.WhenWentAFK = DateTime.Now.ToUniversalTime();
                    playerBehavior.AFKReason = afkReason;

                    session.WriteLine(!string.IsNullOrEmpty(afkReason) ? $"You have set your status to AFK: {afkReason}." : "You have set your status to AFK.");
                }
            }
        }

        /// <summary>Checks against the guards for the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        /// <returns>A string with the error message for the user upon guard failure, else null.</returns>
        public override string Guards(ActionInput actionInput)
        {
            var commonFailure = VerifyCommonGuards(actionInput, ActionGuards);
            if (commonFailure != null)
            {
                return commonFailure;
            }

            return actionInput.Tail.Length > MAXREASONLENGTH ? $"That's too wordy.  Let's keep it to {MAXREASONLENGTH} characters." : null;
        }
    }
}