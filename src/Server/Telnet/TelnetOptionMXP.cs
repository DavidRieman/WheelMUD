//-----------------------------------------------------------------------------
// <copyright file="TelnetOptionMXP.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Class that handles the sub-negotiation of the MUD eXtension Protocol (MXP) telnet option code.</summary>
    /// <remarks>See https://www.zuggsoft.com/zmud/mxp.htm</remarks>
    /// <param name="wantOption">Whether the option is wanted or not.</param>
    /// <param name="connection">The connection.</param>
    internal class TelnetOptionMXP(bool wantOption, Connection connection) : TelnetOption("mxp", 91, wantOption, connection)
    {

        /// <summary>Represents version response state in the MXP telnet option.</summary>
        internal enum ResponseState
        {
            /// <summary>The 'text' version response state.</summary>
            Text,

            /// <summary>The 'esc' version response state.</summary>
            Esc,

            /// <summary>The 'open bracket' version response state.</summary>
            OpenBracket,

            /// <summary>The 'one' version response state.</summary>
            One,

            /// <summary>The 'z' version response state.</summary>
            Z,

            /// <summary>The 'version tag' version response state.</summary>
            VersionTag
        }

        /// <summary>Gets or sets a value indicating whether we are awaiting a version response.</summary>
        public bool AwaitingVersionResponse { get; set; }

        /// <summary>Gets or sets the version response state.</summary>
        internal ResponseState VersionResponseState { get; set; }

        /// <summary>Gets or sets the version response buffer contents.</summary>
        internal string VersionResponseBuffer { get; set; } = string.Empty;

        /// <summary>Process the sub negotiation.</summary>
        /// <param name="data">The data to process.</param>
        public override void ProcessSubNegotiation(byte[] data)
        {
        }

        /// <summary>Post-processing as called after negotiation.</summary>
        public override void AfterNegotiation()
        {
            // If we got a successful turn on.
            if (UsState == TelnetOptionState.YES)
            {
                Connection.TerminalOptions.UseMXP = true;

                // Request a version tag from the client.
                Connection.Send(MXPHandler.SecureLineCode + "<VERSION>", true);
                AwaitingVersionResponse = true;

                // TODO: Send our mxp headers.
            }
        }
    }
}