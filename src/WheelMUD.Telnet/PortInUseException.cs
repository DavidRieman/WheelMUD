// Copyright (c) WheelMUD Development Team.  See LICENSE.txt or https://github.com/DavidRieman/WheelMUD/#license

namespace WheelMUD.Telnet
{
    /// <summary>Exception for the attempted socket port being in use.</summary>
    /// <param name="message">The exception message.</param>
    public class PortInUseException(string message) : Exception(message)
    {
    }
}
