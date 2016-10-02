//-----------------------------------------------------------------------------
// <copyright file="GetDescriptionState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to request a description for the new character.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using WheelMUD.Core;

    /// <summary>Character creation state used to request a description for the new character.</summary>
    public class GetDescriptionState : CharacterCreationSubState
    {
        /// <summary>Initializes a new instance of the <see cref="GetDescriptionState"/> class.</summary>
        /// <param name="session">The session.</param>
        public GetDescriptionState(Session session)
            : base(session)
        {
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            this.StateMachine.NewCharacter.Description = command;
            this.StateMachine.HandleNextStep(this, StepStatus.Success);
        }

        public override string BuildPrompt()
        {
            return "Enter a description for your character:\n> ";
        }
    }
}