//-----------------------------------------------------------------------------
// <copyright file="IConnection.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Net;
using System.Text;
using WheelMUD.Core;
using WheelMUD.Interfaces;

namespace WheelMUD.Server.Interfaces
{
    /// <summary>An interface defining a Connection.</summary>
    public interface IConnection
    {
        /// <summary>Gets the ID of the connection.</summary>
        string ID { get; }

        /// <summary>Gets a value indicating whether the output sent to the client is resting at the beginning of a line.</summary>
        bool AtNewLine { get; }

        /// <summary>Gets or sets the last raw input received from the connection.</summary>
        string LastRawInput { get; set; }

        /// <summary>Gets the IPAddress for the connection.</summary>
        IPAddress CurrentIPAddress { get; }

        /// <summary>Gets or sets the output buffer stored for the connection.</summary>
        OutputBuffer OutputBuffer { get; }

        /// <summary>Gets the terminal for this connection.</summary>
        TerminalOptions TerminalOptions { get; }

        /// <summary>Gets the telnet option code handler for this connection.</summary>
        ITelnetCodeHandler TelnetCodeHandler { get; }

        /// <summary>Gets the byte data currently on the connection (IE hasn't been processed into an input string yet).</summary>
        byte[] Data { get; }

        /// <summary>Gets the textual representation of the data still waiting to be returned as an input string.</summary>
        StringBuilder Buffer { get; }

        /// <summary>Gets or sets the number of rows that are buffered.</summary>
        int PagingRowLimit { get; set; }

        /// <summary>Gets or sets the line terminator used on the last command.</summary>
        string LastInputTerminator { get; set; }

        /// <summary>Disconnect a connection from the server.</summary>
        void Disconnect();

        /// <summary>Send data to the connection.</summary>
        /// <param name="data">The data to send to the connection.</param>
        void Send(byte[] data);

        /// <summary>Send data to the connection.</summary>
        /// <param name="data">The string to send.</param>
        /// <param name="sendAllData">If true, send all data without letting the paging system pause the output.</param>
        void Send(string data, bool sendAllData = false);

        /// <summary>Doesn't modify the buffer but utilizes it to produce output.</summary>
        /// <param name="bufferDirection">Direction to process data</param>
        void ProcessBuffer(BufferDirection bufferDirection);
    }
}