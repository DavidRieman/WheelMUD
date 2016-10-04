//-----------------------------------------------------------------------------
// <copyright file="TelnetOptionMSP.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Class that handles the subnegotiation of an MSP (MUD Sound Protocol) telnet option code.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    using System;

    /// <summary>Class that handles the subnegotiation of an MSP (MUD Sound Protocol) telnet option code.</summary>
    /// <remarks>@@@ TODO: Implement; admins who choose to use this feature should have the telnet options part ready for them.</remarks>
    internal class TelnetOptionMSP : TelnetOption
    {
        /// <summary>Initializes a new instance of the TelnetOptionMSP class.</summary>
        /// <param name="wantOption">Whether the option is wanted or not.</param>
        /// <param name="connection">The connection.</param>
        public TelnetOptionMSP(bool wantOption, Connection connection)
            : base("msp", 90, wantOption, connection)
        {
            throw new NotImplementedException();
        }
    }
}