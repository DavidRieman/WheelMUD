//-----------------------------------------------------------------------------
// <copyright file="ServerManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using WheelMUD.Server;
using WheelMUD.Utilities.Interfaces;
using WheelTelnet;

namespace WheelMUD.Core
{
    /// <summary>The ServerManager controls the flow of data through the various server layers.</summary>
    public class ServerManager : ManagerSystem, IRecomposable
    {
        /// <summary>The active connection manager(s).</summary>
        private List<IGameConnectionManager> connectionManagers = [];

        /// <summary>The input parser.</summary>
        private readonly InputParser inputParser = new();

        /// <summary>Prevents a default instance of the <see cref="ServerManager"/> class from being created.</summary>
        private ServerManager()
        {
            // Set up our event handlers for the command server.
            inputParser.InputReceived += CommandServer_OnInputReceived;

            // Set up to respond to player log out events by closing those connections.
            PlayerManager.Instance.GlobalPlayerLogOutEvent += PlayerManager_GlobalPlayerLogOutEvent;
        }

        /// <summary>Gets the singleton instance of this ServerManager.</summary>
        public static ServerManager Instance { get; } = new ServerManager();

        /// <summary>Gets or sets imported connection manager exporters.</summary>
        /// <remarks>This list is composed via MEF.</remarks>
        [ImportMany]
        private Lazy<GameConnectionManagerExporter, ServerExports.GameConnectionManager>[] ImportedConnectionManagers { get; set; }

        /// <summary>Gets the start time.</summary>
        public DateTime StartTime { get; private set; }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");

            // Find our fresh set of connection managers via MEF composition, and set up event handlers for them.
            Recompose();

            foreach (var manager in connectionManagers)
            {
                manager.SubscribeToSystem(this);
                manager.Start();
                SystemHost.UpdateSystemHost(this, $"Started {manager.GetType().Name}");
            }

            StartTime = DateTime.Now;
            SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            foreach (var manager in connectionManagers)
            {
                manager.Stop();
            }

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Recompose the connection managers from MEF exporters.</summary>
        public void Recompose()
        {
            DefaultComposer.Container.ComposeParts(this);

            if (ImportedConnectionManagers == null || ImportedConnectionManagers.Length == 0)
            {
                SystemHost.UpdateSystemHost(this, "Warning: No connection managers found via MEF: Players will have no means to connect.");
                return;
            }

            // Get the highest priority exporters of each found type name.
            var exportersToUse = new List<GameConnectionManagerExporter>();
            var distinctTypeNames = ImportedConnectionManagers.Select(m => m.Value.ManagerType.Name).Distinct();
            foreach (var typeName in distinctTypeNames)
            {
                var exporter = (from m in ImportedConnectionManagers
                               where m.Value.ManagerType.Name == typeName
                               orderby m.Metadata.Priority descending
                               select m.Value).FirstOrDefault();
                if (exporter != null)
                {
                    exportersToUse.Add(exporter);
                }
            }

            // Unsubscribe from old connection managers
            foreach (var manager in connectionManagers)
            {
                manager.ClientConnect -= BaseServer_OnClientConnect;
                manager.DataReceived -= BaseServer_OnDataReceived;
                manager.ClientDisconnected -= BaseServer_OnClientDisconnected;
            }

            // Set up new connection managers
            connectionManagers = exportersToUse.Select(exporter => exporter.Instance).ToList();

            foreach (var manager in connectionManagers)
            {
                manager.ClientConnect += BaseServer_OnClientConnect;
                manager.DataReceived += BaseServer_OnDataReceived;
                manager.ClientDisconnected += BaseServer_OnClientDisconnected;
            }

            SystemHost.UpdateSystemHost(this, $"Loaded {connectionManagers.Count} connection manager(s)");
        }

        /// <summary>Closes the specified connection.</summary>
        /// <param name="connectionId">The connection ID to be closed.</param>
        public void CloseConnection(string connectionId)
        {
            foreach (var manager in connectionManagers)
            {
                manager.CloseConnection(connectionId);
            }
        }

        /// <summary>Closes the specified connection.</summary>
        /// <param name="connection">The connection to be closed.</param>
        public static void CloseConnection(TelnetConnection connection)
        {
            connection.Disconnect();
        }

        /// <summary>Sends the incoming data up the server chain for processing.</summary>
        /// <param name="sender">The connection sending the data</param>
        /// <param name="data">The data being sent</param>
        private void ProcessIncomingData(Connection sender, byte[] data)
        {
            // The telnet data has already been processed by the connection manager's telnet code handler.
            // Now we just need to pass it to the input parser to handle command parsing.
            inputParser.OnDataReceived(sender, data);
        }

        /// <summary>This is called when we receive input on a connection.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        /// <param name="input">The input received.</param>
        private void CommandServer_OnInputReceived(object sender, Connection connection, string input)
        {
            // We send the data received onto our session manager to deal with the input.
            SessionManager.Instance.OnInputReceived(connection, input);
        }

        /// <summary>This is called when a client connects to the base server.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        private void BaseServer_OnClientConnect(Connection connection)
        {
            // We send the connection to our session manager to deal with.
            UpdateSubSystemHost(null, connection.ID + " - Connected");
            SessionManager.Instance.OnSessionConnected(connection);
        }

        /// <summary>This is called when a client disconnects from the base server.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        private void BaseServer_OnClientDisconnected(Connection connection)
        {
            SessionManager.Instance.OnSessionDisconnected(connection);
            UpdateSubSystemHost(null, connection.ID + " - Disconnected");
        }

        /// <summary>This is called when the base server receives data.</summary>
        /// <param name="sender">The sender of this event.</param>
        /// <param name="args">The event arguments.</param>
        private void BaseServer_OnDataReceived(Connection connection)
        {
            // @@@ TODO Should Connection be the only place handling data received, and raise an event without exposing Data prop directly?
            ProcessIncomingData(connection, connection.Data);
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
        [CoreExports.System(0)]
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