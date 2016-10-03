//-----------------------------------------------------------------------------
// <copyright file="Typo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: December 2009 by bengecko.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Actions
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;
    using WheelMUD.Universe.Information;

    /// <summary>A command to report a simple typographical error.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("typo", CommandCategory.Communicate)]
    [ActionDescription("Report a bug in the descriptions in or near a room.")]
    [ActionSecurity(SecurityRole.player)]
    public class Typo : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastOneArgument,
            CommonGuards.InitiatorMustBeAPlayer,
        };

        private Thing player;

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;
            TypoEntry typoEntry = new TypoEntry
            {
                Note = actionInput.Tail,
                PlaceID = this.player.Parent.ID,
                SubmittedByPlayerID = this.player.ID,
                SubmittedDateTime = DateTime.Now,
                Resolved = false
            };

            typoEntry.Save();

            sender.Write("Thank you.  Your typo report has been submitted.");
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

            this.player = actionInput.Controller.Thing;

            return null;
        }
    }
}