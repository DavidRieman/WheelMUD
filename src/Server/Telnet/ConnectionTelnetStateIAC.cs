//-----------------------------------------------------------------------------
// <copyright file="ConnectionTelnetStateIAC.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Handles the negotiation of a telnet IAC option.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Handles the negotiation of a telnet IAC option.</summary>
    internal class ConnectionTelnetStateIAC : ConnectionTelnetState
    {
        /// <summary>Initializes a new instance of the ConnectionTelnetStateIAC class.</summary>
        /// <param name="parent">The parent telnet code handler which this object is created by.</param>
        public ConnectionTelnetStateIAC(TelnetCodeHandler parent)
            : base(parent)
        {
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="data">The data to be processed.</param>
        public override void ProcessInput(byte data)
        {
            switch (data)
            {
                case SB:
                    // We throw away the data and enter the subrequest state.
                    Parent.ChangeState(new ConnectionTelnetStateSubRequest(Parent));
                    break;
                case (int)TelnetResponseCode.WILL:
                    Parent.ChangeState(new ConnectionTelnetStateOptionCode(Parent, (int)TelnetResponseCode.WILL));
                    break;
                case (int)TelnetResponseCode.WONT:
                    Parent.ChangeState(new ConnectionTelnetStateOptionCode(Parent, (int)TelnetResponseCode.WONT));
                    break;
                case (int)TelnetResponseCode.DO:
                    Parent.ChangeState(new ConnectionTelnetStateOptionCode(Parent, (int)TelnetResponseCode.DO));
                    break;
                case (int)TelnetResponseCode.DONT:
                    Parent.ChangeState(new ConnectionTelnetStateOptionCode(Parent, (int)TelnetResponseCode.DONT));
                    break;
                case IAC:
                    // If its another IAC then it has been correctly escaped so we should pass it back.
                    Parent.AddToBuffer(data);

                    // Change our state back to Text
                    Parent.ChangeState(new ConnectionTelnetStateText(Parent));
                    break;
                default:
                    // If we don't recognise the code that came in, it must be dodgy, so throw it away.
                    // Change our state back to Text.
                    Parent.ChangeState(new ConnectionTelnetStateText(Parent));
                    break;
            }
        }
    }
}
