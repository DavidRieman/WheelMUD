//-----------------------------------------------------------------------------
// <copyright file="UserTelnetSettings.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    /// <summary>User overrides for telnet settings.</summary>
    /// <remarks>
    /// Nulls in nullable properties mean to use the default behavior (generally auto-detected through Telnet handshake).
    /// Otherwise, actual values (true/false) mean the user has a specific override in place until they manually change it again.
    /// </remarks>
    public class UserTelnetSettings
    {
        public bool? WantAnsi { get; set; }

        public bool? WantMCCP { get; set; }

        public bool? WantMXP { get; set; }
    }
}
