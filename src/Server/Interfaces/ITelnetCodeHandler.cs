//-----------------------------------------------------------------------------
// <copyright file="ITelnetCodeHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server.Interfaces;

namespace WheelMUD.Interfaces
{
    /// <summary>An interface defining a TelnetCodeHandler.</summary>
    public interface ITelnetCodeHandler
    {
        /// <summary>Find the TelnetOption class instance of the given type.</summary>
        /// <typeparam name="T">The ITelnetOption type to search for.</typeparam>
        /// <returns>The TelnetOption instance of the given type, or null if it can not be found.</returns>
        T FindOption<T>() where T : ITelnetOption;

        /// <summary>Instruct the handler to process the data for telnet option codes</summary>
        /// <param name="data">The data to process</param>
        /// <returns>A byte array with the telnet option codes stripped out</returns>
        byte[] ProcessInput(byte[] data);

        /// <summary>Begin Negotiation of our telnet options.</summary>
        void BeginNegotiation();
    }
}