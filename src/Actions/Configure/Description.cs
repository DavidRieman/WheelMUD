//-----------------------------------------------------------------------------
// <copyright file="Description.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>An action to change your character's description.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("description", CommandCategory.Configure)]
    [ActionAlias("descript", CommandCategory.Configure)]
    [ActionDescription("Change your character's description.")]
    [ActionSecurity(SecurityRole.player)]
    public class Description : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>();

        /// <summary>Gets or sets the new description that will be used.</summary>
        private string NewDescription { get; set; }

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            if (actionInput.Actor != null)
            {
                if (!string.IsNullOrEmpty(NewDescription))
                {
                    actionInput.Actor.Description = NewDescription;
                    actionInput.Actor.FindBehavior<PlayerBehavior>()?.SavePlayer();
                    actionInput.Session.WriteLine("Description successfully changed.");
                }
                else
                {
                    actionInput.Session.WriteLine($"Your current description is \"{actionInput.Actor.Description}\".");
                }
            }
            else
            {
                actionInput.Session.WriteLine("Unexpected error occurred changing description, please contact admin.");
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

            NewDescription = actionInput.Tail;

            return null;
        }
    }
}