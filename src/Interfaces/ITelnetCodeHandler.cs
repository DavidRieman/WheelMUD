//-----------------------------------------------------------------------------
// <copyright file="ITelnetCodeHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    using System.Collections.Generic;

    /// <summary>An interface defining a TelnetCodeHandler.</summary>
    public interface ITelnetCodeHandler
    {
        /// <summary>Gets a list of the telnet options recognised by the server.</summary>
        List<ITelnetOption> TelnetOptions { get; }

        /// <summary>Instruct the handler to process the data for telnet option codes</summary>
        /// <param name="data">The data to process</param>
        /// <returns>A byte array with the telnet option codes stripped out</returns>
        byte[] ProcessInput(byte[] data);

        /// <summary>Begin Negotiation of our telnet options.</summary>
        void BeginNegotiation();
    }
}