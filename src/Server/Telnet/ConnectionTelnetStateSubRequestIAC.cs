//-----------------------------------------------------------------------------
// <copyright file="ConnectionTelnetStateSubRequestIAC.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>TODO: What is this?</summary>
    /// <remarks>Initializes a new instance of the ConnectionTelnetStateSubRequestIAC class.</remarks>
    /// <param name="parent">The parent telnet code handler which this object is created by.</param>
    internal class ConnectionTelnetStateSubRequestIAC(TelnetCodeHandler parent) : ConnectionTelnetState(parent)
    {
        /// <summary>Process the specified input.</summary>
        /// <param name="data">The data to be processed.</param>
        public override void ProcessInput(byte data)
        {
            switch (data)
            {
                case TelnetCommandByte.IAC:
                    // If we have another IAC then we put it into the buffer.
                    Parent.SubRequestBuffer.Add(data);
                    Parent.ChangeState(new ConnectionTelnetStateSubRequest(Parent));
                    break;
                case TelnetCommandByte.SE:
                    // This signifies the end of the sub negotiation string, so we need to process it.
                    // The first item in the buffer should be our option code.
                    byte optionCode = Parent.SubRequestBuffer[0];

                    // Remove the option code from the buffer.
                    Parent.SubRequestBuffer.RemoveAt(0);

                    // Find the related option.
                    TelnetOption option = (TelnetOption)Parent.FindOption(optionCode);

                    // If we have received an option code that we dont recognise, create a temporary
                    // telnet option to deal with it (marking that we "do not want" whatever it is).
                    option ??= new TelnetOption("temp", optionCode, false, Parent.Connection);

                    // Perform the sub negotiation.
                    option.ProcessSubNegotiation(Parent.SubRequestBuffer.ToArray());
                    Parent.SubRequestBuffer.Clear();

                    // Set our state back to normal.
                    Parent.ChangeState(new ConnectionTelnetStateText(Parent));
                    break;
            }
        }
    }
}
