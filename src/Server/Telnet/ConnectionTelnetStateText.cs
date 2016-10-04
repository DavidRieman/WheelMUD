//-----------------------------------------------------------------------------
// <copyright file="ConnectionTelnetStateText.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Handles the incoming data over our connection.
//   This is the default state for our connections data stream.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Handles the incoming data over our connection.</summary>
    /// <remarks>This is the default state for our connections data stream.</remarks>
    internal class ConnectionTelnetStateText : ConnectionTelnetState
    {
        /// <summary>Initializes a new instance of the ConnectionTelnetStateText class.</summary>
        /// <param name="parent">The parent telnet code handler which this object is created by.</param>
        public ConnectionTelnetStateText(TelnetCodeHandler parent)
            : base(parent)
        {
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="data">The data to be processed.</param>
        public override void ProcessInput(byte data)
        {
            // All we do here is iterate over the data adding data to our buffer
            // if we see an IAC (255) byte then we change our state to IAC and process.
            if (data == IAC)
            {
                Parent.ChangeState(new ConnectionTelnetStateIAC(Parent));
            }
            else
            {
                Parent.AddToBuffer(data);
            }
        }
    }
}
