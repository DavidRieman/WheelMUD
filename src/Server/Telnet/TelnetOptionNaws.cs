//-----------------------------------------------------------------------------
// <copyright file="TelnetOptionNaws.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Class that handles the subnegotiation of a NAWS telnet option code.
//   Created: January 2007 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Class that handles the subnegotiation of a NAWS telnet option code.</summary>
    internal class TelnetOptionNaws : TelnetOption
    {
        /// <summary>Initializes a new instance of the TelnetOptionNaws class.</summary>
        /// <param name="wantOption">Whether the option is wanted or not.</param>
        /// <param name="connection">The connection.</param>
        public TelnetOptionNaws(bool wantOption, Connection connection)
            : base("naws", 31, wantOption, connection)
        {
        }
        
        /// <summary>Process the sub negotiation.</summary>
        /// <param name="data">The data to process.</param>
        public override void ProcessSubNegotiation(byte[] data)
        {
            if (data.Length > 3)
            {
                this.Connection.Terminal.Width = data[1];
                this.Connection.Terminal.Height = data[3];
            }
        }
    }
}