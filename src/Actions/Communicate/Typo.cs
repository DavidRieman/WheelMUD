//-----------------------------------------------------------------------------
// <copyright file="Typo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Universe.Information;

namespace WheelMUD.Actions
{
    /// <summary>A command to report a simple typographical error.</summary>
    /// <remarks>TODO: Should be able to see what the report will contain, target a Thing by keywords, and so on, as well as pointing out what the typo word is.</remarks>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("typo", CommandCategory.Communicate)]
    [ActionDescription("Report a bug in the descriptions in or near a room.")]
    [ActionSecurity(SecurityRole.player)]
    public class Typo : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new()
        {
            CommonGuards.RequiresAtLeastOneArgument,
            CommonGuards.InitiatorMustBeAPlayer,
        };

        private Thing player;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            var session = actionInput.Session;
            if (session == null) return; // This action only makes sense for player sessions.

            var typoEntry = new TypoEntry
            {
                Note = actionInput.Tail,
                PlaceID = player.Parent?.Id,
                SubmittedByPlayerID = player.Id,
                SubmittedDateTime = DateTime.Now,
                Resolved = false
            };

            typoEntry.Save();

            session.WriteLine("Thank you. Your typo report has been submitted.");
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

            player = actionInput.Actor;

            return null;
        }
    }
}