//-----------------------------------------------------------------------------
// <copyright file="GetNameState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using WheelMUD.Core;
using WheelMUD.Data;
using WheelMUD.Data.Repositories;
using WheelMUD.Server;

namespace WheelMUD.ConnectionStates
{
    /// <summary>This is the state for new character name entry as supplied by a player.</summary>
    public class GetNameState : CharacterCreationSubState
    {
        private static readonly OutputBuilder prompt = new OutputBuilder().Append(AppConfigInfo.Instance.UserAccountIsPlayerCharacter ?
            "Please enter a name for your character: > " :
            "Please enter a user account name or email address: > ");

        /// <summary>The minimum allowed length for a new user login name.</summary>
        private const int MinimumUserNameLength = 3;

        /// <summary>The minimum allowed length for a new player character name.</summary>
        private const int MinimumPlayerCharacterNameLength = 3;

        /// <summary>The maximum allowed length for a new user login name.</summary>
        private const int MaximumUserNameLength = 45;

        /// <summary>The maximum allowed length for a new player character name.</summary>
        private const int MaximumPlayerCharacterNameLength = 20;

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
            if (ValidateUserName(ref command))
            {
                // The name is valid, but has it been taken already?
                if (PlayerRepositoryExtensions.UserNameExists(command))
                {
                    Session.WriteLine("I'm sorry, that name is already taken. Please choose another.");
                }
                else if (StateMachine != null)
                {
                    Session.User.UserName = command;
                    if (AppConfigInfo.Instance.UserAccountIsPlayerCharacter)
                    {
                        Session.Thing.Name = command;
                    }
                    else
                    {
                        throw new NotImplementedException("Need to ensure correct flow into character selection state.");
                    }
                    StateMachine.HandleNextStep(this, StepStatus.Success);
                }
            }
        }

        public override OutputBuilder BuildPrompt()
        {
            return prompt;
        }

        /// <summary>Validate a proposed new user name against some basic criteria.</summary>
        /// <remarks>
        /// Note that the user name is not always the same as the player character name, and should not be shown to
        /// other players. As such, we don't necessarily need the same level of protection as player names.
        /// TODO: Rudimentary checks for profanity/slander and such could be done here if this is also the player name.
        ///       This could be done in conjunction with a persisted list (DB-stored)  of banned words to scan for,
        ///       which could be expanded by admins/etc. (Such a system could be employed for chat systems as well.)
        ///       Additionally, it may be appropriate to automatically ban character names that are duplicates of
        ///       mobs/items like "Goblin" or "Sword", to prevent possible confusion of new players and such.
        /// </remarks>
        /// <param name="newUserName">The proposed new user name.</param>
        /// <returns>True if the user name is acceptable for usage, else false.</returns>
        private bool ValidateUserName(ref string newUserName)
        {
            bool isAlsoPlayerName = AppConfigInfo.Instance.UserAccountIsPlayerCharacter;

            // Rule: User and character names may not be missing or empty.
            if (string.IsNullOrEmpty(newUserName))
            {
                Session.WriteLine("You must supply a name.");
                return false;
            }

            // Rule: Player character name can not be too short nor too long.
            // Player character name is usually more restrictive in length since other players have to type it, so
            // it is checked first to have consistent messagine each time you try a really long one in sequence.
            if (isAlsoPlayerName && (newUserName.Length < MinimumPlayerCharacterNameLength || newUserName.Length > MaximumPlayerCharacterNameLength))
            {
                Session.WriteLine($"Player name must be between {MinimumPlayerCharacterNameLength} and {MaximumPlayerCharacterNameLength} letters long. Please choose another.");
                return false;
            }

            // Rule: User name can not be too short nor too long.
            if (newUserName.Length < MinimumUserNameLength || newUserName.Length > MaximumUserNameLength)
            {
                Session.WriteLine($"User name must be between {MinimumUserNameLength} and {MaximumUserNameLength} letters long. Please choose another.");
                return false;
            }

            // We should allow people to use things like their email address as a user name, but not as a character name.
            // TODO: Consider other restrictions for user name though?
            if (isAlsoPlayerName)
            {
                // Rule: Character name may not include non-alphabetical characters.
                foreach (char c in newUserName.ToCharArray())
                {
                    if (!char.IsLetter(c))
                    {
                        Session.WriteLine("Character name must include only letters. Please choose another.");
                        return false;
                    }
                }

                // If the administrator wishes all names to be uniform single-capital letters, fix the name now.
                // This will cause additional validations about all-capitals / all-lowercase to pass automatically.
                // Otherwise, if false, the user may use names like "McGraw" but now "MCGRAW" etc.
                if (AppConfigInfo.Instance.PlayerCharacterNamesMustUseSingleCapital)
                {
                    newUserName = newUserName[..1].ToUpper() + newUserName[1..].ToLower();
                }

                int capitalCount = 0;
                int vowelCount = 0;
                foreach (var c in newUserName.ToCharArray())
                {
                    capitalCount += char.IsUpper(c) ? 1 : 0;
                    if (c == 'a' || c == 'A' || c == 'e' || c == 'E' || c == 'i' || c == 'I' ||
                        c == 'o' || c == 'O' || c == 'u' || c == 'U' || c == 'y' || c == 'Y')
                    {
                        vowelCount++;
                    }
                }

                // Rule: Character name must include at least one vowel or equivalent character.
                if (vowelCount <= 0)
                {
                    Session.WriteLine("Character name may not exclude vowels. Please choose another.");
                    return false;
                }

                // Rule: Character name may not only consist too heavily of uppercase characters.
                if (capitalCount > 1 && capitalCount >= newUserName.Length / 2)
                {
                    Session.WriteLine("Character name may not be heavily uppercased. Please choose another.");
                    return false;
                }

                // Rule: Character name may not only consist of lowercase characters.
                // However, if it does, we will adjust the name automatically with a single starting capital.
                if (newUserName == newUserName.ToLower())
                {
                    newUserName = newUserName[..1].ToUpper() + newUserName[1..];
                }
            }

            return true;
        }
    }
}