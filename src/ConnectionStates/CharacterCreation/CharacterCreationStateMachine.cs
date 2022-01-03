﻿//-----------------------------------------------------------------------------
// <copyright file="CharacterCreationStateMachine.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;

namespace WheelMUD.ConnectionStates
{
    /// <summary>The central coordinator for Character Creation is a state machine.</summary>
    public abstract class CharacterCreationStateMachine
    {
        /// <summary>Initializes a new instance of the <see cref="CharacterCreationStateMachine"/> class.</summary>
        /// <param name="session">The session.</param>
        public CharacterCreationStateMachine(Session session)
        {
            Session = session;

            // If this is attached to a session, prepare a character for building upon.
            if (Session != null)
            {
                Session.Thing = PlayerManager.PrepareBaseCharacter(session);
            }
        }

        /// <summary>Begin running this CharacterCreationStateMachine.</summary>
        public virtual void Begin()
        {
            HandleNextStep(null, StepStatus.Success);
        }

        /// <summary>An event for the completion of character creation.</summary>
        public event CharacterCreationCompleted CharacterCreationCompleted;

        /// <summary>An event for the abortion of character creation.</summary>
        public event CharacterCreationAborted CharacterCreationAborted;

        /// <summary>Gets the session for the user who may be creating a character.</summary>
        public Session Session { get; private set; }

        /// <summary>Gets or sets the character creation state.</summary>
        internal CharacterCreationSubState CurrentStep { get; set; }

        /// <summary>Gets the next step.</summary>
        /// <param name="current">The current.</param>
        /// <param name="previousStatus">The previous status.</param>
        /// <returns>The next character creation state.</returns>
        public abstract CharacterCreationSubState GetNextStep(CharacterCreationSubState current, StepStatus previousStatus);

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public void ProcessInput(string command)
        {
            // Pass on any input to the current sub state character creation step.
            if (command != string.Empty)
            {
                CurrentStep.ProcessInput(command);
            }
        }

        /// <summary>Processes the next step in the character creation chain, or completes the process if there are no more steps.</summary>
        /// <param name="step">The current step.</param>
        /// <param name="result">The result of the current step.</param>
        public void HandleNextStep(CharacterCreationSubState step, StepStatus result)
        {
            CurrentStep = GetNextStep(step, result);

            // If there were no remaining steps found, we're done.
            if (CurrentStep == null)
            {
                OnCreationComplete();
                return;
            }
            else
            {
                CurrentStep.Begin();
            }

            CurrentStep.StateMachine = this;
        }

        /// <summary>Signals abortion of character creation.</summary>
        internal void AbortCreation()
        {
            CharacterCreationAborted?.Invoke();
        }

        /// <summary>Signals completion of character creation.</summary>
        internal void OnCreationComplete()
        {
            CharacterCreationCompleted?.Invoke(Session);
        }
    }
}