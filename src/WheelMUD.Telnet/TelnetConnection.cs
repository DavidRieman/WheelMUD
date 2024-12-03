//-----------------------------------------------------------------------------
// <copyright company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace WheelMUD.Telnet
{
    public class TelnetConnection
    {
        /// <summary>This event is raised when a disconnection with this client is detected.</summary>
        public event EventHandler<TelnetConnection>? Disconnected;

        /// <summary>This event is raised when we have received data from this client.</summary>
        public event EventHandler<TelnetConnection>? DataReceived;

        private readonly Socket socket;

        /// <summary>A cached AsyncCallback version of the OnDataReceived function for efficient repeated reuse.</summary>
        private readonly AsyncCallback onDataReceived;

        public IPAddress? CurrentIPAddress { get; private set; }

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

        public TelnetConnection(Socket socket)
        {
            this.socket = socket;
            onDataReceived = new AsyncCallback(OnDataReceived);
            CurrentIPAddress = (socket.RemoteEndPoint as IPEndPoint)?.Address;
        }

        /// <summary>The callback function invoked when the socket detects any client data was received.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        private void OnDataReceived(IAsyncResult asyncResult)
        {
            try
            {
                int iRx;

                if (socket.Connected)
                {
                    // Complete the BeginReceive() asynchronous call by EndReceive() method
                    // which will return the number of characters written to the stream 
                    // by the client
                    iRx = socket.EndReceive(asyncResult);

                    // If the number of bytes received is 0 then something fishy is going on.
                    if (iRx == 0)
                    {
                        Disconnect();
                    }
                    else
                    {
                        // Raise the Data Received Event. Signals that some data has arrived.
                        DataReceived?.Invoke(this, this);

                        // Continue the waiting for data on the Socket
                        ListenForData();
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
            catch (Exception)
            {
                // @@@ MOVE THIS ALL UP to WRAP the WheelMUD DATA HANDLER INSTEAD
                //string ip = CurrentIPAddress == null ? "[null]" : CurrentIPAddress.ToString();
                //string message = $"Exception encountered for connection:{Environment.NewLine}IP: {ip}, ID {ID}:{Environment.NewLine}{ex.ToDeepString()}";
                //connectionHost.InformSubscribedSystem(message);
                // If the debugger is attached, we probably want to break now in order to better debug 
                // the issue closer to where it occurred; if your debugger broke here you may want to 
                // look at the stack trace to see where the exception originated.
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                Disconnect();
            }
        }

        public void ListenForData()
        {
            try
            {
                // Start receiving any data written by the connected client asynchronously.
                var callback = new AsyncCallback(onDataReceived);
                socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, callback, null);
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

        /// <summary>Disconnects the socket (if still connected) and raises the disconnected event.</summary>
        /// <remarks>Internal: Use TelnetServer's CloseConnection call instead (to ensure it can clean up too).</remarks>
        internal void Disconnect()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Disconnected?.Invoke(this, this);
            }
        }
    }
}
