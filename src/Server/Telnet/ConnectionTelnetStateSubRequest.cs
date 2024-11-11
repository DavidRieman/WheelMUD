//-----------------------------------------------------------------------------
// <copyright file="ConnectionTelnetStateSubRequest.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>TODO: What is this?</summary>
    /// <remarks>Initializes a new instance of the ConnectionTelnetStateSubRequest class.</remarks>
    /// <param name="parent">The parent telnet code handler which this object is created by.</param>
    internal class ConnectionTelnetStateSubRequest(TelnetCodeHandler parent) : ConnectionTelnetState(parent)
    {
        /// <summary>Process the specified input.</summary>
        /// <param name="data">The data to be processed.</param>
        public override void ProcessInput(byte data)
        {
            switch (data)
            {
                case TelnetCommandByte.IAC:
                    // When we see an IAC byte in this state we change state to SubRequestIAC.
                    Parent.ChangeState(new ConnectionTelnetStateSubRequestIAC(Parent));
                    break;
                default:
                    // Add the data to the buffer.
                    Parent.SubRequestBuffer.Add(data);
                    break;
            }
        }
    }
}
