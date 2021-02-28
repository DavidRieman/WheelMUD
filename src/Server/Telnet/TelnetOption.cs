//-----------------------------------------------------------------------------
// <copyright file="TelnetOption.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server.Interfaces;

namespace WheelMUD.Server.Telnet
{
    /// <summary>A class that represents a telnet option and is able to negotiate itself with the client.</summary>
    internal class TelnetOption : ITelnetOption
    {
        /// <summary>Initializes a new instance of the TelnetOption class.</summary>
        /// <param name="name">The name of this telnet option.</param>
        /// <param name="optionCode">The code that represents this telnet option.</param>
        /// <param name="wantOption">Whether this telnet option is wanted or not.</param>
        /// <param name="connection">The connection upon which we are negotiating.</param>
        public TelnetOption(string name, int optionCode, bool wantOption, Connection connection)
        {
            Name = name;
            OptionCode = optionCode;
            WantOption = wantOption;
            Connection = connection;

            // Initialize the default values for all automatic properties of this class
            // that need to be something other than zero or null.
            UsState = TelnetOptionState.NO;
            UsSubState = TelnetQueueState.EMPTY;
            HimState = TelnetOptionState.NO;
            HimSubState = TelnetQueueState.EMPTY;
        }

        /// <summary>The available telnet option states.</summary>
        public enum TelnetOptionState
        {
            /// <summary>The 'no' telnet option state.</summary>
            NO,

            /// <summary>The 'want no' telnet option state.</summary>
            WANTNO,

            /// <summary>The 'yes' telnet option state.</summary>
            YES,

            /// <summary>The 'want yes' telnet option state.</summary>
            WANTYES
        }

        /// <summary>The available telnet option queue states.</summary>
        public enum TelnetQueueState
        {
            /// <summary>The 'empty' telnet option queue state.</summary>
            EMPTY,

            /// <summary>The 'opposite' telnet option queue state.</summary>
            OPPOSITE
        }

        /// <summary>Gets a value indicating whether the client wants the option.</summary>
        public bool WantOption { get; private set; }

        /// <summary>Gets our current telnet option state.</summary>
        public TelnetOptionState UsState { get; private set; }

        /// <summary>Gets our current telnet option sub state.</summary>
        public TelnetQueueState UsSubState { get; private set; }

        /// <summary>Gets their current telnet option state.</summary>
        public TelnetOptionState HimState { get; private set; }

        /// <summary>Gets their current telnet option sub state.</summary>
        public TelnetQueueState HimSubState { get; private set; }

        /// <summary>Gets the option code for this telnet option.</summary>
        public int OptionCode { get; private set; }

        /// <summary>Gets the name of this telnet option.</summary>
        public string Name { get; private set; }

        /// <summary>Gets or sets the connection upon which we are negotiating.</summary>
        protected Connection Connection { get; set; }

        /// <summary>Called to process the sub negotiation via the specified data.</summary>
        /// <param name="data">The data to be processed.</param>
        public virtual void ProcessSubNegotiation(byte[] data)
        {
        }

        /// <summary>Called after the negotiation is finished.</summary>
        public virtual void AfterNegotiation()
        {
        }

        /// <summary>Attempt to negotiate the enabling of the telnet option.</summary>
        public void Enable()
        {
            // RFC States
            // 
            // NO            him=WANTYES, send WILL.
            // YES           Error: Already enabled.
            // WANTNO  EMPTY If we are queueing requests, himq=OPPOSITE;
            //               otherwise, Error: Cannot initiate new request in the middle of negotiation.
            //         OPPOSITE Error: Already queued an enable request.
            // WANTYES EMPTY Error: Already negotiating for enable.
            //         OPPOSITE himq=EMPTY.
            WantOption = true;
            UsState = TelnetOptionState.WANTYES;

            switch (HimState)
            {
                case TelnetOptionState.NO:
                    HimState = TelnetOptionState.WANTYES;
                    Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DO, (byte)OptionCode });
                    break;
                case TelnetOptionState.YES:
                    break;
                case TelnetOptionState.WANTNO:
                    if (HimSubState == TelnetQueueState.EMPTY)
                    {
                        HimSubState = TelnetQueueState.OPPOSITE;
                    }
                    else
                    {
                    }

                    break;
                case TelnetOptionState.WANTYES:
                    if (HimSubState == TelnetQueueState.EMPTY)
                    {
                    }
                    else
                    {
                        HimSubState = TelnetQueueState.EMPTY;
                    }

                    break;
            }
        }

        /// <summary>Attempt to negotiate the disabling of the telnet option.</summary>
        public void Disable()
        {
            // RFC States
            // 
            // NO            Error: Already disabled.
            // YES           him=WANTNO, send DONT.
            // WANTNO  EMPTY Error: Already negotiating for disable.
            //         OPPOSITE himq=EMPTY.
            // WANTYES EMPTY If we are queueing requests, himq=OPPOSITE;
            //               otherwise, Error: Cannot initiate new request in the middle of negotiation.
            //         OPPOSITE Error: Already queued a disable request.
            WantOption = false;
            UsState = TelnetOptionState.WANTNO;

            switch (HimState)
            {
                case TelnetOptionState.NO:
                case TelnetOptionState.YES:
                    HimState = TelnetOptionState.WANTNO;
                    Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DONT, (byte)OptionCode });
                    break;
                case TelnetOptionState.WANTNO:
                    if (HimSubState == TelnetQueueState.EMPTY)
                    {
                    }
                    else
                    {
                        HimSubState = TelnetQueueState.EMPTY;
                    }

                    break;
                case TelnetOptionState.WANTYES:
                    if (HimSubState == TelnetQueueState.EMPTY)
                    {
                        HimSubState = TelnetQueueState.OPPOSITE;
                    }
                    else
                    {
                    }

                    break;
            }
        }

        /// <summary>Negotiates a WILL response for an option.</summary>
        internal void NegotiateWill()
        {
            // RFC States
            // 
            // Upon receipt of WILL, we choose based upon him and himq:
            // NO      If we agree that he should enable, him=YES, send DO; otherwise, send DONT.
            // YES     Ignore.
            // WANTNO  EMPTY Error: DONT answered by WILL. him=NO.
            //         OPPOSITE Error: DONT answered by WILL. him=YES, himq=EMPTY.
            // WANTYES EMPTY him=YES.
            //         OPPOSITE him=WANTNO, himq=EMPTY, send DONT.
            switch (HimState)
            {
                case TelnetOptionState.NO:
                    // If we want to enable it him = yes, send DO; else send DONT.
                    if (WantOption)
                    {
                        // Send DO.
                        HimState = TelnetOptionState.YES;
                        Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DO, (byte)OptionCode });
                    }
                    else
                    {
                        // Send DONT.
                        Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DONT, (byte)OptionCode });
                    }

                    break;
                case TelnetOptionState.YES:
                    break;
                case TelnetOptionState.WANTNO:
                    if (HimSubState == TelnetQueueState.EMPTY)
                    {
                        HimState = TelnetOptionState.NO;
                    }
                    else
                    {
                        HimState = TelnetOptionState.YES;
                        HimSubState = TelnetQueueState.EMPTY;
                    }

                    break;
                case TelnetOptionState.WANTYES:
                    if (HimSubState == TelnetQueueState.EMPTY)
                    {
                        HimState = TelnetOptionState.YES;
                    }
                    else
                    {
                        // Send DONT
                        HimState = TelnetOptionState.NO;
                        HimSubState = TelnetQueueState.EMPTY;
                        Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DONT, (byte)OptionCode });
                    }

                    break;
            }

            if (HimState == TelnetOptionState.YES)
            {
                // Successful negotiation.
                UsState = TelnetOptionState.YES;
                AfterNegotiation();
            }
        }

        /// <summary>Negotiates a WONT response for an option.</summary>
        internal void NegotiateWont()
        {
            // RFC States
            // 
            // Upon receipt of WONT, we choose based upon him and himq:
            // NO            Ignore.
            // YES           him=NO, send DONT.
            // WANTNO  EMPTY him=NO.
            //         OPPOSITE him=WANTYES, himq=NONE, send DO.
            // WANTYES EMPTY him=NO.*
            //         OPPOSITE him=NO, himq=NONE.**
            switch (HimState)
            {
                case TelnetOptionState.NO:
                    break;
                case TelnetOptionState.YES:
                    // SEND DONT
                    HimState = TelnetOptionState.NO;
                    Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DONT, (byte)OptionCode });
                    break;
                case TelnetOptionState.WANTNO:
                    if (HimSubState == TelnetQueueState.EMPTY)
                    {
                        HimState = TelnetOptionState.NO;
                    }
                    else
                    {
                        // SEND DO
                        HimState = TelnetOptionState.WANTYES;
                        Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DO, (byte)OptionCode });
                    }

                    break;
                case TelnetOptionState.WANTYES:
                    if (HimSubState == TelnetQueueState.EMPTY)
                    {
                        HimState = TelnetOptionState.NO;
                    }
                    else
                    {
                        HimState = TelnetOptionState.NO;
                        HimSubState = TelnetQueueState.EMPTY;
                    }

                    break;
            }

            if (HimState == TelnetOptionState.NO)
            {
                UsState = TelnetOptionState.NO;
                AfterNegotiation();
            }
        }

        /// <summary>Negotiates a DO response for an option.</summary>
        internal void NegotiateDo()
        {
            // RFC States
            // 
            // Upon receipt of DO, we choose based upon us and usq:
            // NO      If we agree that he should enable, us=YES, send DO; otherwise, send DONT.
            // YES     Ignore.
            // WANTNO  EMPTY Error: DONT answered by WILL. us=NO.
            //         OPPOSITE Error: DONT answered by WILL. us=YES, usq=EMPTY.
            // WANTYES EMPTY us=YES.
            //         OPPOSITE us=WANTNO, usq=EMPTY, send DONT.
            switch (UsState)
            {
                case TelnetOptionState.NO:
                    if (WantOption)
                    {
                        // SEND DO
                        UsState = TelnetOptionState.YES;
                        Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WILL, (byte)OptionCode });
                    }
                    else
                    {
                        // SEND DONT
                        Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WONT, (byte)OptionCode });
                    }

                    break;
                case TelnetOptionState.YES:
                    break;
                case TelnetOptionState.WANTNO:
                    if (UsSubState == TelnetQueueState.EMPTY)
                    {
                        UsState = TelnetOptionState.NO;
                    }
                    else
                    {
                        UsState = TelnetOptionState.YES;
                        UsSubState = TelnetQueueState.EMPTY;
                    }

                    break;
                case TelnetOptionState.WANTYES:
                    if (UsSubState == TelnetQueueState.EMPTY)
                    {
                        UsState = TelnetOptionState.YES;
                    }
                    else
                    {
                        // SEND DONT
                        UsState = TelnetOptionState.WANTNO;
                        UsSubState = TelnetQueueState.EMPTY;
                        Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WONT, (byte)OptionCode });
                    }

                    break;
            }

            if (UsState == TelnetOptionState.YES)
            {
                HimState = TelnetOptionState.YES;
                AfterNegotiation();
            }
        }

        /// <summary>Negotiates a DONT response for an option.</summary>
        internal void NegotiateDont()
        {
            // RFC States
            // 
            // Upon receipt of DONT, we choose based upon us and usq:
            // NO            If Ignore.
            // YES           us=NO, send WONT.
            // WANTNO  EMPTY us=NO.
            //         OPPOSITE us=WANTYES, usq=NONE, send DO.
            // WANTYES EMPTY us=NO.*
            //         OPPOSITE us=NO, usq=NONE.**
            switch (UsState)
            {
                case TelnetOptionState.NO:
                    break;
                case TelnetOptionState.YES:
                    // SEND WONT
                    UsState = TelnetOptionState.NO;
                    Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WONT, (byte)OptionCode });
                    break;
                case TelnetOptionState.WANTNO:
                    if (UsSubState == TelnetQueueState.EMPTY)
                    {
                        UsState = TelnetOptionState.NO;
                    }
                    else
                    {
                        // SEND DO
                        UsState = TelnetOptionState.WANTYES;
                        UsSubState = TelnetQueueState.EMPTY;
                        Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WILL, (byte)OptionCode });
                    }

                    break;
                case TelnetOptionState.WANTYES:
                    if (UsSubState == TelnetQueueState.EMPTY)
                    {
                        UsState = TelnetOptionState.NO;
                    }
                    else
                    {
                        UsState = TelnetOptionState.NO;
                        UsSubState = TelnetQueueState.EMPTY;
                    }

                    break;
            }

            if (UsState == TelnetOptionState.NO)
            {
                HimState = TelnetOptionState.NO;
                AfterNegotiation();
            }
        }
    }
}