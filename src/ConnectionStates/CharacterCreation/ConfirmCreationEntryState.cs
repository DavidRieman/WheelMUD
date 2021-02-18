//-----------------------------------------------------------------------------
// <copyright file="ConfirmCreationEntryState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using WheelMUD.Core;
    using WheelMUD.Data;

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
                Session.User = new User()
                {
                    AccountCreationDate = DateTime.Now,
                };
                StateMachine.HandleNextStep(this, StepStatus.Success);
            }
            else
            {
                StateMachine.AbortCreation();
            }
        }

        public override string BuildPrompt()
        {
            var creationType = AppConfigInfo.Instance.UserAccountIsPlayerCharacter ? "character" : "user account";
            return string.Format("Are you sure you wish to create a new {0}?{1}Yes/No> ", creationType, Environment.NewLine);
        }
    }
}