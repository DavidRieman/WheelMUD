//-----------------------------------------------------------------------------
// <copyright file="TelnetOptionEcho.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Class that handles the echo telnet option code.</summary>
    /// <remarks>See RFC857 (http://www.faqs.org/rfcs/rfc857.html).</remarks>
    /// <param name="wantOption">Whether the option is wanted or not.</param>
    /// <param name="connection">The connection.</param>
    internal class TelnetOptionEcho(bool wantOption, Connection connection) : TelnetOption("echo", 1, wantOption, connection)
    {
        /// <summary>Process the sub negotiation.</summary>
        /// <param name="data">The data to process.</param>
        public override void ProcessSubNegotiation(byte[] data)
        {
        }

        /// <summary>Post-processing as called after negotiation.</summary>
        public override void AfterNegotiation()
        {
            Connection.TerminalOptions.UseEcho = UsState == TelnetOptionState.YES;
        }
    }
}
