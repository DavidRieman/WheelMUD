//-----------------------------------------------------------------------------
// <copyright file="ConnectionTelnetStateOptionCode.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    /// <summary>TODO: What is this?</summary>
    /// <remarks>Initializes a new instance of the ConnectionTelnetStateOptionCode class.</remarks>
    /// <param name="parent">The parent telnet code handler which this object is created by.</param>
    /// <param name="optionCode">The option code being negotiated.</param>
    internal class ConnectionTelnetStateOptionCode(TelnetCodeHandler parent, int optionCode) : ConnectionTelnetState(parent)
    {
        /// <summary>The option code.</summary>
        private readonly int optionCode = optionCode;

        /// <summary>Process the specified input.</summary>
        /// <param name="data">The data to be processed.</param>
        public override void ProcessInput(byte data)
        {
            // If the data is not one of our implemented options then we reset back.
            TelnetOption option = (TelnetOption)Parent.FindOption(data);

            // If we have received an option code that we do not recognise, create a temporary
            // telnet option to deal with it (marking that we "do not want" whatever it is).
            option ??= new TelnetOption("temp", data, false, Parent.Connection);

            switch (optionCode)
            {
                case TelnetCommandByte.DO:
                    option.NegotiateDo();
                    break;
                case TelnetCommandByte.DONT:
                    option.NegotiateDont();
                    break;
                case TelnetCommandByte.WILL:
                    option.NegotiateWill();
                    break;
                case TelnetCommandByte.WONT:
                    option.NegotiateWont();
                    break;
            }

            Parent.ChangeState(new ConnectionTelnetStateText(Parent));
        }
    }
}
