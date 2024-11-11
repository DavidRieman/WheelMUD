//-----------------------------------------------------------------------------
// <copyright file="TelnetCommandByte.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Convenient names and documentation for common "command bytes" used in the Telnet protocol.</summary>
    /// <remarks>See online "RFC 854" for detailed protocol information.</remarks>
    public static class TelnetCommandByte
    {
        /// <summary>The "Subnegotiation: Begin" (SB) byte.</summary>
        public const byte SE = 240;

        /// <summary>The "Subnegotiation: End" (SE) byte.</summary>
        public const byte SB = 250;

        /// <summary>The "will" telnet response code.</summary>
        public const byte WILL = 251;

        /// <summary>The "won't" telnet response code.</summary>
        public const byte WONT = 252;

        /// <summary>The "do" telnet response code.</summary>
        public const byte DO = 253;

        /// <summary>The "don't" telnet response code.</summary>
        public const byte DONT = 254;

        /// <summary>The "Interpret As Command" (IAC) byte.</summary>
        /// <remarks>When received, the next byte is to be treated as a command rather than data.</remarks>
        public const byte IAC = 255;
    }
}
