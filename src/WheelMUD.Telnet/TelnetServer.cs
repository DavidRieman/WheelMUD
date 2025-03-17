// Copyright (c) WheelMUD Development Team.  See LICENSE.txt or https://github.com/DavidRieman/WheelMUD/#license

using System.Net.Sockets;
using System.Net;
using System.Collections.ObjectModel;

namespace WheelMUD.Telnet
{
    public class TelnetServer(int port)
    {
        public delegate bool ClientBeginConnectionEvent(IPAddress ip);

        public delegate void ClientConnectEvent(TelnetConnection telnetConnection);

        public delegate void ClientDisconnectEvent(TelnetConnection telnetConnection);

        public event ClientBeginConnectionEvent? ClientBeginConnection;

        public event ClientConnectEvent? ClientConnected;

        //public event ClientDisconnectEvent? ClientDisconnected;

        /// <summary>The synchronization lock object.</summary>
        private static readonly object LockObject = new();

        /// <summary>An event raised when any new Telnet client has freshly connected to us.</summary>
        //private event EventHandler<TelnetConnection> ClientConnected = clientConnected;

        /// <summary>An event raised when any managed Telnet connection has now become disconnected.</summary>
        //private event EventHandler<TelnetConnection> ClientDisconnected = clientDisconnected;

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

        public void Stop()
        {
            lock (LockObject)
            {
                foreach (var telnetConnection in connections)
                {
                    telnetConnection.Disconnect();
                }
                connections.Clear();
            }
        }

        public ReadOnlyCollection<TelnetConnection> AllActiveClients
        {
            get
            {
                lock (LockObject)
                {
                    return connections.AsReadOnly();
                }
            }
        }

        /// <summary>The event handler for the 'client disconnected' event.</summary>
        /// <param name="args">The connection arguments for this event.</param>
        private void OnClientDisconnected(TelnetConnection connection)
        {
            lock (LockObject)
            {
                connections.Remove(connection);
            }
        }

        private void OnClientConnect(IAsyncResult asyncResult)
        {
            try
            {
                // Here we complete/end the BeginAccept() asynchronous call by calling EndAccept(),
                // which returns the reference to a new Socket object specifically for the new client.
                Socket socket = mainSocket.EndAccept(asyncResult);
                IPAddress? ip = (socket.RemoteEndPoint as IPEndPoint)?.Address;

                // If we somehow have a weird connection without an address for this client, we won't proceed with it.
                if (ip == null)
                {
                    socket.Close();
                    return;
                }

                // Give a very early opportunity for even subscribers to decide if this connection may be undesirable, such
                // as being a banned IP in a block-list. If so, close the connection before letting any real data through.
                if (ClientBeginConnection != null)
                {
                    var handlers = ClientBeginConnection.GetInvocationList().Cast<ClientBeginConnectionEvent>();
                    foreach (ClientBeginConnectionEvent handler in handlers)
                    {
                        if (!handler(ip))
                        {
                            socket.Close();
                            return;
                        }
                    }
                }

                // If we passed all early checks, then we can proceed with preparing the connection for real communications,
                // including ensuring even early disconnections on either end will be able to emit a disconnection event.
                var telnetConnection = new TelnetConnection(socket, ip);
                telnetConnection.Disconnected += () => { OnClientDisconnected(telnetConnection); };
                lock (LockObject)
                {
                    connections.Add(telnetConnection);
                }

                // Before we start listening, we need to prepare the Telnet code handlers for this TelnetConnection.
                // Since the collection of handlers and desired reactions to them depend on our user's preferences,
                // we can raise an event with this TelnetConnection to give them a chance to set up the handlers.
                ClientConnected?.Invoke(telnetConnection);

                // Let the worker Socket do the further processing for the freshly connected client.
                telnetConnection.ListenForData();
            }
            catch (ObjectDisposedException)
            {
                // Ignore: May sometimes happen when program is trying to shut down.
            }

            try
            {
                // Since the main Socket is now free, it can go back and wait for more incoming clients.
                mainSocket.BeginAccept(OnClientConnect, null);
            }
            catch (ObjectDisposedException)
            {
                // Ignore: May sometimes happen when program is trying to shut down.
            }
        }
    }
}
