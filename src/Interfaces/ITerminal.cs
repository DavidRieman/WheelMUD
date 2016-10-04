//-----------------------------------------------------------------------------
// <copyright file="ITerminal.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An interface describing Terminal settings.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>An interface describing Terminal settings.</summary>
    public interface ITerminal
    {
        /// <summary>Gets or sets the height of the users terminal.</summary>
        int Height { get; set; }

        /// <summary>Gets or sets the width of the users terminal.</summary>
        int Width { get; set; }

        /// <summary>Gets or sets a value indicating whether a Client wants ANSI.</summary>
        bool UseANSI { get; set; }

        /// <summary>Gets or sets a value indicating whether a Client wants MXP.</summary>
        bool UseMXP { get; set; }

        /// <summary>Gets or sets a value indicating whether the connection wants MCP.</summary>
        bool UseMCCP { get; set; }

        /// <summary>Gets or sets a value indicating whether the connection wants echo.</summary>
        bool UseEcho { get; set; }

        /// <summary>Gets or sets a value indicating whether the connection wants wordwrap.</summary>
        bool UseWordWrap { get; set; }

        /// <summary>Gets or sets a value indicating whether to use a server side text output buffer.</summary>
        bool UseBuffer { get; set; }

        /// <summary>Gets or sets the type of terminal attached</summary>
        /// <remarks>We can use this to decide what we can send IE MXP, ANSI.</remarks>
        string TerminalType { get; set; }

        /// <summary>Gets or sets the version of the client being used to connect.</summary>
        /// <remarks>This is only available if the client supports the Version tag specified in the MXP protocol.</remarks>
        string Version { get; set; }

        /// <summary>Gets or sets the client being used to connect.</summary>
        /// <remarks>This is only available if the client supports the Client tag specified in the MXP protocol.</remarks>
        string Client { get; set; }
    }
}