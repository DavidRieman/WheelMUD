//-----------------------------------------------------------------------------
// <copyright file="TelnetOptionMCCP.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Class that handles the subnegotiation of an MCCP2 (Compression) telnet option code.</summary>
    /// <remarks>
    /// Although MCCP version 1 was a separate option code (85), we intentionally do not support the original:
    /// MCCP v1 was quickly deprecated due to an invalid subnegotiation sequence. Thus usually the only version
    /// of MCCP one will want to support will be this version (MCCP2 via option code 86).
    /// Note there may be suggestion of an MCCP3, but it is likely not well supported and generally may be
    /// considered fully redundant as MCCP2 can technically already be bi-directional if a client wants that.
    /// (The server does not support client-side compression yet, but it should be easy to add if found useful.)
    /// </remarks>
    internal class TelnetOptionMCCP : TelnetOption
    {
        /// <summary>Initializes a new instance of the TelnetOptionMCCP class.</summary>
        /// <param name="wantOption">Whether the option is wanted or not.</param>
        /// <param name="connection">The connection.</param>
        public TelnetOptionMCCP(bool wantOption, Connection connection)
            : base("compress2", 86, wantOption, connection)
        {
        }

        /// <summary>Process the sub negotiation.</summary>
        /// <param name="data">The data to process.</param>
        public override void ProcessSubNegotiation(byte[] data)
        {
        }

        /// <summary>Post-processing as called after negotiation.</summary>
        public override void AfterNegotiation()
        {
            Connection.TerminalOptions.UseMCCP = UsState == TelnetOptionState.YES;
        }
    }
}