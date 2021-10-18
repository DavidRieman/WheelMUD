//-----------------------------------------------------------------------------
// <copyright file="ServerManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;
using WheelMUD.Server.Interfaces;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    using System;

    /// <summary>The ServerManager controls the flow of data through the various server layers.</summary>
    public class ServerManager : ManagerSystem
    {
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
            baseServer.ClientConnect += BaseServer_OnClientConnect;
            baseServer.DataReceived += BaseServer_OnDataReceived;
            baseServer.DataSent += BaseServer_OnDataSent;
            baseServer.ClientDisconnected += BaseServer_OnClientDisconnected;

            // Set up our event handlers for the command server.
            inputParser.InputReceived += CommandServer_OnInputReceived;

            // Set up to respond to player log out events by closing those connections.
            PlayerManager.Instance.GlobalPlayerLogOutEvent += PlayerManager_GlobalPlayerLogOutEvent;
        }

        /// <summary>Gets the singleton instance of this ServerManager.</summary>
        public static ServerManager Instance { get; } = new ServerManager();

        /// <summary>Gets the start time.</summary>
        public DateTime StartTime { get; private set; }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");
            baseServer.SubscribeToSystem(this);
            baseServer.Start();
            telnetServer.Start();
            SystemHost.UpdateSystemHost(this, "Started on port " + baseServer.Port);
            StartTime = DateTime.Now;
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            telnetServer.Stop();
            baseServer.Stop();

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Closes the specified connection.</summary>
        /// <param name="connectionId">The connection ID to be closed.</param>
        public void CloseConnection(string connectionId)
        {
            baseServer.CloseConnection(connectionId);
        }

        /// <summary>Closes the specified connection.</summary>
        /// <param name="connection">The connection to be closed.</param>
        public void CloseConnection(IConnection connection)
        {
            baseServer.CloseConnection(connection);
        }

        /// <summary>Gets the specified connection.</summary>
        /// <param name="connectionId">The connection ID to get.</param>
        /// <returns> The get connection.</returns>
        public IConnection GetConnection(string connectionId)
        {
            return baseServer.GetConnection(connectionId);
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
            byte[] bytes = telnetServer.OnDataReceived(sender, data);

            // All bytes might have been stripped out so check for that.
            if (bytes.Length > 0)
            {
                inputParser.OnDataReceived(sender, bytes);
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
            UpdateSubSystemHost((ISubSystem)sender, args.Connection.ID + " - Connected");
            SessionManager.Instance.OnSessionConnected(args.Connection);
        }

        /// <summary>This is called when a client disconnects from the base server.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        private void BaseServer_OnClientDisconnected(object sender, ConnectionArgs args)
        {
            SessionManager.Instance.OnSessionDisconnected(args.Connection);

            UpdateSubSystemHost((ISubSystem)sender, args.Connection.ID + " - Disconnected");
        }

        /// <summary>This is called when the base server receives data.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        private void BaseServer_OnDataReceived(object sender, ConnectionArgs args)
        {
            ProcessIncomingData(args.Connection, args.Connection.Data);
        }

        /// <summary>Processes the player log out events from the player manager; disconnects logged out characters.</summary>
        /// <param name="root">The root location where the log out event originated.</param>
        /// <param name="e">The event arguments.</param>
        private void PlayerManager_GlobalPlayerLogOutEvent(Thing root, GameEvent e)
        {
            // If the player was user-controlled during log out, disconnect that user.
            var userControlledBehavior = e.ActiveThing.FindBehavior<UserControlledBehavior>();
            userControlledBehavior?.Session?.Connection?.Disconnect();
        }

        /// <summary>MEF exporter for ServerManager.</summary>
        [ExportSystem(0)]
        public class ServerManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            /// <returns>A new instance of the singleton system.</returns>
            public override ISystem Instance => ServerManager.Instance;

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType => typeof(ServerManager);
        }
    }
}