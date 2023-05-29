//-----------------------------------------------------------------------------
// <copyright file="ConfirmCreationEntryState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using WheelMUD.Core;
using WheelMUD.Data;
using WheelMUD.Server;

namespace WheelMUD.ConnectionStates
{
    /// <summary>Character creation state used to confirm creation and entry into the game.</summary>
    public class ConfirmCreationEntryState : CharacterCreationSubState
    {
        // TODO: Add and serve ConfirmCreationPromptMXP as well, with clickable menu options to supporting clients?
        private static string ConfirmCreationPrompt = AppConfigInfo.Instance.UserAccountIsPlayerCharacter ?
            "Are you sure you wish to create a new character? [Y]es/[N]o: > " :
            "Are you sure you wish to create a new user account? [Y]es/[N]o: > ";

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
                Session.User = new User();
                Session.User.AccountHistory.Created = DateTime.Now;
                StateMachine.HandleNextStep(this, StepStatus.Success);
            }
            else
            {
                StateMachine.AbortCreation();
            }
        }

        public override OutputBuilder BuildPrompt()
        {
            return new OutputBuilder(ConfirmCreationPrompt.Length).Append(ConfirmCreationPrompt);
        }
    }
}