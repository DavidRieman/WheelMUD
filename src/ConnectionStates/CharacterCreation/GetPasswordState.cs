//-----------------------------------------------------------------------------
// <copyright file="GetPasswordState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;
using WheelMUD.Data;
using WheelMUD.Server;

namespace WheelMUD.ConnectionStates
{
    /// <summary>Character creation state used to request a password for the new character.</summary>
    public class GetPasswordState : CharacterCreationSubState
    {
        // Attempt to use "hidden" mode for a while, in case the client+server negotiated a mode where the server
        // is repeating received keystrokes back to their output.
        private static readonly OutputBuilder prompt = new OutputBuilder().Append("Enter a password: > <%hidden%>");

        private static readonly OutputBuilder InitialStateMessage;

        static GetPasswordState()
        {
            var output = new OutputBuilder().AppendLine();
            output.AppendLine(AppConfigInfo.Instance.UserAccountIsPlayerCharacter ?
                "Please carefully select a password for this character." :
                "Please carefully select a password for this user account.");
            output.AppendLine("Although the Telnet protocol provides an authentic retro experience, unfortunately it also sends password in plain-text. Do not use the same password as you use for any other account. Do not use this password on a network with machines do not fully trust (especially public networks).");
            output.AppendLine("Your password can be changed while logged in.");
            InitialStateMessage = output;
        }

        /// <summary>Initializes a new instance of the <see cref="GetPasswordState"/> class.</summary>
        /// <param name="session">The session.</param>
        public GetPasswordState(Session session)
            : base(session)
        { }

        public override void Begin()
        {
            Session.Write(InitialStateMessage);
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            // Do not use the command parameter here. It is trimmed of whitespace, which will inhibit the use of passwords 
            // with whitespace on either end. Instead we need to respect the raw line of input for password entries.
            Session.User.SetPassword(Session.Connection.LastRawInput);
            Session.Connection.LastRawInput = null;

            StateMachine.HandleNextStep(this, StepStatus.Success);
        }

        public override OutputBuilder BuildPrompt()
        {
            return prompt;
        }
    }
}