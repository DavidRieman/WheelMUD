//-----------------------------------------------------------------------------
// <copyright file="CreationState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The 'character creation' session state.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Data.Entities;
    using WheelMUD.Utilities;

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
            this.subStateHandler = CharacterCreationStateMachineManager.Instance.CreateDefaultCharacterCreationStateMachine(session);
            this.subStateHandler.CharacterCreationAborted += this.SubState_CharacterCreationAborted;
            this.subStateHandler.CharacterCreationCompleted += this.SubState_CharacterCreationCompleted;
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            this.Session.AtPrompt = false;
            this.subStateHandler.ProcessInput(command);
        }

        public override string BuildPrompt()
        {
            if (this.subStateHandler != null && this.subStateHandler.CurrentStep != null)
            {
                return this.subStateHandler.CurrentStep.BuildPrompt();
            }

            return "> ";
        }

        /// <summary>Called upon the completion of character creation.</summary>
        /// <param name="newCharacter">The new Character.</param>
        private void SubState_CharacterCreationCompleted(Thing newCharacter)
        {
            // Attach the new character to the current session.
            this.Session.Thing = this.subStateHandler.NewCharacter;
            this.Session.Write(string.Format("Saving player {0}...", newCharacter.Name), false);

            // Prepare the default options and new data that will go in the special player DB.
            string currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var playerBehavior = newCharacter.FindBehavior<PlayerBehavior>();
            playerBehavior.PlayerData = new PlayerRecord
            {
                UserName = newCharacter.Name,
                Password = playerBehavior.Password,
                DisplayName = newCharacter.Name,
                Description = newCharacter.Description,
                CurrentRoomID = MudEngineAttributes.Instance.DefaultRoomID,
                LastLogin = currentTime,
                CreateDate = currentTime,
                LastIPAddress = this.Session.Connection.CurrentIPAddress.ToString(),
                WantAnsi = true,
                WantMCCP = false,
                WantMXP = true
            };

            // If the player isn't located anywhere yet, try to drop them in the default room.
            if (newCharacter.Parent == null)
            {
                this.PlaceCharacterInDefaultRoom(newCharacter);
            }

            // Saving the player's Thing should save everything we need to now.
            newCharacter.Save();
            this.Session.Write(string.Format(" Done saving player {0}\n\n", newCharacter.Name), false);

            // Automatically authenticate (the user just created username and password) and
            // get in-game when character creation is completed.)
            this.Session.UserName = newCharacter.Name;
            this.Session.State = new PlayingState(this.Session);
            this.Session.AuthenticateSession();
        }

        /// <summary>Place the specified character in the "default" room.</summary>
        /// <param name="character">The character to place.</param>
        private void PlaceCharacterInDefaultRoom(Thing character)
        {
            character.Parent = (from t in ThingManager.Instance.Things
                                where t.ID == "room/" + MudEngineAttributes.Instance.DefaultRoomID
                                select t).FirstOrDefault();
            if (character.Parent == null)
            {
                throw new InvalidOperationException("Could not place a new character in the default room!");
            }
        }

        /// <summary>Called upon the abortion of character creation.</summary>
        private void SubState_CharacterCreationAborted()
        {
            this.Session.State = new ConnectedState(this.Session);
            this.Session.WritePrompt();
        }
    }
}