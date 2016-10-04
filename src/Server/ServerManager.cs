//-----------------------------------------------------------------------------
// <copyright file="ServerManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Controls the flow of data through the various server layers.
//   Created: August 2006 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using System;
    using WheelMUD.Core;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>The ServerManager controls the flow of data through the various server layers.</summary>
    public class ServerManager : ManagerSystem
    {
        /// <summary>The singleton instance of this class.</summary>
        private static readonly ServerManager SingletonInstance = new ServerManager();

        /// <summary>The base server.</summary>
        private readonly BaseServer baseServer = new BaseServer();

        /// <summary>The telnet server.</summary>
        private readonly TelnetServer telnetServer = new TelnetServer();

        /// <summary>The input parser.</summary>
        private readonly InputParser inputParser = new InputParser();

        /// <summary>Prevents a default instance of the <see cref="ServerManager"/> class from being created.</summary>
        private ServerManager()
        {
            // Set up our event handlers for the base server.
            this.baseServer.ClientConnect += this.BaseServer_OnClientConnect;
            this.baseServer.DataReceived += this.BaseServer_OnDataReceived;
            this.baseServer.DataSent += BaseServer_OnDataSent;
            this.baseServer.ClientDisconnected += this.BaseServer_OnClientDisconnected;

            // Set up our event handlers for the command server.
            this.inputParser.InputReceived += this.CommandServer_OnInputReceived;

            // Set up to respond to player log out events by closing those connections.
            PlayerManager.GlobalPlayerLogOutEvent += this.PlayerManager_GlobalPlayerLogOutEvent;
        }

        /// <summary>Gets the singleton instance of this ServerManager.</summary>
        public static ServerManager Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Gets the start time.</summary>
        public DateTime StartTime { get; private set; }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
            this.SystemHost.UpdateSystemHost(this, "Starting...");

            this.baseServer.SubscribeToSystem(this);
            this.baseServer.Start();

            this.telnetServer.Start();

            this.SystemHost.UpdateSystemHost(this, "Started on port " + this.baseServer.Port);

            this.StartTime = DateTime.Now;
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            this.SystemHost.UpdateSystemHost(this, "Stopping...");

            this.telnetServer.Stop();
            this.baseServer.Stop();

            this.SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Closes the specified connection.</summary>
        /// <param name="connectionId">The connection ID to be closed.</param>
        public void CloseConnection(string connectionId)
        {
            this.baseServer.CloseConnection(connectionId);
        }

        /// <summary>Closes the specified connection.</summary>
        /// <param name="connection">The connection to be closed.</param>
        public void CloseConnection(IConnection connection)
        {
            this.baseServer.CloseConnection(connection);
        }

        /// <summary>Gets the specified connection.</summary>
        /// <param name="connectionId">The connection ID to get.</param>
        /// <returns> The get connection.</returns>
        public IConnection GetConnection(string connectionId)
        {
            return this.baseServer.GetConnection(connectionId);
        }

        /// <summary>This is called when the base server sent data.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        private static void BaseServer_OnDataSent(object sender, ConnectionArgs args)
        {
        }

        /// <summary>Sends the incoming data up the server chain for processing.</summary>
        /// <param name="sender">The connection sending the data</param>
        /// <param name="data">The data being sent</param>
        private void ProcessIncomingData(IConnection sender, byte[] data)
        {
            byte[] bytes = this.telnetServer.OnDataReceived(sender, data);

            // All bytes might have been stripped out so check for that.
            if (bytes.Length > 0)
            {
                this.inputParser.OnDataReceived(sender, bytes);
            }
        }

        /// <summary>This is called when we receive input on a connection.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        /// <param name="input">The input received.</param>
        private void CommandServer_OnInputReceived(object sender, ConnectionArgs args, string input)
        {
            // We send the data received onto our session manager to deal with the input.
            SessionManager.Instance.OnInputReceived(args.Connection, input);
        }

        /// <summary>This is called when a client connects to the base server.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        private void BaseServer_OnClientConnect(object sender, ConnectionArgs args)
        {
            // We send the connection to our session manager to deal with.
            this.UpdateSubSystemHost((ISubSystem)sender, args.Connection.ID + " - Connected");
            SessionManager.Instance.OnSessionConnected(args.Connection);
        }

        /// <summary>This is called when a client disconnects from the base server.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        private void BaseServer_OnClientDisconnected(object sender, ConnectionArgs args)
        {
            SessionManager.Instance.OnSessionDisconnected(args.Connection);

            this.UpdateSubSystemHost((ISubSystem)sender, args.Connection.ID + " - Disconnected");
        }

        /// <summary>This is called when the base server receives data.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        private void BaseServer_OnDataReceived(object sender, ConnectionArgs args)
        {
            this.ProcessIncomingData(args.Connection, args.Connection.Data);
        }

        /// <summary>Processes the player log out events from the player manager; disconnects logged out characters.</summary>
        /// <param name="root">The root location where the log out event originated.</param>
        /// <param name="e">The event arguments.</param>
        private void PlayerManager_GlobalPlayerLogOutEvent(Thing root, GameEvent e)
        {
            // If the player was user-controlled during log out, disconnect that user.
            var userControlledBehavior = e.ActiveThing.Behaviors.FindFirst<UserControlledBehavior>();
            if (userControlledBehavior != null && userControlledBehavior.Controller != null)
            {
                var session = userControlledBehavior.Controller as Session;
                if (session != null && session.Connection != null)
                {
                    session.Connection.Disconnect();
                }
            }
        }

        /// <summary>MEF exporter for ServerManager.</summary>
        [ExportSystem]
        public class ServerManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            /// <returns>A new instance of the singleton system.</returns>
            public override ISystem Instance
            {
                get { return ServerManager.Instance; }
            }

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType
            {
                get { return typeof(ServerManager); }
            }
        }
    }
}