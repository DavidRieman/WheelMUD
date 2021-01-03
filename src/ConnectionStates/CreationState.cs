//-----------------------------------------------------------------------------
// <copyright file="CreationState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using WheelMUD.Core;
    using WheelMUD.Data;

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

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            Session.AtPrompt = false;
            subStateHandler.ProcessInput(command);
        }

        public override string BuildPrompt()
        {
            if (subStateHandler != null && subStateHandler.CurrentStep != null)
            {
                return subStateHandler.CurrentStep.BuildPrompt();
            }

            return "> ";
        }

        /// <summary>Called upon the completion of character creation.</summary>
        /// <param name="newCharacter">The new Character.</param>
        private static void SubState_CharacterCreationCompleted(Session session)
        {
            var nl = Environment.NewLine;
            var user = session.User;
            var character = session.Thing;

            session.Write(string.Format("Saving character {0}.{1}", character.Name, nl));
            using (var docSession = Helpers.OpenDocumentSession())
            {
                // Save the character first so we can use the auto-assigned unique identity.
                // We could have used playerBehavior.SavePlayer but this uses the same session for storing User too.
                docSession.Store(character);

                // Ensure the User tracks this character ID as one of their characters
                user.AddPlayerCharacter(character.Id);
                docSession.Store(user);
                docSession.SaveChanges();
            }
            session.Write(string.Format("Done saving {0}.{1}", character.Name, nl));

            var playerBehavior = character.Behaviors.FindFirst<PlayerBehavior>();
            if (playerBehavior.LogIn(session))
            {
                // Automatically authenticate (the user just created username and password) and
                // get in-game when character creation is completed.)
                session.AuthenticateSession();
                session.State = new PlayingState(session);
            }
            else
            {
                session.Write("Character was created but could not be logged in right now. Disconnecting.");
                session.State = null;
                session.Connection.Disconnect();
            }
        }

        /// <summary>Called upon the abortion of character creation.</summary>
        private void SubState_CharacterCreationAborted()
        {
            Session.State = new ConnectedState(Session);
            Session.WritePrompt();
        }
    }
}