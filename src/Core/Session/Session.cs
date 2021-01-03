//-----------------------------------------------------------------------------
// <copyright file="Session.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using WheelMUD.Core.Events;
    using WheelMUD.Data;
    using WheelMUD.Interfaces;

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
                State = SessionStateManager.Instance.CreateDefaultState(this);

                // The very first time we create a session, it couldn't write the prompt due to State
                // not being set during that SessionState's construction, so force a prompt print here.
                Write(string.Empty, true);

                AtPrompt = false;
            }
        }

        /// <summary>The 'action received' event handler.</summary>
        public event ActionReceivedEventHandler ActionReceived;

        /// <summary>The 'session authenticated' event handler.</summary>
        public event SessionAuthenticatedEventHandler SessionAuthenticated;

        /// <summary>Gets the ID of the session.</summary>
        public string ID
        {
            get { return Connection.ID; }
        }

        /// <summary>Gets the terminal this session is using.</summary>
        public ITerminal Terminal
        {
            get { return Connection.Terminal; }
        }

        /// <summary>Gets or sets the player Thing attached to this session.</summary>
        public Thing Thing { get; set; }

        /// <summary>Gets the living behavior of the player attached to this session.</summary>
        public LivingBehavior LivingBehavior
        {
            get { return Thing != null ? Thing.Behaviors.FindFirst<LivingBehavior>() : null; }
        }

        /// <summary>Gets the connection for this session.</summary>
        public IConnection Connection { get; private set; }

        /// <summary>Gets or sets a value indicating whether the client is currently at a prompt or not.</summary>
        public bool AtPrompt { get; set; }

        /// <summary>Gets or sets the User authenticated to this session (if any).</summary>
        public User User { get; set; }

        /// <summary>Gets the last action input the session received.</summary>
        public ActionInput LastActionInput { get; private set; }

        /// <summary>Gets or sets the state of this session.</summary>
        public SessionState State { get; set; }

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
        public void SendPrompt()
        {
            if (!AtPrompt)
            {
                Connection.Send(Environment.NewLine + State.BuildPrompt());
            }
            else
            {
                Connection.Send(State.BuildPrompt());
            }

            AtPrompt = true;
        }

        /// <summary>Writes an empty string followed by a prompt.</summary>
        public void WritePrompt()
        {
            Write(string.Empty, true);
        }

        /// <summary>Write data to the users screen.</summary>
        /// <param name="data">The data to write.</param>
        /// <param name="sendPrompt">true to send the prompt after, false otherwise.</param>
        public void Write(string data, bool sendPrompt = true)
        {
            if (AtPrompt)
            {
                data = Environment.NewLine + data;
            }

            AtPrompt = false;
            if (sendPrompt)
            {
                string prompt = State != null ? State.BuildPrompt() : string.Empty;

                // Protection against double prompt.
                if (!data.EndsWith(Environment.NewLine + prompt))
                {
                    data = data + Environment.NewLine + prompt;
                }

                AtPrompt = true;
            }

            Connection.Send(data);
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