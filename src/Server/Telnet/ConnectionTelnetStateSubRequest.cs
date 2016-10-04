//-----------------------------------------------------------------------------
// <copyright file="ConnectionTelnetStateSubRequest.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>@@@ What is this?</summary>
    internal class ConnectionTelnetStateSubRequest : ConnectionTelnetState
    {
        /// <summary>Initializes a new instance of the ConnectionTelnetStateSubRequest class.</summary>
        /// <param name="parent">The parent telnet code handler which this object is created by.</param>
        public ConnectionTelnetStateSubRequest(TelnetCodeHandler parent)
            : base(parent)
        {
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="data">The data to be processed.</param>
        public override void ProcessInput(byte data)
        {
            switch (data)
            {
                case IAC:
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
