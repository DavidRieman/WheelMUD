//-----------------------------------------------------------------------------
// <copyright file="Connection.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WheelMUD.Core;
using WheelMUD.Interfaces;
using WheelMUD.Server.Interfaces;
using WheelMUD.Server.Telnet;
using WheelMUD.Utilities;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Server
{
    /// <summary>Represents a connection to a client.</summary>
    /// <remarks>This is the low level connection object that is assigned to a user when they connect.</remarks>
    public class Connection : IConnection
    {
        // After porting to .NET Core, it seems we are no longer able to use the 8-bit ASCII
        // encoder "Encoding.GetEncoding(437)", so we went back to this 7-bit ASCII encoder.
        public static readonly Encoding CurrentEncoding = Encoding.ASCII;

        /// <summary>The threshold, in characters, beyond which MCCP should be used.</summary>
        private const int MCCPThreshold = 100;

        /// <summary>The socket upon which this connection is based.</summary>
        private readonly Socket socket;

        /// <summary>The hosting system of this connection.</summary>
        private readonly ISubSystem connectionHost;

        /// <summary>How many rows of text the client can handle as a single display page.</summary>
        private int pagingRowLimit;

        /// <summary>Initializes a new instance of the <see cref="Connection"/> class.</summary>
        /// <param name="socket">The socket upon which this connection is to be based.</param>
        /// <param name="connectionHost">The system hosting this connection.</param>
        public Connection(Socket socket, ISubSystem connectionHost)
        {
            Buffer = new StringBuilder();
            OutputBuffer = new OutputBuffer();
            Data = new byte[1];
            TerminalOptions = new TerminalOptions();
            this.socket = socket;
            var remoteEndPoint = (IPEndPoint)this.socket.RemoteEndPoint;
            CurrentIPAddress = remoteEndPoint.Address;
            ID = Guid.NewGuid().ToString();
            TelnetCodeHandler = new TelnetCodeHandler(this);

            // TODO: Paging row size should be dynamic from Telnet (NAWS?) or a player-chosen override.
            //       (This used to be called BufferLength in old discussions.)
            PagingRowLimit = 40;
            this.connectionHost = connectionHost;
        }

        /// <summary>The 'client disconnected' event handler.</summary>
        public event EventHandler<ConnectionArgs> ClientDisconnected;

        /// <summary>The 'data received' event handler.</summary>
        public event EventHandler<ConnectionArgs> DataReceived;

        /// <summary>The 'data sent' event handler.</summary>
        public event EventHandler<ConnectionArgs> DataSent;

        /// <summary>Gets the Terminal Options of this connection.</summary>
        public TerminalOptions TerminalOptions { get; private set; }

        /// <summary>Gets the ID of this connection.</summary>
        public string ID { get; private set; }

        /// <summary>Gets the IP Address for this connection.</summary>
        public IPAddress CurrentIPAddress { get; private set; }

        /// <summary>Gets the buffer of this connection.</summary>
        public byte[] Data { get; private set; }

        /// <summary>Gets or sets the buffer of data not yet passed as an action.</summary>
        public StringBuilder Buffer { get; set; }

        /// <summary>Gets or sets the number of buffered rows which the connection's client can handle as one page.</summary>
        public int PagingRowLimit
        {
            get
            {
                return pagingRowLimit;
            }

            set
            {
                pagingRowLimit = value == 0 ? 1000 : value;
            }
        }

        /// <summary>Gets or sets the last raw input the server received.</summary>
        public string LastRawInput { get; set; }

        /// <summary>Gets the telnet option code handler for this connection.</summary>
        public ITelnetCodeHandler TelnetCodeHandler { get; private set; }

        /// <summary>Gets or sets the last string used to end input from the client.</summary>
        public string LastInputTerminator { get; set; }

        /// <summary>Gets or sets the buffer still waiting to be sent to the connection.</summary>
        public OutputBuffer OutputBuffer { get; set; }

        /// <summary>Disconnects the connection.</summary>
        public void Disconnect()
        {
            OnConnectionDisconnect();
        }

        /// <summary>Sends raw bytes to the connection.</summary>
        /// <param name="data">The data to send to the connection.</param>
        public void Send(byte[] data)
        {
            try
            {
                socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(OnSendComplete), null);
            }
            catch (SocketException)
            {
                OnConnectionDisconnect();
            }
            catch (ObjectDisposedException)
            {
                OnConnectionDisconnect();
            }
        }

        /// <summary>Sends string data to the connection.</summary>
        /// <remarks>The data passes through the handlers to be formatted for display.</remarks>
        /// <param name="data">The data to send</param>
        public void Send(string data)
        {
            Send(data, false);
        }

        /// <summary>Sends string data to the connection.</summary>
        /// <param name="data">The data to send.</param>
        /// <param name="bypassDataFormatter">Indicates whether the data formatter should be bypassed (for a quicker send).</param>
        public void Send(string data, bool bypassDataFormatter)
        {
            Send(data, bypassDataFormatter, false);
        }

        /// <summary>Sends string data to the connection</summary>
        /// <param name="data">data to send.</param>
        /// <param name="bypassDataFormatter">Indicates whether the data formatter should be bypassed (for a quicker send).</param>
        /// <param name="sendAllData">Indicates if paging should be allowed</param>
        public void Send(string data, bool bypassDataFormatter, bool sendAllData)
        {
            if (!bypassDataFormatter)
            {
                data = DataFormatter.FormatData(data, this, sendAllData);
            }

            byte[] bytes;

            // Check for MCCP (its not worth using for short strings as the overhead is quite high).
            if (TerminalOptions.UseMCCP && data.Length > MCCPThreshold)
            {
                // Compress the data
                bytes = MCCPHandler.Compress(data);

                // Send the sub request to say that the next load of data
                // is compressed. The sub request is IAC SE COMPRESS2 IAC SB
                Send(new byte[] { 255, 250, 86, 255, 240 });
            }
            else
            {
                bytes = CurrentEncoding.GetBytes(data);
            }

            // Send the data.
            Send(bytes);
        }

        /// <summary>Sends data from the output buffer to the client.</summary>
        /// <param name="bufferDirection">Direction to move in the buffer.</param>
        public void ProcessBuffer(BufferDirection bufferDirection)
        {
            if (OutputBuffer.HasMoreData)
            {
                string[] output = OutputBuffer.GetRows(bufferDirection, PagingRowLimit);

                bool appendOverflow = OutputBuffer.HasMoreData;
                string data = BufferHandler.Format(
                    output,
                    false,
                    appendOverflow,
                    OutputBuffer.CurrentLocation,
                    OutputBuffer.Length);

                Send(data, false, true);
            }
        }

        /// <summary>Asynchronously listens for any incoming data.</summary>
        public void ListenForData()
        {
            try
            {
                // Start receiving any data written by the connected client asynchronously.
                var callback = new AsyncCallback(OnDataReceived);
                socket.BeginReceive(Data, 0, Data.Length, SocketFlags.None, callback, null);
            }
            catch (ObjectDisposedException)
            {
                OnConnectionDisconnect();
            }
            catch (SocketException)
            {
                OnConnectionDisconnect();
            }
        }

        /// <summary>Asynchronous callback when a send completes successfully.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        private void OnSendComplete(IAsyncResult asyncResult)
        {
            try
            {
                socket.EndSend(asyncResult);

                // Raise our data sent event.
                DataSent?.Invoke(this, new ConnectionArgs(this));
            }
            catch
            {
                OnConnectionDisconnect();
            }
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

                    // If the number of bytes received is 0 then something fishy is going on, so
                    // we close the socket.
                    if (iRx == 0)
                    {
                        OnConnectionDisconnect();
                    }
                    else
                    {
                        // Raise the Data Received Event. Signals that some data has arrived.
                        DataReceived?.Invoke(this, new ConnectionArgs(this));

                        // Continue the waiting for data on the Socket
                        ListenForData();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // If we're shutting down, quietly ignore these exceptions and try to close the connection.
                OnConnectionDisconnect();
            }
            catch (ThreadAbortException)
            {
                // If we're shutting down, quietly ignore these exceptions and try to close the connection.
                OnConnectionDisconnect();
            }
            catch (Exception ex)
            {
                // In order to isolate connection-specific issues, we're going to trap the exception, log
                // the details, and kill that connection.  (Other connections and the game itself should
                // be able to continue through such situations.)
                string ip = CurrentIPAddress == null ? "[null]" : CurrentIPAddress.ToString();
                string message = $"Exception encountered for connection:{Environment.NewLine}IP: {ip}, ID {ID}:{Environment.NewLine}{ex.ToDeepString()}";
                connectionHost.InformSubscribedSystem(message);

                // If the debugger is attached, we probably want to break now in order to better debug 
                // the issue closer to where it occurred; if your debugger broke here you may want to 
                // look at the stack trace to see where the exception originated.
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                OnConnectionDisconnect();
            }
        }

        /// <summary>Disconnects the sockets and raises the disconnected event.</summary>
        private void OnConnectionDisconnect()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                ClientDisconnected?.Invoke(this, new ConnectionArgs(this));
            }
        }
    }
}