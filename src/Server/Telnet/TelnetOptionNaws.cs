//-----------------------------------------------------------------------------
// <copyright file="TelnetOptionNaws.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Class that handles the subnegotiation of Negotiate About Windows Size (NAWS) telnet option code.</summary>
    /// <remarks>see: http://www.faqs.org/rfcs/rfc1073.html</remarks>
    internal class TelnetOptionNaws : TelnetOption
    {
        /// <summary>The assumed terminal height to format output against, should NAWS negotation be denied or malformed.</summary>
        public static readonly int DefaultTerminalHeight = 20;

        /// <summary>The assumed terminal width to format output against, should NAWS negotiation be denied or malformed.</summary>
        public static readonly int DefaultTerminalWidth = 80;

        // Since a MUD will often perform word-wrapping and paging and such, we should restrict nonsense sizes from imposing excessive burdens (and possibly crashing) the server (like impossible word wrapping challenges).
        // These values are chosen to support the most extreme but potentially real possibilities. E.G. suppose a telnet client gets developed targeting older smartphones with small screens.
        // Ultimately higher level systems can (and probably should) impose even more restrictive values than these low-level restrictions, at the systems that rely on them (E.G. word wrapping and output caching levels).
        private const int MinimumHonoredTerminalHeight = 6;
        private const int MinimumHonoredTerminalWidth = 20;

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
                int height = 256 * data[2] + data[3];
                int width = 256 * data[0] + data[1];
                Connection.Terminal.Height = height <= 0 ? DefaultTerminalHeight : height;
                Connection.Terminal.Width = width <= 0 ? DefaultTerminalWidth : width;
            }
            if (Connection.Terminal.Width < MinimumHonoredTerminalWidth)
            {
                Connection.Terminal.Width = MinimumHonoredTerminalWidth;
            }
            if (Connection.Terminal.Height < MinimumHonoredTerminalHeight)
            {
                Connection.Terminal.Height = MinimumHonoredTerminalHeight;
            }
        }
    }
}