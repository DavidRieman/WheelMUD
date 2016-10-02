//-----------------------------------------------------------------------------
// <copyright file="ConfirmCreationEntryState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to confirm creation and entry into the game.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using WheelMUD.Core;

    /// <summary>Character creation state used to confirm creation and entry into the game.</summary>
    public class ConfirmCreationEntryState : CharacterCreationSubState
    {
        /// <summary>Initializes a new instance of the <see cref="ConfirmCreationEntryState"/> class.</summary>
        /// <param name="session">The session.</param>
        public ConfirmCreationEntryState(Session session)
            : base(session)
        {
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            if (command.ToLower() == "yes" || command.ToLower() == "y")
            {
                this.StateMachine.HandleNextStep(this, StepStatus.Success);
            }
            else
            {
                this.StateMachine.AbortCreation();
            }
        }

        public override string BuildPrompt()
        {
            return string.Format("Are you sure you wish to create a new character?{0}Yes/No> ", Environment.NewLine);
        }
    }
}