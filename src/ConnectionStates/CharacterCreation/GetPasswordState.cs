//-----------------------------------------------------------------------------
// <copyright file="GetPasswordState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to request a password from the new character.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using WheelMUD.Core;

    /// <summary>Character creation state used to request a password from the new character.</summary>
    public class GetPasswordState : CharacterCreationSubState
    {
        /// <summary>Initializes a new instance of the <see cref="GetPasswordState"/> class.</summary>
        /// <param name="session">The session.</param>
        public GetPasswordState(Session session)
            : base(session)
        {
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            var playerBehavior = this.StateMachine.NewCharacter.Behaviors.FindFirst<PlayerBehavior>();

            // Do not use the command parameter here. It is trimmed of whitespace, which will inhibit the use of passwords 
            // with whitespace on either end. Instead we need to respect the raw line of input for password entries.
            playerBehavior.SetPassword(this.Session.Connection.LastRawInput);

            this.StateMachine.HandleNextStep(this, StepStatus.Success);
        }

        public override string BuildPrompt()
        {
            return "Enter a password for this character.\n> ";
        }
    }
}