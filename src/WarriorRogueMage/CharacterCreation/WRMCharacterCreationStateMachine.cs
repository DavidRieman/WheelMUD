// <copyright file="WRMCharacterCreationStateMachine.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   State machine for creating a new character using Warrior, Rogue, and Mage
//   rules.
//   Created By : Fastalanasa
//   Created On : May 12, 2011.
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.CharacterCreation
{
    using System;
    using WheelMUD.ConnectionStates;
    using WheelMUD.Core;

    /// <summary>Warrior, Rogue, Mage state machine for creating a new character.</summary>
    [ExportCharacterCreationStateMachine(200)]
    public class WRMCharacterCreationStateMachine : CharacterCreationStateMachine
    {
        /// <summary>Initializes a new instance of the <see cref="WRMCharacterCreationStateMachine" /> class.</summary>
        /// <param name="session">The session.</param>
        public WRMCharacterCreationStateMachine(Session session) 
            : base(session)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WRMCharacterCreationStateMachine" /> class.</summary>
        public WRMCharacterCreationStateMachine()
            : this(null)
        {
        }

        /// <summary>Gets the next step in the creation process.</summary>
        /// <param name="current">The current (just executed step).</param>
        /// <param name="previousStatus">Whether the current step passed or failed.</param>
        /// <returns>The next step in the character creation process, or null if it is finished.</returns>
        public override CharacterCreationSubState GetNextStep(CharacterCreationSubState current, StepStatus previousStatus)
        {
            // entry point of the state machine
            if (current == null)
            {
                return new ConfirmCreationEntryState(this.Session);
            }

            if (previousStatus == StepStatus.Success)
            {
                return this.AdvanceState(current);
            }
            else
            {
                return this.RegressState(current);
            }
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
                return new PickGenderState(this.Session);
            }
            else if (current is PickGenderState)
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
                return new SetAttributesState(this.Session);
            }
            else if (current is SetAttributesState)
            {
                return new PickSkillsState(this.Session);
            }
            else if (current is PickSkillsState)
            {
                return new PickTalentsState(this.Session);
            }
            else if (current is PickTalentsState)
            {
                return new PickRaceState(this.Session);
            }
            else if (current is PickRaceState)
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
