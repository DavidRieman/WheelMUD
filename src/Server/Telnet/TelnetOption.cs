//-----------------------------------------------------------------------------
// <copyright file="TelnetOption.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A class that represents a telnet option and is able to negotiate itself with the client.
//   Created: January 2007 by Foxedup
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Server.Telnet
{
    using WheelMUD.Interfaces;

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
            this.Name = name;
            this.OptionCode = optionCode;
            this.WantOption = wantOption;
            this.Connection = connection;
            
            // Initialize the default values for all automatic properties of this class
            // that need to be something other than zero or null.
            this.UsState = TelnetOptionState.NO;
            this.UsSubState = TelnetQueueState.EMPTY;
            this.HimState = TelnetOptionState.NO;
            this.HimSubState = TelnetQueueState.EMPTY;
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
            this.WantOption = true;
            this.UsState = TelnetOptionState.WANTYES;

            switch (this.HimState)
            {
                case TelnetOptionState.NO:
                    this.HimState = TelnetOptionState.WANTYES;
                    this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DO, (byte)this.OptionCode });
                    break;
                case TelnetOptionState.YES:
                    break;
                case TelnetOptionState.WANTNO:
                    if (this.HimSubState == TelnetQueueState.EMPTY)
                    {
                        this.HimSubState = TelnetQueueState.OPPOSITE;
                    }
                    else
                    { 
                    }
                    
                    break;
                case TelnetOptionState.WANTYES:
                    if (this.HimSubState == TelnetQueueState.EMPTY)
                    { 
                    }
                    else
                    {
                        this.HimSubState = TelnetQueueState.EMPTY;
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
            this.WantOption = false;
            this.UsState = TelnetOptionState.WANTNO;

            switch (this.HimState)
            {
                case TelnetOptionState.NO:
                case TelnetOptionState.YES:
                    this.HimState = TelnetOptionState.WANTNO;
                    this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DONT, (byte)this.OptionCode });
                    break;
                case TelnetOptionState.WANTNO:
                    if (this.HimSubState == TelnetQueueState.EMPTY)
                    {
                    }
                    else
                    {
                        this.HimSubState = TelnetQueueState.EMPTY;
                    }
                    
                    break;
                case TelnetOptionState.WANTYES:
                    if (this.HimSubState == TelnetQueueState.EMPTY)
                    {
                        this.HimSubState = TelnetQueueState.OPPOSITE;
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
            switch (this.HimState)
            {
                case TelnetOptionState.NO:
                    // If we want to enable it him = yes, send DO; else send DONT.
                    if (this.WantOption)
                    {
                        // Send DO.
                        this.HimState = TelnetOptionState.YES;
                        this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DO, (byte)this.OptionCode });
                    }
                    else
                    {
                        // Send DONT.
                        this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DONT, (byte)this.OptionCode });
                    }
                    
                    break;
                case TelnetOptionState.YES:
                    break;
                case TelnetOptionState.WANTNO:
                    if (this.HimSubState == TelnetQueueState.EMPTY)
                    {
                        this.HimState = TelnetOptionState.NO;
                    }
                    else
                    {
                        this.HimState = TelnetOptionState.YES;
                        this.HimSubState = TelnetQueueState.EMPTY;
                    }
                    
                    break;
                case TelnetOptionState.WANTYES:
                    if (this.HimSubState == TelnetQueueState.EMPTY)
                    {
                        this.HimState = TelnetOptionState.YES;
                    }
                    else
                    {
                        // Send DONT
                        this.HimState = TelnetOptionState.NO;
                        this.HimSubState = TelnetQueueState.EMPTY;
                        this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DONT, (byte)this.OptionCode });
                    }
                    
                    break;
            }

            if (this.HimState == TelnetOptionState.YES)
            {
                // Successful negotiation.
                this.UsState = TelnetOptionState.YES;
                this.AfterNegotiation();
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
            switch (this.HimState)
            {
                case TelnetOptionState.NO:
                    break;
                case TelnetOptionState.YES:
                    // SEND DONT
                    this.HimState = TelnetOptionState.NO;
                    this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DONT, (byte)this.OptionCode });
                    break;
                case TelnetOptionState.WANTNO:
                    if (this.HimSubState == TelnetQueueState.EMPTY)
                    {
                        this.HimState = TelnetOptionState.NO;
                    }
                    else
                    {
                        // SEND DO
                        this.HimState = TelnetOptionState.WANTYES;
                        this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.DO, (byte)this.OptionCode });
                    }
                    
                    break;
                case TelnetOptionState.WANTYES:
                    if (this.HimSubState == TelnetQueueState.EMPTY)
                    {
                        this.HimState = TelnetOptionState.NO;
                    }
                    else
                    {
                        this.HimState = TelnetOptionState.NO;
                        this.HimSubState = TelnetQueueState.EMPTY;
                    }
                    
                    break;
            }

            if (this.HimState == TelnetOptionState.NO)
            {
                this.UsState = TelnetOptionState.NO;
                this.AfterNegotiation();
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
            switch (this.UsState)
            {
                case TelnetOptionState.NO:
                    if (this.WantOption)
                    {
                        // SEND DO
                        this.UsState = TelnetOptionState.YES;
                        this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WILL, (byte)this.OptionCode });
                    }
                    else
                    {
                        // SEND DONT
                        this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WONT, (byte)this.OptionCode });
                    }
                    
                    break;
                case TelnetOptionState.YES:
                    break;
                case TelnetOptionState.WANTNO:
                    if (this.UsSubState == TelnetQueueState.EMPTY)
                    {
                        this.UsState = TelnetOptionState.NO;
                    }
                    else
                    {
                        this.UsState = TelnetOptionState.YES;
                        this.UsSubState = TelnetQueueState.EMPTY;
                    }
                    
                    break;
                case TelnetOptionState.WANTYES:
                    if (this.UsSubState == TelnetQueueState.EMPTY)
                    {
                        this.UsState = TelnetOptionState.YES;
                    }
                    else
                    {
                        // SEND DONT
                        this.UsState = TelnetOptionState.WANTNO;
                        this.UsSubState = TelnetQueueState.EMPTY;
                        Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WONT, (byte)this.OptionCode });
                    }
                    
                    break;
            }
            
            if (this.UsState == TelnetOptionState.YES)
            {
                this.HimState = TelnetOptionState.YES;
                this.AfterNegotiation();
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
            switch (this.UsState)
            {
                case TelnetOptionState.NO:
                    break;
                case TelnetOptionState.YES:
                    // SEND WONT
                    this.UsState = TelnetOptionState.NO;
                    this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WONT, (byte)this.OptionCode });
                    break;
                case TelnetOptionState.WANTNO:
                    if (this.UsSubState == TelnetQueueState.EMPTY)
                    {
                        this.UsState = TelnetOptionState.NO;
                    }
                    else
                    {
                        // SEND DO
                        this.UsState = TelnetOptionState.WANTYES;
                        this.UsSubState = TelnetQueueState.EMPTY;
                        this.Connection.Send(new byte[] { 255, (byte)TelnetResponseCode.WILL, (byte)this.OptionCode });
                    }
                    
                    break;
                case TelnetOptionState.WANTYES:
                    if (this.UsSubState == TelnetQueueState.EMPTY)
                    {
                        this.UsState = TelnetOptionState.NO;
                    }
                    else
                    {
                        this.UsState = TelnetOptionState.NO;
                        this.UsSubState = TelnetQueueState.EMPTY;
                    }
                    
                    break;
            }

            if (this.UsState == TelnetOptionState.NO)
            {
                this.HimState = TelnetOptionState.NO;
                this.AfterNegotiation();
            }
        }
    }
}