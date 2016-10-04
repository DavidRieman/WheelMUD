//-----------------------------------------------------------------------------
// <copyright file="Terminal.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Class to hold Termial Options and info.
//   Created: January 2007 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server
{
    using WheelMUD.Interfaces;

    /// <summary>The terminal options used for a connection.</summary>
    public class Terminal : ITerminal
    {
        /// <summary>The terminal type.</summary>
        private string terminalType = string.Empty;

        /// <summary>The version of the client connected.</summary>
        private string version = string.Empty;

        /// <summary>The connected client.</summary>
        private string client = string.Empty;

        /// <summary>Whether or not to use ANSI for this connection.</summary>
        private bool useANSI = true;

        /// <summary>Whether or not to use a word wrap for this connection.</summary>
        private bool useWordWrap = true;

        /// <summary>Whether or not to use a buffer for this connection.</summary>
        private bool useBuffer = true;

        /// <summary>Gets or sets the height of the users terminal.</summary>
        public int Height
        {
            get;
            set;
        }

        /// <summary>Gets or sets the width of the users terminal.</summary>
        public int Width
        {
            get;
            set;
        }

        /// <summary>Gets or sets a value indicating whether the client wants ANSI.</summary>
        public bool UseANSI
        {
            get { return this.useANSI; }
            set { this.useANSI = value; }
        }

        /// <summary>Gets or sets a value indicating whether the client wants MXP.</summary>
        public bool UseMXP
        {
            get;
            set;
        }

        /// <summary>Gets or sets a value indicating whether the client wants MCCP.</summary>
        public bool UseMCCP
        {
            get;
            set;
        }

        /// <summary>Gets or sets a value indicating whether the client wants echo.</summary>
        public bool UseEcho
        {
            get;
            set;
        }

        /// <summary>Gets or sets a value indicating whether the client wants word-wrapping.</summary>
        public bool UseWordWrap
        {
            get { return this.useWordWrap; }
            set { this.useWordWrap = value; }
        }

        /// <summary>Gets or sets a value indicating whether the server side text output buffer should be used.</summary>
        public bool UseBuffer
        {
            get { return this.useBuffer; }
            set { this.useBuffer = value; }
        }

        /// <summary>Gets or sets the type of terminal attached (used to decide if we can send MXP, ANSI, etc.)</summary>
        public string TerminalType
        {
            get { return this.terminalType; }
            set { this.terminalType = value; }
        }

        /// <summary>Gets or sets the version of the client being used to connect.</summary>
        /// <remarks>This is only available if the client supports the Version tag specified in the MXP protocol.</remarks>
        public string Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        /// <summary>Gets or sets the client being used to connect.</summary>
        /// <remarks>This is only available if the client supports the Client tag specified in the MXP protocol.</remarks>
        public string Client
        {
            get { return this.client; }
            set { this.client = value; }
        }
    }
}