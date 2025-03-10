// Copyright (c) WheelMUD Development Team.  See LICENSE.txt or https://github.com/DavidRieman/WheelMUD/#license

using System.Net;
using System.Net.Sockets;

namespace WheelMUD.Telnet
{
    public class TelnetConnection
    {
        private readonly object lockObject = new();

        public delegate void DisconnectedEvent();

        public delegate void DataReceivedEvent(int totalReceivedBytes);

        public delegate void ErrorEvent(Exception exception);

        /// <summary>This event is raised when a disconnection with this client is detected.</summary>
        public event DisconnectedEvent? Disconnected;

        /// <summary>This event is raised when we have received data from this client.</summary>
        public event DataReceivedEvent? DataReceived;

        /// <summary>This event is raised when we hit an asynchronous unrecognized problem with the connection (such as while processing incoming data).</summary>
        /// <remarks>Generally the connection will also be closed immediately after raising the event.</remarks>
        public event ErrorEvent? ErrorEncountered;

        private readonly Socket socket;

        /// <summary>A cached AsyncCallback version of the OnDataReceived function for efficient repeated reuse.</summary>
        private readonly AsyncCallback onDataReceived;

        public IPAddress CurrentIPAddress { get; private set; }

        /// <summary>Gets the buffer of this connection.</summary>
        /// <remarks>
        /// This buffer is intentionally tiny for now so we don't have to keep making new buffers or cleansing old ones.
        /// Basically this means our data streams in similarly for char-at-a-time mode and line-at-a-time mode since the
        /// line-at-a-time mode will also only add one byte at a time to the building received input buffer.
        /// The architecture handles byte[] though, as we should be able to try handling more data at a time in the future.
        /// TODO: Try optimizing for clients which send many bytes at a time (with fresh small buffers of various sizes)
        ///       and perform performance analysis of the various configurations to find a good default configuration.
        /// </remarks>
        public byte[] Data { get; private set; } = new byte[1];

        public string ID { get; } = Guid.NewGuid().ToString();

        public TelnetConnection(Socket socket, IPAddress ip)
        {
            this.socket = socket;
            onDataReceived = new AsyncCallback(OnDataReceived);
            CurrentIPAddress = ip;
        }

        /// <summary>The callback function invoked when the socket detects any client data was received.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        private void OnDataReceived(IAsyncResult asyncResult)
        {
            try
            {
                int totalReceivedBytes;

                if (socket.Connected)
                {
                    // Complete the BeginReceive() asynchronous call by EndReceive() method
                    // which will return the number of characters written to the stream 
                    // by the client
                    totalReceivedBytes = socket.EndReceive(asyncResult);

                    // If the number of bytes received is 0 then something fishy is going on.
                    if (totalReceivedBytes == 0)
                    {
                        Disconnect();
                    }
                    else
                    {
                        // Raise the Data Received Event. Signals that some data has arrived.
                        DataReceived?.Invoke(totalReceivedBytes);

                        // Continue the waiting for data on the Socket, but as a deferred task to avoid stack
                        // overflows from the client sending a ton of data in one pass. (This will also help
                        // ensure each connected client gets their turn if many are sending lots of data.)
                        Task.Run(ListenForData);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // If we're shutting down, quietly ignore these exceptions and try to close the connection.
                Disconnect();
            }
            catch (ThreadAbortException)
            {
                // If we're shutting down, quietly ignore these exceptions and try to close the connection.
                Disconnect();
            }
            catch (Exception ex)
            {
                // Unrecognized problems can be escalated to our user, if they have subscribed to such events.
                // This may provide an opportunity for a dev to hit a breakpoint, before we follow up with a disconnect.
                ErrorEncountered?.Invoke(ex);
                Disconnect();
            }
        }

        internal void ListenForData()
        {
            try
            {
                // Start receiving any data written by the connected client asynchronously.
                socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, onDataReceived, null);
            }
            catch (ObjectDisposedException)
            {
                Disconnect();
            }
            catch (SocketException)
            {
                Disconnect();
            }
        }

        /// <summary>Asynchronous callback when a send completes successfully.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        private void OnSendComplete(IAsyncResult asyncResult)
        {
            try
            {
                socket.EndSend(asyncResult);
            }
            catch
            {
                Disconnect();
            }
        }

        public void Send(byte[] data)
        {
            socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(OnSendComplete), null);
        }

        /// <summary>Disconnects the socket (if still connected) and raises the disconnected event.</summary>
        /// <remarks>Internal: Use TelnetServer's CloseConnection call instead (to ensure it can clean up too).</remarks>
        public void Disconnect()
        {
            // Prevent simultaneous disconnects from multiple threads. (First one gets to cleanup, others will skip dangerous redundant work.)
            lock (lockObject)
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    Disconnected?.Invoke();
                    Disconnected = null;
                    DataReceived = null;
                }
            }
        }
    }
}
