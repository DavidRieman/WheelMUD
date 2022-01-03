//-----------------------------------------------------------------------------
// <copyright file="TelnetOptionTermType.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Text;

namespace WheelMUD.Server.Telnet
{
    /// <summary>Class that handles the Terminal Type telnet option code.</summary>
    internal class TelnetOptionTermType : TelnetOption
    {
        /// <summary>Initializes a new instance of the TelnetOptionTermType class.</summary>
        /// <param name="wantOption">Whether the option is wanted or not.</param>
        /// <param name="connection">The connection.</param>
        public TelnetOptionTermType(bool wantOption, Connection connection)
            : base("termtype", 24, wantOption, connection)
        {
        }

        /// <summary>Process the sub negotiation.</summary>
        /// <param name="data">The data to process.</param>
        public override void ProcessSubNegotiation(byte[] data)
        {
            // Set the terminal type to the sub negotiation request.
            // The RFC (1091) says that we can loop through the different terminal types
            // that the terminal supports and the last one we receive is the one the terminal
            // sets itself to. We only get the first one which is normally the primary terminal emulation.
            Connection.TerminalOptions.TerminalType = Encoding.ASCII.GetString(data).TrimStart(new char[] { '\0' }).ToLower();
        }

        /// <summary>Post-processing as called after negotiation.</summary>
        public override void AfterNegotiation()
        {
            // If we got a successful turn on.
            if (UsState == TelnetOptionState.YES)
            {
                // Send the request for terminal information. For example:
                // IAC SB TERM SEND IAC SE
                Connection.Send(new byte[] { 255, 250, 24, 1, 255, 240 });
            }
        }
    }
}