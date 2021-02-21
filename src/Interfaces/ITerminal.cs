//-----------------------------------------------------------------------------
// <copyright file="ITerminal.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>An interface describing Terminal settings.</summary>
    public interface ITerminal
    {
        /// <summary>Gets or sets the client being used to connect.</summary>
        /// <remarks>This is only available if the client supports the Client tag specified in the MXP protocol.</remarks>
        string Client { get; set; }

        /// <summary>Gets or sets the height of the users terminal.</summary>
        /// <remarks>Technically these could be ushort, but we're not storing millions of these, ushort can be slower to process on modern machines, etc.</remarks>
        int Height { get; set; }

        /// <summary>Gets or sets the width of the users terminal.</summary>
        /// <remarks>Technically these could be ushort, but we're not storing millions of these, ushort can be slower to process on modern machines, etc.</remarks>
        int Width { get; set; }

        /// <summary>Gets or sets a value indicating whether the client wants to communicate ANSI escape sequences (for output colorization and such).</summary>
        bool UseANSI { get; set; }

        /// <summary>Gets or sets a value indicating whether the client wants to use the MUD eXtension Protocol (MXP).</summary>
        bool UseMXP { get; set; }

        /// <summary>Gets or sets a value indicating whether the client wants to use the MUD Client Compression Protocol (MCCP).</summary>
        bool UseMCCP { get; set; }

        /// <summary>Gets or sets a value indicating whether the client wants to receive output text echoing what was sent to the server.</summary>
        bool UseEcho { get; set; }

        /// <summary>Gets or sets a value indicating whether the client wants the server to perform word-wrapping.</summary>
        bool UseWordWrap { get; set; }

        /// <summary>Gets or sets a value indicating whether the server side text output buffer should be used.</summary>
        bool UseBuffer { get; set; }

        /// <summary>Gets or sets the type of terminal attached (used to decide if we can send MXP, ANSI, etc.)</summary>
        string TerminalType { get; set; }

        /// <summary>Gets or sets the version of the client being used to connect.</summary>
        /// <remarks>This is only available if the client supports the Version tag specified in the MXP protocol.</remarks>
        string Version { get; set; }
    }
}