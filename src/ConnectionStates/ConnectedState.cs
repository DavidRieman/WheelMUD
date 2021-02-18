//-----------------------------------------------------------------------------
// <copyright file="ConnectedState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using WheelMUD.Core;
    using WheelMUD.Data;

    /// <summary>The 'connected' session state.</summary>
    [ExportSessionState(0)]
    public class ConnectedState : SessionState
    {
        /// <summary>Initializes a new instance of the <see cref="ConnectedState"/> class.</summary>
        /// <param name="session">The session entering this state.</param>
        public ConnectedState(Session session)
            : base(session)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ConnectedState"/> class.</summary>
        /// <remarks>This constructor is required to support MEF discovery as our default connection state.</remarks>
        public ConnectedState()
            : this(null)
        {
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            Session.AtPrompt = false;
            switch (command.ToLower())
            {
                case "":
                    break;
                case "new":
                    Session.State = new CreationState(Session);
                    Session.WritePrompt();
                    break;
                case "quit":
                case "close":
                case "exit":
                    Session.Connection.Disconnect();
                    break;
                default:
                    Session.State = new LoginState(Session, command);
                    Session.WritePrompt();
                    break;
            }
        }

        /// <summary>Builds the current prompt for this state.</summary>
        /// <returns>The current prompt.</returns>
        public override string BuildPrompt()
        {
            var creationType = AppConfigInfo.Instance.UserAccountIsPlayerCharacter ? "character name" : "user name";
            return string.Format("Enter your {0} or type NEW to create a new one.{1}> ", creationType, Environment.NewLine);
        }
    }
}