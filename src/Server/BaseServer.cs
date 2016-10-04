//-----------------------------------------------------------------------------
// <copyright file="BaseServer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   This is the lowest level of our server, this object deals with the creation of our connections
//   and also the sending and receiving of data over the wire. All data flows through this class.
//   Created: August 2006 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using WheelMUD.Core;
using WheelMUD.Interfaces;

namespace WheelMUD.Server
{
    /// <summary>The base, lowest level of our server.</summary>
    public class BaseServer : ISubSystem
    {
        /// <summary>The synchronization lock object.</summary>
        private static readonly object LockObject = new object();

        /// <summary>The list of current connections to this server.</summary>
        private readonly List<IConnection> connections = new List<IConnection>();

        /// <summary>The primary socket for incoming connections.</summary>
        private Socket mainSocket;

        /// <summary>Sub system host.</summary>
        private ISubSystemHost subSystemHost;

        /// <summary>Initializes a new instance of the BaseServer class.</summary>
        public BaseServer()
        {
            Port = 4000;
        }

        /// <summary>Initializes a new instance of the BaseServer class, and specifies which port to use.</summary>
        /// <param name="port">Which port to open up for incoming connections.</param>
        public BaseServer(int port)
        {
            Port = port;
        }

        /// <summary>A 'client connected' event raised by the server.</summary>
        public event EventHandler<ConnectionArgs> ClientConnect;

        /// <summary>A 'client disconnected' event raised by the server.</summary>
        public event EventHandler<ConnectionArgs> ClientDisconnected;

        /// <summary>A 'data received' event raised by the server.</summary>
        public event EventHandler<ConnectionArgs> DataReceived;

        /// <summary>A 'data sent' event raised by the server.</summary>
        public event EventHandler<ConnectionArgs> DataSent;

        /// <summary>An 'input received' event raised by the server.</summary>
        public event InputReceivedEventHandler InputReceived;

        /// <summary>Gets or sets which port this server listens to for incoming connections.</summary>
        public int Port { get; set; }

        /// <summary>Starts up the server.</summary>
        public void Start()
        {
            try
            {
                // Create the listening socket...
                mainSocket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Stream,
                        ProtocolType.Tcp);
                var localIP = new IPEndPoint(IPAddress.Any, Port);
                
                // Bind to local IP Address...
                mainSocket.Bind(localIP);
                
                // Start listening...
                mainSocket.Listen(4);
                
                // Create the call back for any client connections...
                mainSocket.BeginAccept(OnClientConnect, null);
            }
            catch (SocketException se)
            {
                // Convert port-in-use into a more useful/distinct exception.
                if (se.ErrorCode == 10048)
                {
                    string format = "Error number {0}: {1}. Port number {2} is already in use.";
                    string message = string.Format(format, se.ErrorCode, se.Message, Port);
                    throw new PortInUseException(message);
                }

                // Any other unrecognized exception at startup should just be rethrown for debugging.
                throw;
            }
        }

        /// <summary>Stops the server.</summary>
        public void Stop()
        {
            CloseSockets();
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
        public IConnection GetConnection(string connectionId)
        {
            for (int i = connections.Count - 1; i >= 0; i--)
            {
                if (connections[i].ID == connectionId)
                {
                    IConnection connection = connections[i];

                    return connection;
                }
            }

            return null;
        }

        /// <summary>Closes the specified connection.</summary>
        /// <param name="connection">Connection that is to be closed.</param>
        public void CloseConnection(IConnection connection)
        {
            if (connection is Connection)
            {
                var conn = (Connection)connection;
                conn.DataSent -= EventHandlerDataSent;
                conn.DataReceived -= EventHandlerDataReceived;
                conn.ClientDisconnected -= EventHandlerClientDisconnected;
            }

            connection.Disconnect();
            lock (LockObject)
            {
                connections.Remove(connection);
                if (ClientDisconnected != null)
                {
                    ClientDisconnected(this, new ConnectionArgs(connection));
                }
            }
        }

        /// <summary>Closes a connection.</summary>
        /// <param name="connectionId">The ID of the connection to close.</param>
        public void CloseConnection(string connectionId)
        {
            for (int i = connections.Count - 1; i >= 0; i--)
            {
                if (connections[i].ID == connectionId)
                {
                    IConnection connection = connections[i];

                    CloseConnection(connection);
                }
            }
        }

        /// <summary>Send data to the specified connection.</summary>
        /// <param name="connection">The connection to send the data to.</param>
        /// <param name="data">The data to be sent.</param>
        public void SendData(Connection connection, byte[] data)
        {
            connection.Send(data);
        }

        /// <summary>This is the callback when a client connects</summary>
        /// <param name="asyncResult">@@@ What is this?</param>
        private void OnClientConnect(IAsyncResult asyncResult)
        {
            try
            {
                // Here we complete/end the BeginAccept() asynchronous call
                // by calling EndAccept() - which returns the reference to
                // a new Socket object
                Socket socket = mainSocket.EndAccept(asyncResult);
                var conn = new Connection(socket, this);
                conn.DataSent += EventHandlerDataSent;
                conn.DataReceived += EventHandlerDataReceived;
                conn.ClientDisconnected += EventHandlerClientDisconnected;

                // Let the worker Socket do the further processing for the 
                // just connected client
                conn.ListenForData();

                lock (LockObject)
                {
                    connections.Add(conn);
                }

                // Raise our client connect event.
                if (ClientConnect != null)
                {
                    ClientConnect(this, new ConnectionArgs(conn));
                }

                // Since the main Socket is now free, it can go back and wait for
                // other clients who are attempting to connect
                mainSocket.BeginAccept(OnClientConnect, null);
            }
            catch (ObjectDisposedException)
            {
                // This exception was preventing the console from closing when the
                // shutdown command was issued.
            }
        }

        /// <summary>The event handler for the 'client disconnected' event.</summary>
        /// <param name="sender">The connection that originated this event.</param>
        /// <param name="args">The connection arguments for this event.</param>
        private void EventHandlerClientDisconnected(object sender, ConnectionArgs args)
        {
            lock (LockObject)
            {
                connections.Remove(args.Connection);
            }

            if (ClientDisconnected != null)
            {
                ClientDisconnected(this, args);
            }
        }

        /// <summary>The event handler for the 'data received' event.</summary>
        /// <param name="sender">The connection that originated this event.</param>
        /// <param name="args">The connection arguments for this event.</param>
        private void EventHandlerDataReceived(object sender, ConnectionArgs args)
        {
            if (DataReceived != null)
            {
                DataReceived(sender, args);
            }
        }

        /// <summary>The event handler for the 'data sent' event.</summary>
        /// <param name="sender">The connection that originated this event.</param>
        /// <param name="args">The connection arguments for this event.</param>
        private void EventHandlerDataSent(object sender, ConnectionArgs args)
        {
            if (DataSent != null)
            {
                DataSent(sender, args);
            }
        }

        /// <summary>The event handler for the 'input received' event.</summary>
        /// <param name="sender">The connection that originated this event.</param>
        /// <param name="input">The input that was received.</param>
        private void EventHandlerInputReceived(IConnection sender, string input)
        {
            if (InputReceived != null)
            {
                InputReceived(sender, input);
            }
        }

        /// <summary>Close all connected sockets.</summary>
        private void CloseSockets()
        {
            if (mainSocket != null)
            {
                mainSocket.Close();
            }

            IList<IConnection> tempConnections = new List<IConnection>(connections);
            foreach (IConnection conn in tempConnections)
            {
                conn.Send("Server is shutting down; your connection is being closed.");
                conn.Disconnect();
            }
        }
    }
}