//-----------------------------------------------------------------------------
// <copyright file="Password.cs" company="WheelMUD Development Team">
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
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Core.Attributes;
    using WheelMUD.Interfaces;

    /// <summary>An action to set your status as 'inactive' to other players.</summary>
    [ExportGameAction]
    [ActionPrimaryAlias("password", CommandCategory.Configure)]
    [ActionDescription("Change's your password. Usage: password Value ConfirmValue")]
    [ActionSecurity(SecurityRole.player)]
    public class Password : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new List<CommonGuards>
        {
            CommonGuards.RequiresAtLeastTwoArguments,
            CommonGuards.InitiatorMustBeAPlayer,
        };

        /// <summary>Gets or sets the new password that will be used.</summary>
        private string NewPassword { get; set; }

        /// <summary>Executes the command.</summary>
        /// <param name="actionInput">The full input specified for executing the command.</param>
        public override void Execute(ActionInput actionInput)
        {
            IController sender = actionInput.Controller;

            if (sender != null && sender.Thing != null)
            {
                PlayerBehavior playerBehavior = sender.Thing.Behaviors.FindFirst<PlayerBehavior>();
                if (playerBehavior != null)
                {
                    playerBehavior.SetPassword(this.NewPassword);
                    sender.Write("Password successfully changed.");
                }
                else
                {
                    sender.Write("Unexpected error occurred changing password, please contact admin.");
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

            // @@@ TODO: Use temporary change of command context for verification, much like '
            if (actionInput.Params.Length == 2)
            {
                string password1 = actionInput.Params[0];
                string password2 = actionInput.Params[1];

                if (password1 != password2)
                {
                    return "Passwords do not match.";
                }

                this.NewPassword = password1;
                return null;
            }

            return "Invalid number of parameters.";
        }
    }
}