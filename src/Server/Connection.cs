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
using WheelMUD.Core;
using WheelMUD.Interfaces;
using WheelMUD.Server.Interfaces;
using WheelMUD.Server.Telnet;
using WheelMUD.Utilities;
using WheelMUD.Utilities.Interfaces;
using WheelTelnet;

namespace WheelMUD.Server
{
    /// <summary>Represents a connection to a client.</summary>
    /// <remarks>
    /// Connection wraps a TelnetConnection to provide a more MUD-centric connection which abstracts some Telnet protocol details.
    /// For example, this class handles details like byte[] data conversion so the rest of the string-focued MUD code doesn't need to.
    /// It will also handle connection-specific details like accumulation of input over time (as Telnet may stream it in across
    /// several packets, especially in char-at-a-time mode) and buffering of output back to that client for paging purposes, etc.
    /// </remarks>
    public class Connection : IConnection
    {
        /// <summary>The telnet command bytes to indicate the next load of data is compressed.</summary>
        /// <remarks>The sub request is IAC SB COMPRESS2(86) IAC SE.</remarks>
        private static readonly byte[] reponseDataIsCompressed = [TelnetCommandByte.IAC, TelnetCommandByte.SB, 86, TelnetCommandByte.IAC, TelnetCommandByte.SE];

        // After porting to .NET Core, it seems we are no longer able to use the 8-bit ASCII
        // encoder "Encoding.GetEncoding(437)", so we went back to this 7-bit ASCII encoder.
        public static readonly Encoding CurrentEncoding = Encoding.ASCII;

        /// <summary>
        /// If true, replicate ALL output going to all connections to the console as well.
        /// Prints in a convenient debugging format to demonstrate special characters too (and assumes the console window can handle color output).
        /// </summary>
        private static readonly bool DebugConnectionsOutgoingData = true;

        /// <summary>
        /// If true, replicate ALL incoming input from all connections to the console as well.
        /// Prints in a convenient debugging format to demonstrate special characters too (and assumes the console window can handle color output).
        /// </summary>
        private static readonly bool DebugConnectionsIncomingData = true;

        /// <summary>The threshold, in characters, beyond which MCCP should be used.</summary>
        private const int MCCPThreshold = 100;

        /// <summary>The TelnetConnection upon which this server connection is based.</summary>
        private readonly TelnetConnection telnetConnection;

        /// <summary>The hosting system of this connection.</summary>
        private readonly ISubSystem connectionHost;

        /// <summary>How many rows of text the client can handle as a single display page.</summary>
        private int pagingRowLimit = 1000;

        /// <summary>Initializes a new instance of the <see cref="Connection"/> class.</summary>
        /// <param name="socket">The socket upon which this connection is to be based.</param>
        /// <param name="connectionHost">The system hosting this connection.</param>
        public Connection(TelnetConnection telnetConnection, ISubSystem connectionHost)
        {
            TerminalOptions = new TerminalOptions();
            this.telnetConnection = telnetConnection;
            telnetConnection.ErrorEncountered += (Exception ex) =>
            {
                // In order to isolate connection-specific issues, we're going to trap the exception, log the details,
                // and TelnetConnection by default already kills that connection.  (Other connections and the game
                // itself should be expected to continue running without problem upon connection-specific problems.)
                string ip = CurrentIPAddress == null ? "[null]" : CurrentIPAddress.ToString();
                string message = $"Exception encountered for connection:{Environment.NewLine}IP: {ip}, ID {ID}:{Environment.NewLine}{ex.ToDeepString()}";
                connectionHost?.InformSubscribedSystem(message);

                // If the debugger is attached, we probably want to break now in order to better debug 
                // the issue closer to where it occurred; if your debugger broke here you may want to 
                // look at the stack trace to see where the exception originated.
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
            };

            // Subscribe to data received events from the underlying telnet connection.
            telnetConnection.DataReceived += (int connectionId, byte[] data) =>
            {
                // Process telnet protocol codes and extract actual user input.
                Data = TelnetCodeHandler.ProcessInput(data);

                // Raise our data received event if we have actual user input.
                if (Data != null && Data.Length > 0)
                {
                    DataReceived?.Invoke(this);
                }
            };

            ID = Guid.NewGuid().ToString();
            TelnetCodeHandler = new TelnetCodeHandler(this);
            this.connectionHost = connectionHost;
        }

        public bool AtNewLine { get; private set; } = true;

        /// <summary>Gets the Terminal Options of this connection.</summary>
        public TerminalOptions TerminalOptions { get; private set; }

        /// <summary>Gets the ID of this connection.</summary>
        public string ID { get; private set; }

        /// <summary>Gets the IP Address for this connection.</summary>
        public IPAddress CurrentIPAddress { get { return telnetConnection?.CurrentIPAddress; } }

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

        /// <summary>Gets or sets the buffer of data not yet passed as an action.</summary>
        public StringBuilder Buffer { get; set; } = new StringBuilder();

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
        public OutputBuffer OutputBuffer { get; private set; } = new OutputBuffer();

        /// <summary>Event raised when data is received on this connection.</summary>
        public event Action<Connection> DataReceived;

        /// <summary>Disconnects the connection.</summary>
        public void Disconnect()
        {
            telnetConnection.Disconnect();
        }

        /// <summary>Sends raw bytes to the connection.</summary>
        /// <param name="data">The data to send to the connection.</param>
        public void Send(byte[] data)
        {
            try
            {
                if (telnetConnection.IsConnected)
                {
                    if (DebugConnectionsOutgoingData)
                    {
                        DebugConsoleLogOutgoing(data);
                    }
                    telnetConnection.Send(data);
                }
            }
            catch (SocketException)
            {
                // Ignore for now.
            }
            catch (ObjectDisposedException)
            {
                // Ignore for now.
            }
        }

        /// <summary>Sends string data to the connection.</summary>
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
            byte[] bytes = CurrentEncoding.GetBytes(data);
            if (TerminalOptions.UseMCCP && data.Length > MCCPThreshold)
            {
                // Compress the data.
                bytes = MCCPHandler.Compress(bytes);
                Send(reponseDataIsCompressed);
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

        private static string HumanReadable(byte b)
        {
            // Decide what string to represent each byte in the debug mode with.
            // E.G. we might want to use "CR" and "LF" for those special characters to understand them quickly.
            switch (b)
            {
                case 0:  return "NUL";   // NULL character.
                case 10: return "LF";    // Line Feed.
                case 13: return "CR";    // Carriage Return.
                case TelnetCommandByte.SE:   return "SubE"; // Subnegotiation End.
                case TelnetCommandByte.SB:   return "SubB"; // Subnegotiation End.
                case TelnetCommandByte.WILL: return "WILL"; // Commonly after IAC, informs we will use a protocol mechanism.
                case TelnetCommandByte.WONT: return "WONT"; // Commonly after IAC, informs we won't use a protocol mechanism.
                case TelnetCommandByte.DO:   return "DO";   // Commonly after IAC, instruct other party to use a protocol mechanism.
                case TelnetCommandByte.DONT: return "DONT"; // Commonly after IAC, instruct other party not to use a protocol mechanism.
                case TelnetCommandByte.IAC:  return "IAC";  // Sequence Initialize and Escape Character. Opens telnet commands, etc.
                default: break;
            }
            return b.ToString();
        }

        private static void DebugConsoleLogOutgoing(byte[] data)
        {
            bool lastWasCR = false;
            var sb = new StringBuilder(data.Length);
            foreach (byte b in data)
            {
                // The nice printable characters range from 32 (space) up to 126 (tilde).
                bool printable = (b >= 32 && b <= 126);
                sb.Append(printable ? $"{AnsiSequences.ForegroundYellow}{(char)b}" : $"{AnsiSequences.ForegroundGreen}({AnsiSequences.ForegroundYellow}{HumanReadable(b)}{AnsiSequences.ForegroundGreen}){AnsiSequences.TextNormal}");

                // If it was a LF or NULL and the last character was CR, then we just print like [CR][LF]
                // and can follow that up now with a real console newline to match.
                if ((b == 0 || b == 10) && lastWasCR)
                {
                    sb.AppendLine();
                }

                // Finally, track whether THIS character was a CR, for the next pass to know.
                lastWasCR = b == 13;
            }
            sb.Append($"{AnsiSequences.TextNormal}");
            Console.Write(sb.ToString());
        }

        private static void DebugConsoleLogIncoming(byte[] data)
        {
            var sb = new StringBuilder(data.Length);
            foreach (byte b in data)
            {
                // The nice printable characters range from 32 (space) up to 126 (tilde).
                bool printable = (b >= 32 && b <= 126);
                sb.Append(printable ? $"{AnsiSequences.ForegroundRed}{(char)b}" : $"{AnsiSequences.ForegroundMagenta}({AnsiSequences.ForegroundRed}{HumanReadable(b)}{AnsiSequences.ForegroundMagenta})");
                if (b == 0 || b == 10)
                {
                    sb.AppendLine();
                }
            }
            sb.Append($"{AnsiSequences.TextNormal}");
            Console.Write(sb.ToString());
        }
    }
}
