//-----------------------------------------------------------------------------
// <copyright company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Net.Sockets;
using System.Net;

namespace WheelMUD.Telnet
{
    public class TelnetServer(int port, EventHandler<TelnetConnection> clientConnected, EventHandler<TelnetConnection> clientDisconnected)
    {
        /// <summary>The synchronization lock object.</summary>
        private static readonly object LockObject = new();

        /// <summary>An event raised when any new Telnet client has freshly connected to us.</summary>
        private event EventHandler<TelnetConnection> ClientConnected = clientConnected;

        /// <summary>An event raised when any managed Telnet connection has now become disconnected.</summary>
        private event EventHandler<TelnetConnection> ClientDisconnected = clientDisconnected;

        private int Port { get; } = port;

        private readonly IPEndPoint endPoint = new(IPAddress.Any, port);

        private readonly Socket mainSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private readonly List<TelnetConnection> connections = [];

        public void Start()
        {
            try
            {
                mainSocket.Bind(endPoint);
                mainSocket.Listen(4);
                mainSocket.BeginAccept(OnClientConnect, null);
            }
            catch (SocketException se)
            {
                // Convert port-in-use into a more useful/distinct exception.
                if (se.ErrorCode == 10048)
                {
                    throw new PortInUseException($"Port number {Port} is already in use. (Error {se.ErrorCode}: {se.Message}.)");
                }

                // Any other unrecognized exception at startup should just be rethrown as-is for debugging.
                throw;
            }
        }

        /// <summary>Closes the specified connection.</summary>
        /// <param name="connection">Connection that is to be closed.</param>
        /*public void CloseConnection(TelnetConnection connection)
        {
            connection.Disconnect();
            connection.DataReceived -= EventHandlerDataReceived;
            connection.ClientDisconnected -= EventHandlerClientDisconnected;

            connection.Disconnect();
            lock (LockObject)
            {
                connections.Remove(connection);
                ClientDisconnected?.Invoke(this, connection);
            }
        }*/

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

        private void OnClientConnect(IAsyncResult asyncResult)
        {
            try
            {
                // Here we complete/end the BeginAccept() asynchronous call by calling EndAccept(),
                // which returns the reference to a new Socket object specifically for the new client.
                Socket socket = mainSocket.EndAccept(asyncResult);
                var connection = new TelnetConnection(socket);
                EventHandler<TelnetConnection> onDisconnect = (sender, e) =>
                {
                    lock (LockObject)
                    {
                        connections.Remove(connection);
                    }
                };
                connection.Disconnected += onDisconnect;

                // Let the worker Socket do the further processing for the freshly connected client.
                connection.ListenForData();

                lock (LockObject)
                {
                    connections.Add(connection);
                }

                // Raise our client connect event.
                ClientConnected?.Invoke(this, connection);

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
    }
}
