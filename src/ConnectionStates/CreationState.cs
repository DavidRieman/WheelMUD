//-----------------------------------------------------------------------------
// <copyright file="CreationState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using WheelMUD.Core;
using WheelMUD.Data.Repositories;
using WheelMUD.Server;

namespace WheelMUD.ConnectionStates
{
    /// <summary>The 'character creation' session state.</summary>
    public class CreationState : SessionState
    {
        /// <summary>The character creation handler.</summary>
        private readonly CharacterCreationStateMachine subStateHandler;

        /// <summary>Initializes a new instance of the CreationState class.</summary>
        /// <param name="session">The session entering this state.</param>
        public CreationState(Session session)
            : base(session)
        {
            // Get the default CharacterCreationStateMachine via MEF to drive character creation sub-states.
            subStateHandler = CharacterCreationStateMachineManager.Instance.CreateDefaultCharacterCreationStateMachine(session);
            subStateHandler.CharacterCreationAborted += SubState_CharacterCreationAborted;
            subStateHandler.CharacterCreationCompleted += SubState_CharacterCreationCompleted;
        }

        public override void Begin()
        {
            // Beginning the character creation sub-state handler should automatically set and Begin the first sub-state, and print the associated input prompt.
            subStateHandler.Begin();
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            subStateHandler.ProcessInput(command);
        }

        public override OutputBuilder BuildPrompt()
        {
            if (subStateHandler != null && subStateHandler.CurrentStep != null)
            {
                return subStateHandler.CurrentStep.BuildPrompt();
            }

            return new OutputBuilder(2).Append("> ");
        }

        /// <summary>Called upon the completion of character creation.</summary>
        /// <param name="newCharacter">The new Character.</param>
        private static void SubState_CharacterCreationCompleted(Session session)
        {
            var user = session.User;
            var character = session.Thing;
            character.Id ??= $"char/{character.Name.ToLower()}";

            var playerBehavior = character.FindBehavior<PlayerBehavior>();
            playerBehavior.History.Created = playerBehavior.History.LastLogIn = DateTime.Now;

            // Ensure the User tracks this character ID as one of their characters
            user.AddPlayerCharacter(character.Id);

            // Save the character first so we can use the auto-assigned unique identity.
            // We could have used character.Save() but this uses the same session for storing User too.
            DocumentRepository.SaveAll(character, user);
            session.WriteLine($"Saved {character.Name}.", false);

            if (playerBehavior.LogIn(session))
            {
                // Already authenticated (the user just created user name and password); get in the game now.
                session.SetState(new PlayingState(session));
            }
            else
            {
                session.WriteLine("Character was created but could not be logged in right now. Disconnecting.");
                session.SetState(null);
                session.Connection.Disconnect();
            }
        }

        /// <summary>Called upon the abortion of character creation.</summary>
        private void SubState_CharacterCreationAborted()
        {
            Session.SetState(new ConnectedState(Session));
        }
    }
}