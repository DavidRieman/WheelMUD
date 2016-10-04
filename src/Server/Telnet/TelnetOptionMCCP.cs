//-----------------------------------------------------------------------------
// <copyright file="TelnetOptionMCCP.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Class that handles the subnegotiation of an MCCP (Compression) telnet option code.
//   Created: January 2007 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Class that handles the subnegotiation of an MCCP (Compression) telnet option code.</summary>
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
            if (this.UsState == TelnetOptionState.YES)
            {
                this.Connection.Terminal.UseMCCP = true;
            }
            else
            {
                this.Connection.Terminal.UseMCCP = false;
            }
        }
    }
}