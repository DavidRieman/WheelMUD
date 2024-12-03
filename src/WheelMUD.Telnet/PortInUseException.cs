//-----------------------------------------------------------------------------
// <copyright company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Telnet
{
    /// <summary>Exception for the attempted socket port being in use.</summary>
    /// <param name="message">The exception message.</param>
    public class PortInUseException(string message) : Exception(message)
    {
    }
}
