//-----------------------------------------------------------------------------
// <copyright file="ConfirmPasswordState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.ConnectionStates
{
    /// <summary>Character creation state used to confirm a password for the new character.</summary>
    public class ConfirmPasswordState : CharacterCreationSubState
    {
        // Attempt to use "hidden" mode for a while, in case the client+server negotiated a mode where the server
        // is repeating received keystrokes back to their output.
        private static readonly OutputBuilder prompt = new OutputBuilder().Append("Please retype your password: > <%hidden%>");

        /// <summary>Initializes a new instance of the <see cref="ConfirmPasswordState"/> class.</summary>
        /// <param name="session">The session.</param>
        public ConfirmPasswordState(Session session)
            : base(session)
        {
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            // Do not use the command parameter here. It is trimmed of whitespace, which will inhibit the use of passwords 
            // with whitespace on either end. Instead we need to respect the raw line of input for password entries.
            if (Session.User.PasswordMatches(Session.Connection.LastRawInput))
            {
                StateMachine.HandleNextStep(this, StepStatus.Success);
            }
            else
            {
                Session.WriteLine("I am afraid the passwords entered do not match.", false);
                StateMachine.HandleNextStep(this, StepStatus.Failure);
            }
        }

        public override OutputBuilder BuildPrompt()
        {
            return prompt;
        }
    }
}