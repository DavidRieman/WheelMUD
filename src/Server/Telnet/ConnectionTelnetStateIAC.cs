//-----------------------------------------------------------------------------
// <copyright file="ConnectionTelnetStateIAC.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>Handles the negotiation of a telnet "Interpret As Command" (IAC) option.</summary>
    /// <remarks>Initializes a new instance of the ConnectionTelnetStateIAC class.</remarks>
    /// <param name="parent">The parent telnet code handler which this object is created by.</param>
    internal class ConnectionTelnetStateIAC(TelnetCodeHandler parent) : ConnectionTelnetState(parent)
    {
        /// <summary>Process the specified input.</summary>
        /// <param name="data">The data to be processed.</param>
        public override void ProcessInput(byte data)
        {
            switch (data)
            {
                case TelnetCommandByte.SB:
                    // We throw away the data and enter the subrequest state.
                    Parent.ChangeState(new ConnectionTelnetStateSubRequest(Parent));
                    break;
                case TelnetCommandByte.WILL:
                    Parent.ChangeState(new ConnectionTelnetStateOptionCode(Parent, TelnetCommandByte.WILL));
                    break;
                case TelnetCommandByte.WONT:
                    Parent.ChangeState(new ConnectionTelnetStateOptionCode(Parent, TelnetCommandByte.WONT));
                    break;
                case TelnetCommandByte.DO:
                    Parent.ChangeState(new ConnectionTelnetStateOptionCode(Parent, TelnetCommandByte.DO));
                    break;
                case TelnetCommandByte.DONT:
                    Parent.ChangeState(new ConnectionTelnetStateOptionCode(Parent, TelnetCommandByte.DONT));
                    break;
                case TelnetCommandByte.IAC:
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
