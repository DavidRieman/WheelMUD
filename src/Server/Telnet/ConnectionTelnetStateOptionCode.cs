//-----------------------------------------------------------------------------
// <copyright file="ConnectionTelnetStateOptionCode.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    using WheelMUD.Interfaces;
    
    /// <summary>@@@ What is this?</summary>
    internal class ConnectionTelnetStateOptionCode : ConnectionTelnetState
    {
        /// <summary>The option code.</summary>
        private int optionCode;

        /// <summary>Initializes a new instance of the ConnectionTelnetStateOptionCode class.</summary>
        /// <param name="parent">The parent telnet code handler which this object is created by.</param>
        /// <param name="optionCode">The option code being negotiated.</param>
        public ConnectionTelnetStateOptionCode(TelnetCodeHandler parent, int optionCode)
            : base(parent)
        {
            this.optionCode = optionCode;
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="data">The data to be processed.</param>
        public override void ProcessInput(byte data)
        {
            // If the data is not one of our implemented options then we reset back.
            TelnetOption option = (TelnetOption)Parent.TelnetOptions.Find(delegate(ITelnetOption o) { return o.OptionCode == data; });
            if (option == null)
            {
                // We have received an option code that we dont recognise, so we create a temporary
                // telnet option to deal with it.
                option = new TelnetOption("temp", data, false, Parent.Connection);
            }

            switch (this.optionCode)
            {
                case (int)TelnetResponseCode.DO:
                    option.NegotiateDo();
                    break;
                case (int)TelnetResponseCode.DONT:
                    option.NegotiateDont();
                    break;
                case (int)TelnetResponseCode.WILL:
                    option.NegotiateWill();
                    break;
                case (int)TelnetResponseCode.WONT:
                    option.NegotiateWont();
                    break;
            }

            Parent.ChangeState(new ConnectionTelnetStateText(Parent));
        }
    }
}
