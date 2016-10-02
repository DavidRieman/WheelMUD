// <copyright file="DefaultCharacterCreationStateMachine.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Default state machine for creating a new character
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using WheelMUD.Core;

    /// <summary>Default state machine for creating a new character.</summary>
    [ExportCharacterCreationStateMachine(100)]
    public class DefaultCharacterCreationStateMachine : CharacterCreationStateMachine
    {
        /// <summary>Initializes a new instance of the <see cref="DefaultCharacterCreationStateMachine"/> class.</summary>
        /// <param name="session">The session.</param>
        public DefaultCharacterCreationStateMachine(Session session) 
            : base(session)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DefaultCharacterCreationStateMachine"/> class.</summary>
        public DefaultCharacterCreationStateMachine()
            : this(null)
        {
        }

        /// <summary>Gets the next step in the creation process.</summary>
        /// <param name="current">The current (just executed step)</param>
        /// <param name="previousStatus">Whether the current step passed or failed</param>
        /// <returns>The next step in the character creation process, or null if it is finished</returns>
        public override CharacterCreationSubState GetNextStep(CharacterCreationSubState current, StepStatus previousStatus)
        {
            // If there is no state yet, set up the initial character creation state.
            if (current == null)
            {
                return new ConfirmCreationEntryState(this.Session);
            }

            // Otherwise either go forward to the next state, or back to the previous state if requested.
            return previousStatus == StepStatus.Success ? this.AdvanceState(current) : this.RegressState(current);
        }

        private CharacterCreationSubState AdvanceState(CharacterCreationSubState current)
        {
            // This character creation state machine can return actual creation state objects - if someone
            // were to expand and add new creation state(s) that are not MUD-agnostic, then they should also
            // add and use their own CreationStateMachine handling those states instead of this default one;
            // they could of course reuse some/all of the states below in addition to their own.
            if (current is ConfirmCreationEntryState)
            {
                return new GetNameState(this.Session);
            }
            else if (current is GetNameState)
            {
                return new GetDescriptionState(this.Session);
            }
            else if (current is GetDescriptionState)
            {
                return new GetPasswordState(this.Session);
            }
            else if (current is GetPasswordState)
            {
                return new ConfirmPasswordState(this.Session);
            }
            else if (current is ConfirmPasswordState)
            {
                // We are done with character creation!
                return null;
            }

            throw new InvalidOperationException("The character state machine does not know how to calculate the next step after '" + current.GetType().Name + "' succeeds");
        }

        private CharacterCreationSubState RegressState(CharacterCreationSubState current)
        {
            if (current is ConfirmPasswordState)
            {
                // If password confirmation failed, try selecting a new password.
                return new GetPasswordState(this.Session);
            }

            throw new InvalidOperationException("The character state machine does not know how to calculate the next step after '" + current.GetType().Name + "' fails");
        }
    }
}
