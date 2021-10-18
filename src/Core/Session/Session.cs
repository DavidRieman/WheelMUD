//-----------------------------------------------------------------------------
// <copyright file="Session.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using WheelMUD.Data;
using WheelMUD.Interfaces;
using WheelMUD.Server;
using WheelMUD.Server.Interfaces;
using WheelMUD.Utilities;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>The 'session authenticated' event handler delegate.</summary>
    /// <param name="session">The session that was authenticated.</param>
    public delegate void SessionAuthenticatedEventHandler(Session session);

    /// <summary>A player's session.</summary>
    public class Session : IController, ISubSystem
    {
        /// <summary>This session's subscribed sub system host.</summary>
        private ISubSystemHost host;

        /// <summary>Initializes a new instance of the Session class.</summary>
        /// <param name="connection">The connection this session is based on.</param>
        public Session(IConnection connection)
        {
            if (connection != null)
            {
                Connection = connection;
                SetState(SessionStateManager.Instance.CreateDefaultState(this));
            }
        }

        /// <summary>The 'action received' event handler.</summary>
        public event ActionReceivedEventHandler ActionReceived;

        /// <summary>The 'session authenticated' event handler.</summary>
        public event SessionAuthenticatedEventHandler SessionAuthenticated;

        /// <summary>Gets the ID of the session.</summary>
        public string ID => Connection.ID;

        /// <summary>Gets the terminal this session is using.</summary>
        public TerminalOptions TerminalOptions => Connection.TerminalOptions;

        /// <summary>Gets or sets the player Thing attached to this session.</summary>
        public Thing Thing { get; set; }

        /// <summary>Gets the living behavior of the player attached to this session.</summary>
        public LivingBehavior LivingBehavior => Thing?.FindBehavior<LivingBehavior>();

        /// <summary>Gets the connection for this session.</summary>
        public IConnection Connection { get; private set; }

        /// <summary>Gets a value indicating whether the output sent to the client is currently at a prompt or not.</summary>
        public bool AtPrompt =>
            // As prompts let the cursor rest on the same line, this can be indicated via the connection's cursor position; any
            // output other than prompts should occur as a full line that terminates with NewLine.
            !Connection.AtNewLine;

        /// <summary>Gets or sets the User authenticated to this session (if any).</summary>
        public User User { get; set; }

        /// <summary>Gets the last action input the session received.</summary>
        public ActionInput LastActionInput { get; private set; }

        /// <summary>Gets or sets the state of this session.</summary>
        public SessionState State { get; private set; }

        /// <summary>Sets a new SessionState for this Session, and tells it to begin.</summary>
        public void SetState(SessionState newState)
        {
            State = newState;
            newState?.Begin();
        }

        /// <summary>Provides authentication services for this session.</summary>
        public void AuthenticateSession()
        {
            SessionAuthenticated?.Invoke(this);
        }

        /// <summary>Passes the input up the chain for processing.</summary>
        /// <param name="input">The input to pass up the chain</param>
        public void ProcessCommand(string input)
        {
            State.ProcessInput(input);
        }

        /// <summary>Sends the prompt to the connection.</summary>
        public void WritePrompt()
        {
            var prompt = State.BuildPrompt().Parse(Connection.TerminalOptions);
            
            if (!AtPrompt)
            {
                prompt = AnsiSequences.NewLine + prompt;
            }
            
            Connection.Send(prompt);
        }

        public void WriteLine(string singleLineOutput, bool sendPrompt = true)
        {
            var buffer = singleLineOutput.ToCharArray();
            FinalWrite(OutputParser.Parse(buffer, buffer.Length, Connection.TerminalOptions), sendPrompt);
        }

        public void Write(OutputBuilder output, bool sendPrompt = true)
        {
            FinalWrite(output.Parse(Connection.TerminalOptions), sendPrompt);
        }

        /// <summary>Write data to the users screen.</summary>
        /// <param name="data">The string of text to write to this session's connected user.</param>
        /// <param name="sendPrompt">true to send the prompt after, false otherwise.</param>
        private void FinalWrite(string data, bool sendPrompt)
        {
            // If our last output to this session was printing the prompt, inject a new line in front of this next set of data,
            // so the data won't potentially start printing on the same line as the prompt.
            if (AtPrompt)
            {
                data = AnsiSequences.NewLine + data;
            }

            if (sendPrompt)
            {
                if (!data.EndsWith(AnsiSequences.NewLine))
                    data += AnsiSequences.NewLine;
                data += State?.BuildPrompt()?.Parse(Connection.TerminalOptions);
            }

            // If a particular state doesn't support the paging commands (like "m" or "more") then we should force sending all
            // data instead of potentially printing paging output that isn't supported in the current state.
            Connection.Send(data, !State.SupportsPaging);
        }

        /// <summary>Place an action on the command queue for execution.</summary>
        /// <param name="actionInput">The action input to attempt to execute.</param>
        public void ExecuteAction(ActionInput actionInput)
        {
            LastActionInput = actionInput;
            ActionReceived?.Invoke((IController)this, actionInput);
        }

        /// <summary>Subscribe to receive system updates from this system.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        public void SubscribeToSystem(ISubSystemHost sender)
        {
            host = sender;
        }

        /// <summary>Removes subscriptions from the system.</summary>
        public void UnsubscribeToSystem()
        {
            host = null;
        }

        /// <summary>Inform subscribed system(s) of the specified update.</summary>
        /// <param name="msg">The message to be sent to subscribed system(s).</param>
        public void InformSubscribedSystem(string msg)
        {
            host.UpdateSubSystemHost(this, msg);
        }

        /// <summary>Starts this session.</summary>
        public void Start()
        {
            throw new NotImplementedException();
        }

        /// <summary>Stops this session.</summary>
        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}