//-----------------------------------------------------------------------------
// <copyright file="Password.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Actions
{
    /// <summary>An action to set your login password.</summary>
    [CoreExports.GameAction(0)]
    [ActionPrimaryAlias("password", CommandCategory.Configure)]
    // TODO: #65: This should really take a series of stand-alone prompts, including: the Old Password, the New Password, and Confirm the New Password!
    //       Perhaps the password change should only be implemented after a proper prompt queue is built. Alternatively, this could become only
    //       supported from the character selection state, and have a way to get back there even when UserAccountIsPlayerCharacter is set.
    [ActionDescription("Change's your password. Usage: password Value ConfirmValue")]
    [ActionSecurity(SecurityRole.player)]
    public class Password : GameAction
    {
        /// <summary>List of reusable guards which must be passed before action requests may proceed to execution.</summary>
        private static readonly List<CommonGuards> ActionGuards = new()
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
            /* TODO: https://github.com/DavidRieman/WheelMUD/issues/43: Repair and improve password command.
            {
                sender.Write("Password successfully changed.");
            }
            else
            {
                sender.Write("Unexpected error occurred changing password, please contact admin.");
            }*/
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

            // TODO: Use temporary change of command context for verification (rather than single line confirmation).
            if (actionInput.Params.Length == 2)
            {
                var password1 = actionInput.Params[0];
                var password2 = actionInput.Params[1];

                if (password1 != password2)
                {
                    return "Passwords do not match.";
                }

                NewPassword = password1;
                return null;
            }

            return "Invalid number of parameters.";
        }
    }
}