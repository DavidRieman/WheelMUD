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

        /// <summary>
        /// If true, replicate ALL output going to all connections to the console as well.
        /// Prints in a convenient debugging format to demonstrate special characters too (and assumes the console window can handle color output).
        /// </summary>
        private static bool DebugConnectionsOutgoingData = false;

        /// <summary>The threshold, in characters, beyond which MCCP should be used.</summary>
        private const int MCCPThreshold = 100;

        /// <summary>The socket upon which this connection is based.</summary>
        private readonly Socket socket;

        /// <summary>The hosting system of this connection.</summary>
        private readonly ISubSystem connectionHost;

        /// <summary>How many rows of text the client can handle as a single display page.</summary>
        private int pagingRowLimit = 1000;

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
            this.connectionHost = connectionHost;
        }

        public bool AtNewLine { get; private set; } = true;

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
                pagingRowLimit = (value <= 0 || value > 1000) ? 1000 : value;
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
                if (DebugConnectionsOutgoingData)
                {
                    DebugConsoleLog(data);
                }
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

        /// <summary>Sends string data to the connection</summary>
        /// <param name="data">The string to send.</param>
        /// <param name="sendAllData">If true, send all data without letting the paging system pause the output.</param>
        public void Send(string data, bool sendAllData = false)
        {
            // TODO: Eventually, certain large blocks of text might be good candidates for caching the final result due to high frequency of sending to clients (such as welcome
            //       messages or the most popular room descriptions and so on), so here might be a good place to perform an options-sensitive cache lookup of the source data to
            //       the final (potentially word-wrapped, potentially compressed) data. This could reduce total word-wrapping and compression work. (Such a potential improvement
            //       should be carefully measured for whether it is worth the complexity and potential bugs.) As such, it may make sense to coalesce the honored word wrap widths
            //       when formatting data below, into common groupings (such as rounding down to nearest multiple of 20 and enforcing a max) to increase those cache hits at the
            //       trade-off of not printing to the right-most characters of some terminal sizes. (This would probably look fine, generally.)

            // Check if the client wants to use compression (MCCP) and whether data is long enough to bother (as the overhead is quite high).
            byte[] bytes;
            if (TerminalOptions.UseMCCP && data.Length > MCCPThreshold)
            {
                // Compress the data.
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

            // Track whether we've finished with a NewLine, so we can avoid new lines of output going to the same one.
            AtNewLine = data.EndsWith(AnsiSequences.NewLine);
        }

        /// <summary>Sends data from the output buffer to the client.</summary>
        /// <param name="bufferDirection">Direction to move in the buffer.</param>
        public void ProcessBuffer(BufferDirection bufferDirection)
        {
            if (OutputBuffer.HasMoreData)
            {
                string[] output = OutputBuffer.GetRows(bufferDirection, PagingRowLimit);

                bool appendOverflow = OutputBuffer.HasMoreData;
                string data = (AtNewLine ? null : AnsiSequences.NewLine) + BufferHandler.Format(
                    output,
                    appendOverflow,
                    OutputBuffer.CurrentLocation,
                    OutputBuffer.Length);

                Send(data, true);
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

        private static string HumanReadable(byte b)
        {
            // Decide what string to represent each byte in the debug mode with.
            // E.G. we might want to use "CR" and "LF" for those special characters to understand them quickly.
            switch (b)
            {
                case 0: return "NUL";    // NULL character.
                case 10: return "LF";    // Line Feed.
                case 13: return "CR";    // Carriage Return.
                case 240: return "SubE"; // Subnegotiation End.
                case 250: return "SubB"; // Subnegotiation End.
                case 251: return "WILL"; // Commonly after IAC, informs we will use a protocol mechanism.
                case 252: return "WONT"; // Commonly after IAC, informs we won't use a protocol mechanism.
                case 253: return "DO";   // Commonly after IAC, instruct other party to use a protocol mechanism.
                case 254: return "DONT"; // Commonly after IAC, instruct other party not to use a protocol mechanism.
                case 255: return "IAC";  // Sequence Initialize and Escape Character. Opens telnet commands, etc.
                default: break;
            }
            return b.ToString();
        }

        private static void DebugConsoleLog(byte[] data)
        {
            bool lastWasCR = false;
            var sb = new StringBuilder(data.Length);
            foreach (byte b in data)
            {
                // The nice printable characters range from 32 (space) up to 126 (tilde).
                // We don't want to try to print control characters, DEL character (127), etc.
                bool printable = (b >= 32 && b <= 126);
                sb.Append(printable ? (char)b : $"{AnsiSequences.ForegroundGreen}({AnsiSequences.ForegroundYellow}{HumanReadable(b)}{AnsiSequences.ForegroundGreen}){AnsiSequences.TextNormal}");

                // If it was a LF or NULL and the last character was CR, then we just print like [CR][LF]
                // and can follow that up now with a real console newline to match.
                if ((b == 0 || b == 10) && lastWasCR)
                {
                    sb.AppendLine();
                }

                // Finally, track whether THIS character was a CR, for the next pass to know.
                lastWasCR = b == 13;
            }
            Console.Write(sb.ToString());
        }
    }
}
