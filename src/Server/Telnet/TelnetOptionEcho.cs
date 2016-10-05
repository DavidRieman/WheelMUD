//-----------------------------------------------------------------------------
// <copyright file="TelnetOptionEcho.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Class that handles the echo telnet option code.
//   Created: January 2007 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Class that handles the echo telnet option code.</summary>
    /// <remarks>See RFC857 (http://www.faqs.org/rfcs/rfc857.html)</remarks>
    internal class TelnetOptionEcho : TelnetOption
    {
        /// <summary>Initializes a new instance of the TelnetOptionEcho class.</summary>
        /// <param name="wantOption">Whether the option is wanted or not.</param>
        /// <param name="connection">The connection.</param>
        public TelnetOptionEcho(bool wantOption, Connection connection)
            : base("echo", 1, wantOption, connection)
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
            switch (this.UsState)
            {
                case TelnetOptionState.YES:
                    this.Connection.Terminal.UseEcho = true;
                    break;
                case TelnetOptionState.NO:
                    this.Connection.Terminal.UseEcho = false;
                    break;
            }
        }
    }
}