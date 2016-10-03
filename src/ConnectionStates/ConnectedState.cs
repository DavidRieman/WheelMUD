//-----------------------------------------------------------------------------
// <copyright file="ConnectedState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The 'connected' session state.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using WheelMUD.Core;

    /// <summary>The 'connected' session state.</summary>
    [ExportSessionState(100)]
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
            this.Session.AtPrompt = false;
            switch (command.ToLower())
            {
                case "":
                    break;
                case "new":
                    this.Session.State = new CreationState(this.Session);
                    this.Session.WritePrompt();
                    break;
                case "quit":
                case "close":
                case "exit":
                    this.Session.Connection.Disconnect();
                    break;
                default:
                    this.Session.UserName = command;
                    this.Session.State = new LoginState(this.Session);
                    this.Session.WritePrompt();
                    break;
            }
        }

        /// <summary>Builds the current prompt for this state.</summary>
        /// <returns>The current prompt.</returns>
        public override string BuildPrompt()
        {
            return string.Format("Enter your character name or type NEW to create a new one.{0}> ", Environment.NewLine);
        }
    }
}