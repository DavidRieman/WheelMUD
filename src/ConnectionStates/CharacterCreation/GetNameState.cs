//-----------------------------------------------------------------------------
// <copyright file="GetNameState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using WheelMUD.Core;
    using WheelMUD.Data.Repositories;

    /// <summary>This is the state for new character name entry as supplied by a player.</summary>
    public class GetNameState : CharacterCreationSubState
    {
        /// <summary>The minimum allowed length for a new character's name.</summary>
        private const int MinimumNameLength = 2;

        /// <summary>The maximum allowed length for a new character's name.</summary>
        private const int MaximumNameLength = 20;

        /// <summary>Initializes a new instance of the <see cref="GetNameState"/> class.</summary>
        /// <param name="session">The session.</param>
        public GetNameState(Session session)
            : base(session)
        {
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            if (this.ValidateCharacterName(ref command))
            {
                // The name is valid, but has it been taken already?
               var repository = new PlayerRepository();

                if (repository.GetPlayerByUserName(command) == null)
                {
                    if (this.StateMachine != null)
                    {
                        this.StateMachine.NewCharacter.Name = command;
                        this.StateMachine.HandleNextStep(this, StepStatus.Success);
                    }
                }
                else
                {
                    this.Session.Write("I'm sorry, that name is already taken. Please choose another.");
                }
            }
        }

        public override string BuildPrompt()
        {
            return "Please enter a name for your character.\n> ";
        }

        /// <summary>Validate a proposed new character name against some basic criteria.</summary>
        /// <remarks>
        /// Created: April 2009 by Karak.
        /// TODO: Rudimentary checks for profanity/slander and such could be done here.
        ///       This could be done in conjunction with a persisted list (DB-stored)
        ///       of banned words to scan for, which could be expanded by admins/etc.
        ///       (Such a system could be employed for chat systems as well.)
        ///       Additionally, it may be appropriate to automatically ban character 
        ///       names that are duplicates of mobs/items like "Goblin" or "Sword",
        ///       to prevent possible confusion of new players and such.</remarks>
        /// <param name="newCharacterName">The proposed new character's name.</param>
        /// <returns>True if the character name is acceptable so far, else false.</returns>
        private bool ValidateCharacterName(ref string newCharacterName)
        {
            // Rule: Character name may not be missing or empty.
            if (string.IsNullOrEmpty(newCharacterName))
            {
                this.Session.Write("You must supply a character name.");
                return false;
            }
            
            // Rule: Character name can not be too short nor too long.
            if (newCharacterName.Length < MinimumNameLength || newCharacterName.Length > MaximumNameLength)
            {
                this.Session.Write(string.Format("Name must be between {0} and {1} letters long. Please choose another.", MinimumNameLength, MaximumNameLength));
                return false;
            }
            
            // Rule: Character name may not include non-alphabetical characters.
            foreach (char c in newCharacterName.ToCharArray())
            {
                if (!char.IsLetter(c))
                {
                    this.Session.Write("Character name must include only letters. Please choose another.");
                    return false;
                }
            }

            // Rule: Character name may not only consist of uppercase characters.
            if (newCharacterName == newCharacterName.ToUpper())
            {
                this.Session.Write("Character name may not be entirely uppercase. Please choose another.");
                return false;
            }

            // Rule: Character name may not only consist of lowercase characters.
            // However, if it does, we will adjust the input for them, assuming the 
            // standard character name which starts with one capital letter.
            if (newCharacterName == newCharacterName.ToLower())
            {
                newCharacterName = newCharacterName.Substring(0, 1).ToUpper() + newCharacterName.Substring(1);
            }
            
            return true;
        }
    }
}