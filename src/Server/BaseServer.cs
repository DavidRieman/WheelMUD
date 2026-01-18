//-----------------------------------------------------------------------------
// <copyright file="BaseServer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Telnet;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Server
{
    /// <summary>The base, lowest level of our server.</summary>
    /// <remarks>
    /// This object deals with the creation of our connections and also the sending and receiving
    /// of data over the wire. All data going to/from client connections flow through this class.
    /// </remarks>
    public class BaseServer : ISubSystem
    {
        /// <summary>The synchronization lock object.</summary>
        private static readonly object LockObject = new();

        /// <summary>The list of current connections to this server.</summary>
        private readonly List<TelnetConnection> connections = new();

        /// <summary>Sub system host.</summary>
        private ISubSystemHost subSystemHost;

        /// <summary>Initializes a new instance of the BaseServer class.</summary>
        public BaseServer()
        {
            Port = 4000; // TODO: Read from app.config TelnetPort instead?

            // Prepare the telnet server, but do not start it until requested to.
            telnetServer = new WheelMUD.Telnet.TelnetServer(Port);

            // For now we allow all connections, but we could implement block-list support here like this:
            // telnetServer.ClientBeginConnection += (IPAddress ip) => call a block-list manager to return true/false for us;
            // We could also disregard new-connection spam from single IP addresses to prevent DoS attacks from a single source,
            // or mechanics like that for at least keeping us from getting bogged down during DDoS attempts, and so on.

            // @@@ Do we configure default option code handlers here, or just raise and let another system do that?
            telnetServer.ClientConnected += (TelnetConnection telnetConnection) => ClientConnect?.Invoke(telnetConnection);
            telnetServer.ClientDisconnected += (TelnetConnection telnetConnection) => ClientDisconnected?.Invoke(telnetConnection);
        }

        /// <summary>A 'client connected' event raised by the server.</summary>
        public event Action<TelnetConnection> ClientConnect;

        /// <summary>A 'client disconnected' event raised by the server.</summary>
        public event Action<TelnetConnection> ClientDisconnected;

        /// <summary>A 'data received' event raised by the server.</summary>
        public event Action<TelnetConnection> DataReceived;

        /// <summary>Gets or sets which port this server listens to for incoming connections.</summary>
        public int Port { get; set; }

        private WheelMUD.Telnet.TelnetServer telnetServer;

        /// <summary>Starts up the server.</summary>
        public void Start()
        {
            telnetServer.Start();
        }

        /// <summary>Stops the server.</summary>
        public void Stop()
        {
            telnetServer.Stop();
        }

        /// <summary>Subscribe to receive system updates from this system.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        public void SubscribeToSystem(ISubSystemHost sender)
        {
            subSystemHost = sender;
        }

        /// <summary>Inform subscribed system(s) of the specified update.</summary>
        /// <param name="message">The message to be sent to subscribed system(s).</param>
        public void InformSubscribedSystem(string message)
        {
            subSystemHost.UpdateSubSystemHost(this, message);
        }

        /// <summary>Gets a connection specified by the connectionId.</summary>
        /// <param name="connectionId">The ID of the connection to get</param>
        /// <returns>The specified connection</returns>
        public TelnetConnection GetConnection(string connectionId)
        {
            lock (LockObject)
            {
                return connections.Where(c => c.ID == connectionId).FirstOrDefault();
            }
        }

        /// <summary>Closes the specified connection.</summary>
        /// <param name="connection">Connection that is to be closed.</param>
        public void CloseConnection(TelnetConnection connection)
        {
            connection.Disconnect();
            lock (LockObject)
            {
                connections.Remove(connection);
                ClientDisconnected?.Invoke(this, connection);
            }
        }

        /// <summary>Closes a connection.</summary>
        /// <param name="connectionId">The ID of the connection to close.</param>
        public void CloseConnection(string connectionId)
        {
            var connection = GetConnection(connectionId);
            if (connection != null)
            {
                CloseConnection(connection);
            }
        }

        /// <summary>Send data to the specified connection.</summary>
        /// <param name="connection">The connection to send the data to.</param>
        /// <param name="data">The data to be sent.</param>
        public void SendData(Connection connection, byte[] data)
        {
            connection.Send(data);
        }

        /// <summary>The event handler for the 'client disconnected' event.</summary>
        /// <param name="sender">The connection that originated this event.</param>
        /// <param name="args">The connection arguments for this event.</param>
        private void EventHandlerClientDisconnected(object sender, TelnetConnection connection)
        {
            lock (LockObject)
            {
                connections.Remove(connection);
            }

            ClientDisconnected?.Invoke(this, connection);
        }

        /// <summary>The event handler for the 'data received' event.</summary>
        /// <param name="sender">The connection that originated this event.</param>
        /// <param name="args">The connection arguments for this event.</param>
        private void EventHandlerDataReceived(object sender, TelnetConnection connection)
        {
            DataReceived?.Invoke(sender, connection);
        }

        /// <summary>Close all connected sockets.</summary>
        private void CloseTelnetConnections()
        {
            var tempConnections = new List<TelnetConnection>(connections);
            foreach (TelnetConnection telnetConnection in tempConnections)
            {
                telnetConnection.Send("Server is shutting down; your connection is being closed.");
                telnetConnection.Disconnect();
            }
        }
    }
}