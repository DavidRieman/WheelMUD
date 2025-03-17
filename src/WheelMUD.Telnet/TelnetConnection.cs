// Copyright (c) WheelMUD Development Team.  See LICENSE.txt or https://github.com/DavidRieman/WheelMUD/#license

using System.Net;
using System.Net.Sockets;

namespace WheelMUD.Telnet
{
    public class TelnetConnection
    {
        private readonly object lockObject = new();

        /// <summary>
        /// This buffer is intentionally tiny for now to ensure our users handle valid Telent edge cases like receiving
        /// packet breaks in the middle of a Telnet command, character encoding, or other "inconvenient" moments. These
        /// are valid Telnet situations, so make sure you test such things thoroughly before raising the data buffer size.
        /// This also means our data streams in similarly whether in char-at-a-time mode or line-at-a-time mode since the
        /// line-at-a-time mode will also only add one byte at a time to the building received input buffer.
        /// </summary>
        private readonly int DataBufferSize = 1;

        public delegate void DisconnectedEvent();

        /// <summary>This event is raised when we have received data from this client.</summary>
        /// <param name="totalReceivedBytes">The total received bytes. Any additional bytes in the full buffer (if any) should be ignored.</param>
        /// <param name="dataBuffer">The buffer which received the bytes. Only read up to totalReceivedBytes worth of bytes from this buffer.</param>
        public delegate void DataReceivedEvent(int totalReceivedBytes, byte[] dataBuffer);

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

        private byte[] Data { get; set; }

        public string ID { get; } = Guid.NewGuid().ToString();

        public TelnetConnection(Socket socket, IPAddress ip, int dataBufferSize = 1)
        {
            this.socket = socket;
            DataBufferSize = dataBufferSize;
            Data = NextDataBuffer();
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
                        // Keep a reference to the Data we just received, and use eventing to pass it to the user.
                        // Setting Data to a new buffer means even if that event lags behind the next processing,
                        // it won't matter as each one will be a new local `data` here held in memory only as long
                        // as it is still needed.
                        var data = Data;
                        Data = NextDataBuffer();

                        // Raise the Data Received Event. Signals that some data has arrived.
                        DataReceived?.Invoke(totalReceivedBytes, data);

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
                if (socket.Connected)
                {
                    // Start receiving any data written by the connected client asynchronously.
                    socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, onDataReceived, null);
                }
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
            if (socket.Connected)
            {
                socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(OnSendComplete), null);
            }
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

        // TODO: Performance test and compare trade-offs for various options, such as combinations of:
        // 1) Keeping a global collection of data buffers of the given size, to only "new" one if the collection of data buffers is
        //    empty. Return each finished Data buffer to the collection for reuse after OnDataRecieved event handlers are done.
        // 2) Try different default DataBufferSize other than 1.
        // 3) Keeping as-is.
        private byte[] NextDataBuffer()
        {
            return new byte[DataBufferSize];
        }
    }
}
