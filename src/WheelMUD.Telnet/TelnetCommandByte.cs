//-----------------------------------------------------------------------------
// <copyright file="TelnetCommandByte.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Telnet
{
    /// <summary>Convenient names and documentation for common "command bytes" used in the Telnet protocol.</summary>
    /// <remarks>See online "RFC 854" for detailed protocol information.</remarks>
    public static class TelnetCommandByte
    {
        /// <summary>The "Subnegotiation: Begin" (SB) byte.</summary>
        public const byte SE = 240;

        /// <summary>The "no-operation" (NOP) byte.</summary>
        /// <remarks>
        /// Some servers and clients may opt to send IAC NOP after some idling period to help keep a connection alive.
        /// (E.G. you may wish to avoid most or all IAC commands, but especially NOP, from influencing user idle time tracking.)
        /// </remarks>
        public const byte NOP = 241;

        // Ignored for now: public const byte DataMark = 242;

        /// <summary>The "break" (BRK) byte.</summary>
        public const byte BRK = 243;

        /// <summary>The "interrupt process" (IP) byte.</summary>
        public const byte IP = 244;

        /// <summary>The "abort output" (AO) byte.</summary>
        public const byte AO = 245;

        /// <summary>The "are you there" (AYT) byte.</summary>
        public const byte AYT = 246;

        /// <summary>The "erase character" (EC) byte.</summary>
        /// <remarks>
        /// Since this is not honored by all character-at-a-time (echo) contexts (E.G. Windows Telnet.exe currently), one might simply
        /// ignore this command byte and choose a strategy like "move cursor left, right a space, move character left again" to
        /// approximate erasing a character from the client's screen. However, it might be slightly more efficient (and more correct)
        /// to send this IAC command byte to clients which are assumed (through TERMINAL-TYPE negotiation) to correctly support it.
        /// </remarks>
        public const byte EC = 247;

        /// <summary>The "erase line" (EL) byte.</summary>
        public const byte EL = 248;

        /// <summary>The "Go-Ahead" signal.</summary>
        /// <remarks>
        /// For example, server sending this to client upon a prompt can help indicate the server is waiting for user input.
        /// Due to how Telnet data streams, sending this can help a client know to display things like `>` prompts without CR/LF bytes.
        /// </remarks>
        public const byte GA = 249;

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
