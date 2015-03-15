//-----------------------------------------------------------------------------
// <copyright file="Session.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A player's session.
//   Created: September 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using WheelMUD.Core.Events;
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
                this.Connection = connection;
                this.State = SessionStateManager.Instance.CreateDefaultState(this);
                
                // The very first time we create a session, it couldn't write the prompt due to this.State
                // not being set during that SessionState's construction, so force a prompt print here.
                this.Write(string.Empty, true);

                this.AtPrompt = false;
                this.UserName = string.Empty;
            }
        }

        /// <summary>The 'action received' event handler.</summary>
        public event ActionReceivedEventHandler ActionReceived;

        /// <summary>The 'session authenticated' event handler.</summary>
        public event SessionAuthenticatedEventHandler SessionAuthenticated;

        /// <summary>Gets the ID of the session.</summary>
        public string ID
        {
            get { return this.Connection.ID; }
        }

        /// <summary>Gets the terminal this session is using.</summary>
        public ITerminal Terminal
        {
            get { return this.Connection.Terminal; }
        }

        /// <summary>Gets or sets the player Thing attached to this session.</summary>
        public Thing Thing { get; set; }

        /// <summary>Gets the living behavior of the player attached to this session.</summary>
        public LivingBehavior LivingBehavior 
        { 
            get { return this.Thing != null ? this.Thing.Behaviors.FindFirst<LivingBehavior>() : null; }
        }

        /// <summary>Gets the connection for this session.</summary>
        public IConnection Connection { get; private set; }

        /// <summary>Gets or sets a value indicating whether the client is currently at a prompt or not.</summary>
        public bool AtPrompt { get; set; }

        /// <summary>Gets or sets the username this session used to identify themselves with at login.</summary>
        public string UserName { get; set; }

        /// <summary>Gets the last action input the session received.</summary>
        public ActionInput LastActionInput { get; private set; }

        /// <summary>Gets or sets the state of this session.</summary>
        public SessionState State { get; set; }

        /// <summary>Provides authentication services for this session.</summary>
        public void AuthenticateSession()
        {
            if (this.SessionAuthenticated != null)
            {
                this.SessionAuthenticated(this);
            }
        }

        /// <summary>Passes the input up the chain for processing.</summary>
        /// <param name="input">The input to pass up the chain</param>
        public void ProcessCommand(string input)
        {
            this.State.ProcessInput(input);
        }

        /// <summary>Sends the prompt to the connection.</summary>
        public void SendPrompt()
        {
            if (!this.AtPrompt)
            {
                this.Connection.Send(Environment.NewLine + this.State.BuildPrompt());
            }
            else
            {
                this.Connection.Send(this.State.BuildPrompt());
            }
            
            this.AtPrompt = true;
        }

        /// <summary>Writes an empty string followed by a prompt.</summary>
        public void WritePrompt()
        {
            this.Write(string.Empty, true);
        }

        /// <summary>Write data to the users screen.</summary>
        /// <param name="data">The data to write.</param>
        /// <param name="sendPrompt">true to send the prompt after, false otherwise.</param>
        public void Write(string data, bool sendPrompt = true)
        {
            if (this.AtPrompt)
            {
                data = Environment.NewLine + data;
            }

            this.AtPrompt = false;
            if (sendPrompt)
            {
                string prompt = this.State != null ? this.State.BuildPrompt() : string.Empty;

                // Protection against double prompt.
                if (!data.EndsWith(Environment.NewLine + prompt))
                {
                    data = data + Environment.NewLine + prompt;
                }
                
                this.AtPrompt = true;
            }

            this.Connection.Send(data);
        }

        /// <summary>Place an action on the command queue for execution.</summary>
        /// <param name="actionInput">The action input to attempt to execute.</param>
        public void ExecuteAction(ActionInput actionInput)
        {
            this.LastActionInput = actionInput;
            if (this.ActionReceived != null)
            {
                this.ActionReceived((IController)this, actionInput);
            }
        }

        /// <summary>Subscribe to receive system updates from this system.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        public void SubscribeToSystem(ISubSystemHost sender)
        {
            this.host = sender;
        }

        /// <summary>Removes subscriptions from the system.</summary>
        public void UnsubscribeToSystem()
        {
            this.host = null;
        }

        /// <summary>Inform subscribed system(s) of the specified update.</summary>
        /// <param name="msg">The message to be sent to subscribed system(s).</param>
        public void InformSubscribedSystem(string msg)
        {
            this.host.UpdateSubSystemHost(this, msg);
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