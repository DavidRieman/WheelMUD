//-----------------------------------------------------------------------------
// <copyright file="ConnectedState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;
using WheelMUD.Data;
using WheelMUD.Server;

namespace WheelMUD.ConnectionStates
{
    /// <summary>The 'connected' session state.</summary>
    [ExportSessionState(0)]
    public class ConnectedState : SessionState
    {
        // TODO: Adjust message selection when guest characters / guest accounts are an available App.config option.
        private static readonly string ConnectedPrompt = AppConfigInfo.Instance.UserAccountIsPlayerCharacter ?
            "Enter your character name or type NEW to create a new one: > " :
            "Enter your account name or type NEW to create a new account: > ";

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
            switch (command.ToLower())
            {
                case "":
                    break;
                case "new":
                    Session.SetState(new CreationState(Session));
                    break;
                case "quit":
                case "close":
                case "exit":
                    Session.Connection.Disconnect();
                    break;
                default:
                    Session.SetState(new LoginState(Session, command));
                    break;
            }
        }

        /// <summary>Builds the current prompt for this state.</summary>
        /// <returns>The current prompt.</returns>
        public override OutputBuilder BuildPrompt()
        {
            return new OutputBuilder(ConnectedPrompt.Length).Append(ConnectedPrompt);
        }
    }
}