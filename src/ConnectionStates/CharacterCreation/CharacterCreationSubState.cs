//-----------------------------------------------------------------------------
// <copyright file="CharacterCreationSubState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using WheelMUD.Core;

    /// <summary>Provides a sub state for the parent state.</summary>
    public abstract class CharacterCreationSubState
    {
        /// <summary>Initializes a new instance of the <see cref="CharacterCreationSubState"/> class.</summary>
        /// <param name="session">The session.</param>
        protected CharacterCreationSubState(Session session)
        {
            Session = session;
        }

        /// <summary>Gets or sets the state machine managing this creation state.</summary>
        public CharacterCreationStateMachine StateMachine { get; set; }

        /// <summary>Gets the session for this user connection.</summary>
        protected Session Session { get; private set; }

        /// <summary>Gets the character creation handler.</summary>
        protected CharacterCreationStateMachineManager Handler { get; private set; }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public abstract void ProcessInput(string command);

        /// <summary>Builds the player prompt for this stage of character creation.</summary>
        /// <returns>The build player prompt.</returns>
        public abstract string BuildPrompt();

        /// <summary>Called when this CharacterCreationSubState has become the active sub-state for a character creation session.</summary>
        /// <remarks>This is the first opportunity to Write introductory information for the sub-state (with a prompt, instead of defaulting to only printing a prompt) to the client.</remarks>
        public virtual void Begin()
        {
            Session.WritePrompt();
        }
    }
}