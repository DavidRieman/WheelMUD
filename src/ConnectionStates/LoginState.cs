//-----------------------------------------------------------------------------
// <copyright file="LoginState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The 'login' session state.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using WheelMUD.Core;
    using WheelMUD.Data.Repositories;

    /// <summary>The 'login' session state.</summary>
    public class LoginState : SessionState
    {
        /// <summary>Initializes a new instance of the LoginState class.</summary>
        /// <param name="session">The session entering this state.</param>
        public LoginState(Session session)
            : base(session)
        {
            session.Write("Please enter your password:");
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            this.Session.AtPrompt = false;
            if (command != string.Empty)
            {
                // @@@ TODO: This should be encrypted immediately; no properties should house plain text passwords.
                this.Password = command;
                if (this.Authenticate())
                {
                    this.Session.State = new PlayingState(this.Session);
                    this.Session.AuthenticateSession();
                }
                else
                {
                    this.Session.Write("Incorrect username or password.\r\n\r\n", false);
                    this.Session.InformSubscribedSystem(this.Session.ID + " failed to log in");
                    this.Session.State = new ConnectedState(this.Session);
                    this.Session.WritePrompt();
                }
            }
        }

        public override string BuildPrompt()
        {
            return "> ";
        }

        /// <summary>Authenticate the user name and password supplied.</summary>
        /// <returns>True if authenticated, else false.</returns>
        private bool Authenticate()
        {
            return PlayerRepository.Authenticate(this.Session.UserName, this.Password);
        }
    }
}