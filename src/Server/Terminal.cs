//-----------------------------------------------------------------------------
// <copyright file="Terminal.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using WheelMUD.Interfaces;
    using WheelMUD.Server.Telnet;

    /// <summary>The terminal options used for a connection.</summary>
    public class Terminal : ITerminal
    {
        /// <summary>Gets or sets the client being used to connect.</summary>
        /// <remarks>This is only available if the client supports the Client tag specified in the MXP protocol.</remarks>
        public string Client { get; set; } = string.Empty;

        /// <summary>Gets or sets the height of the users terminal.</summary>
        public int Height { get; set; } = TelnetOptionNaws.DefaultTerminalHeight;

        /// <summary>Gets or sets the width of the users terminal.</summary>
        public int Width { get; set; } = TelnetOptionNaws.DefaultTerminalWidth;

        /// <summary>Gets or sets a value indicating whether the client wants ANSI.</summary>
        public bool UseANSI { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether the client wants MXP.</summary>
        public bool UseMXP { get; set; }

        /// <summary>Gets or sets a value indicating whether the client wants MCCP.</summary>
        public bool UseMCCP { get; set; }

        /// <summary>Gets or sets a value indicating whether the client wants echo.</summary>
        public bool UseEcho { get; set; }

        /// <summary>Gets or sets a value indicating whether the client wants word-wrapping.</summary>
        public bool UseWordWrap { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether the server side text output buffer should be used.</summary>
        public bool UseBuffer { get; set; } = true;

        /// <summary>Gets or sets the type of terminal attached (used to decide if we can send MXP, ANSI, etc.)</summary>
        public string TerminalType { get; set; } = string.Empty;

        /// <summary>Gets or sets the version of the client being used to connect.</summary>
        /// <remarks>This is only available if the client supports the Version tag specified in the MXP protocol.</remarks>
        public string Version { get; set; } = string.Empty;
    }
}