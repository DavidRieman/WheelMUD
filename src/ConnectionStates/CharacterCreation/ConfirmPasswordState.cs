//-----------------------------------------------------------------------------
// <copyright file="ConfirmPasswordState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Character creation state used to confirm a password for the new character.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System.Text;
    using WheelMUD.Core;

    /// <summary>Character creation state used to confirm a password for the new character.</summary>
    public class ConfirmPasswordState : CharacterCreationSubState
    {
        /// <summary>Initializes a new instance of the <see cref="ConfirmPasswordState"/> class.</summary>
        /// <param name="session">The session.</param>
        public ConfirmPasswordState(Session session)
            : base(session)
        {
        }

        /// <summary>ProcessInput is used to receive the user input during this state.</summary>
        /// <param name="command">The command text to be processed.</param>
        public override void ProcessInput(string command)
        {
            var playerBehavior = this.StateMachine.NewCharacter.Behaviors.FindFirst<PlayerBehavior>();

            // Do not use the command parameter here. It is trimmed of whitespace, which will inhibit the use of passwords 
            // with whitespace on either end. Instead we need to respect the raw line of input for password entries.
            if (playerBehavior.PasswordMatches(this.Session.Connection.LastRawInput))
            {
                if (this.AddCharacterToDatabase(this.Session))
                {
                    // Proceed to automatic login step.
                    this.StateMachine.HandleNextStep(this, StepStatus.Success);
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.Append("There was a problem saving this character to the database.\r\n");

                    this.Session.Write(sb.ToString(), false);
                    this.StateMachine.AbortCreation();
                }
            }
            else
            {
                this.Session.Write("I am afraid the passwords entered do not match.\r\n", false);
                this.StateMachine.HandleNextStep(this, StepStatus.Failure);
            }
        }

        public override string BuildPrompt()
        {
            return "Please retype your password.\n> ";
        }

        /// <summary>Add this character to the database.</summary>
        /// <param name="session">The session for the player being created.</param>
        /// <returns>True if successful, else false.</returns>
        private bool AddCharacterToDatabase(Session session)
        {
            // @@@ TODO: Save simply with: this.StateMachine.NewCharacter.Save();
            return true;

            //var repository = new PlayerRepository();
            //this.ConfigurePlayer(ref record);
            //record.LastIPAddress = session.Connection.CurrentIPAddress;
            //try
            //{
            //    repository.Add(record);

            //    if (record.ID > 0)
            //    {
            //        var roleRepository = new RoleRepository();
            //        var roleRecord = roleRepository.GetRoleByName("player");
            //        var playerRoleRecord = new PlayerRoleRecord();
            //        playerRoleRecord.PlayerID = record.ID;
            //        playerRoleRecord.RoleID = roleRecord.ID;
            //        var repo = new PlayerRoleRepository();
            //        repo.Add(playerRoleRecord);
            //    }
            //}
            //catch
            //{
            //    retval = false;
            //}
        }

        /// <summary>Fills up the player record with some default data.</summary>
        /// <param name="playerRecord">The <see cref="PlayerRecord"/> that need default data.</param>
        //private void ConfigurePlayer(ref PlayerRecord playerRecord)
        //{
        //    var config = MudEngineAttributes.Instance;

        //    playerRecord.CreateDate = DateTime.Now.ToUniversalTime().ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";
        //    playerRecord.LastLogin = DateTime.Now.ToUniversalTime().ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";
        //    playerRecord.CurrentRoomID = config.DefaultRoomID;
        //    playerRecord.WantAnsi = true;
        //    playerRecord.WantMXP = true;
        //    playerRecord.WantMCCP = false;
        //    playerRecord.Prompt = ">";

        //    // Temporary until chargen steps get created for these properties.
        //    playerRecord.ProfessionID = 0;
        //    playerRecord.RaceID = 0;
        //    playerRecord.SpouseID = 0;
        //    playerRecord.ClanID = 0;
        //    playerRecord.GenderID = 1;
        //    playerRecord.Age = 18;
        //}
    }
}