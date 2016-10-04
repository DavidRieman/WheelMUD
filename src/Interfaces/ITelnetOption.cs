//-----------------------------------------------------------------------------
// <copyright file="ITelnetOption.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>An interface defining a TelnetOption.</summary>
    public interface ITelnetOption
    {
        /// <summary>Gets the name of the telnet option.</summary>
        string Name { get; }

        /// <summary>Gets the code number of the option.</summary>
        int OptionCode { get; }

        /// <summary>Attempt to enable the option.</summary>
        void Enable();

        /// <summary>Attempt to disable the option.</summary>
        void Disable();
    }
}