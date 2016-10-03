//-----------------------------------------------------------------------------
// <copyright file="Connection.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   This is the low level connection object that is assigned to a user when they connect.
//   Created: August 2006 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using WheelMUD.Core;
    using WheelMUD.Core.Enums;
    using WheelMUD.Core.Output;
    using WheelMUD.Interfaces;
    using WheelMUD.Server.Telnet;

    /// <summary>Represents a connection to a client.</summary>
    public class Connection : IConnection
    {
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
            this.Buffer = new StringBuilder();
            this.OutputBuffer = new OutputBuffer();
            this.Data = new byte[1];
            this.Terminal = new Terminal();
            this.socket = socket;
            var remoteEndPoint = (IPEndPoint)this.socket.RemoteEndPoint;
            this.CurrentIPAddress = remoteEndPoint.Address;
            this.ID = Guid.NewGuid().ToString();
            this.TelnetCodeHandler = new TelnetCodeHandler(this);

            // @@@ TODO: Paging row size should be dynamic; this WAS called BufferLength in
            //     discussion: http://www.wheelmud.net/Forums/tabid/59/aft/1600/Default.aspx
            this.PagingRowLimit = 40;
            this.connectionHost = connectionHost;
        }

        /// <summary>The 'client disconnected' event handler.</summary>
        public event EventHandler<ConnectionArgs> ClientDisconnected;

        /// <summary>The 'data received' event handler.</summary>
        public event EventHandler<ConnectionArgs> DataReceived;

        /// <summary>The 'data sent' event handler.</summary>
        public event EventHandler<ConnectionArgs> DataSent;

        /// <summary>Gets the Terminal Options of this connection.</summary>
        public ITerminal Terminal { get; private set; }

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
                return this.pagingRowLimit;
            }

            set
            {
                this.pagingRowLimit = value == 0 ? 1000 : value;
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
            this.OnConnectionDisconnect();
        }

        /// <summary>Sends raw bytes to the connection.</summary>
        /// <param name="data">The data to send to the connection.</param>
        public void Send(byte[] data)
        {
            try
            {
                this.socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(this.OnSendComplete), null);
            }
            catch (SocketException)
            {
                this.OnConnectionDisconnect();
            }
            catch (ObjectDisposedException)
            {
                this.OnConnectionDisconnect();
            }
        }

        /// <summary>Sends string data to the connection.</summary>
        /// <remarks>The data passes through the handlers to be formatted for display.</remarks>
        /// <param name="data">The data to send</param>
        public void Send(string data)
        {
            this.Send(data, false);
        }

        /// <summary>Sends string data to the connection.</summary>
        /// <param name="data">The data to send.</param>
        /// <param name="bypassDataFormatter">Indicates whether the data formatter should be bypassed (for a quicker send).</param>
        public void Send(string data, bool bypassDataFormatter)
        {
            this.Send(data, bypassDataFormatter, false);
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
            if (this.Terminal.UseMCCP && data.Length > MCCPThreshold)
            {
                // Compress the data
                bytes = MCCPHandler.Compress(data);

                // Send the sub request to say that the next load of data
                // is compressed. The sub request is IAC SE COMPRESS2 IAC SB
                this.Send(new byte[] { 255, 250, 86, 255, 240 });
            }
            else
            {
                // Line below commented out by JFed 11/28/2011.  We lose the 8th bit with this encoding, which breaks special characters like ASCII art
                // bytes = ASCIIEncoding.ASCII.GetBytes(data);

                // Encoding using code page 437 (old 8bit default ascii).
                bytes = Encoding.GetEncoding(437).GetBytes(data);
            }

            // Send the data.
            this.Send(bytes);
        }

        /// <summary>Sends data from the output buffer to the client.</summary>
        /// <param name="bufferDirection">Direction to move in the buffer.</param>
        public void ProcessBuffer(BufferDirection bufferDirection)
        {
            if (this.OutputBuffer.HasMoreData)
            {
                string[] output = this.OutputBuffer.GetRows(bufferDirection, this.PagingRowLimit);

                bool appendOverflow = this.OutputBuffer.HasMoreData;
                string data = BufferHandler.Format(
                    output,
                    false,
                    appendOverflow,
                    this.OutputBuffer.CurrentLocation,
                    this.OutputBuffer.Length);

                this.Send(data, false, true);
            }
        }

        /// <summary>Asynchronously listens for any incoming data.</summary>
        public void ListenForData()
        {
            try
            {
                // Start receiving any data written by the connected client
                // asynchronously
                this.socket.BeginReceive(
                        this.Data,
                        0,
                        this.Data.Length,
                        SocketFlags.None,
                        new AsyncCallback(this.OnDataReceived),
                        null);
            }
            catch (SocketException)
            {
                this.OnConnectionDisconnect();
            }
        }

        /// <summary>Asynchronous callback when a send completes successfully.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        private void OnSendComplete(IAsyncResult asyncResult)
        {
            try
            {
                this.socket.EndSend(asyncResult);

                // Raise our data sent event.
                if (this.DataSent != null)
                {
                    this.DataSent(this, new ConnectionArgs(this));
                }
            }
            catch
            {
                this.OnConnectionDisconnect();
            }
        }

        /// <summary>The callback function invoked when the socket detects any client data was received.</summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        private void OnDataReceived(IAsyncResult asyncResult)
        {
            try
            {
                int iRx;

                if (this.socket.Connected)
                {
                    // Complete the BeginReceive() asynchronous call by EndReceive() method
                    // which will return the number of characters written to the stream 
                    // by the client
                    iRx = this.socket.EndReceive(asyncResult);

                    // If the number of bytes received is 0 then something fishy is going on, so
                    // we close the socket.
                    if (iRx == 0)
                    {
                        this.OnConnectionDisconnect();
                    }
                    else
                    {
                        // Raise the Data Received Event. Signals that some data has arrived.
                        if (this.DataReceived != null)
                        {
                            this.DataReceived(this, new ConnectionArgs(this));
                        }

                        // Continue the waiting for data on the Socket
                        this.ListenForData();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // If we're shutting down, quietly ignore these exceptions and try to close the connection.
                this.OnConnectionDisconnect();
            }
            catch (ThreadAbortException)
            {
                // If we're shutting down, quietly ignore these exceptions and try to close the connection.
                this.OnConnectionDisconnect();
            }
            catch (Exception ex)
            {
                // In order to isolate connection-specific issues, we're going to trap the exception, log
                // the details, and kill that connection.  (Other connections and the game itself should
                // be able to continue through such situations.)
                string ip = this.CurrentIPAddress == null ? "[null]" : this.CurrentIPAddress.ToString();
                string format = "Exception encountered for connection:{0}IP: {1} (ID {2}):{0}{3}";
                string message = string.Format(format, Environment.NewLine, ip, this.ID, ex.ToDeepString());
                this.connectionHost.InformSubscribedSystem(message);

                // If the debugger is attached, we probably want to break now in order to better debug 
                // the issue closer to where it occurred; if your debugger broke here you may want to 
                // look at the stack trace to see where the exception originated.
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                this.OnConnectionDisconnect();
            }
        }

        /// <summary>Disconnects the sockets and raises the disconnected event.</summary>
        private void OnConnectionDisconnect()
        {
            if (this.socket.Connected)
            {
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();

                if (this.ClientDisconnected != null)
                {
                    this.ClientDisconnected(this, new ConnectionArgs(this));
                }
            }
        }
    }
}