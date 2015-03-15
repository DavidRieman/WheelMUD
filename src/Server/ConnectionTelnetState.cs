using System;
using System.Collections.Generic;
using System.Text;

namespace WheelMUD.Server
{

    /// <summary>
    /// Class processes all incoming data for any IAC options
    /// </summary>
    internal class TelnetCodeHandler
    {
        private ConnectionTelnetState _state;
        private List<byte> _buffer = new List<byte>();
        private List<byte> _subRequestBuffer = new List<byte>();
        
        /// <summary>
        /// We pass each byte that comes in to our state classes for processing
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] ProcessInput(byte[] data)
        {
            foreach (byte bit in data)
            {
                _state.ProcessInput(bit);
            }
            return data;
        }

        /// <summary>
        /// Allows our state classes to change the state being used
        /// </summary>
        /// <param name="newState">The state to change to</param>
        internal void ChangeState(ConnectionTelnetState newState)
        {
            _state = newState;
        }

        /// <summary>
        /// Allows our state classes to add data to the sub request buffer for processing
        /// </summary>
        /// <param name="data"></param>
        internal void AddToSubRequestBuffer(byte data)
        {
            _subRequestBuffer.Add(data);
        }

        public TelnetCodeHandler()
        {
            _state = new ConnectionTellnetStateText(this);
        }

    }

    internal abstract class ConnectionTelnetState
    {
        internal const int IAC = 255;
        internal const int SB = 250;
        internal const int SE = 240;
        internal const int WILL = 251;
        internal const int WONT = 252;
        internal const int DO = 253;
        internal const int DONT = 254;

        internal TelnetCodeHandler _parent;

        public abstract byte ProcessInput(byte data);

        public ConnectionTelnetState(TelnetCodeHandler parent)
        {
            _parent = parent;
        }
    }


    /// <summary>
    /// Handles the incoming data over our connection.
    /// This is the default state for our connections data stream
    /// </summary>
    internal class ConnectionTellnetStateText : ConnectionTelnetState
    {
        public ConnectionTellnetStateText(TelnetCodeHandler parent) : base(parent)
        {
        }

        public override byte ProcessInput(byte data)
        {
            //all we do here is iterate over the data adding data to our buffer
            //if we see an IAC (255) byte then we change our state to IAC and process.
            byte returnByte = new byte();
            if (data == IAC)
            {
                _parent.ChangeState(new ConnectionTelnetStateIAC(_parent));
            }
            else
            {
                returnByte = data;
            }
            return returnByte;
        }
    }


    /// <summary>
    /// Handles the negotiation of a telnet IAC option
    /// </summary>
    internal class ConnectionTelnetStateIAC : ConnectionTelnetState
    {
        public ConnectionTelnetStateIAC(TelnetCodeHandler parent) : base(parent)
        {
        }

        public override byte ProcessInput(byte data)
        {
            byte returnByte = new byte();
            switch (data)
            {
                case SB:
                    //We throw away the data and enter the subrequest state
                    _parent.ChangeState(new ConnectionTelnetStateSubRequest(_parent));
                    break;
                case WILL:

                    break;
                case WONT:

                    break;
                case DO:

                    break;
                case DONT:

                    break;
                case IAC:
                    //if its another IAC then it has been correctly escaped so we should pass it back
                    returnByte = data;
                    //change our state back to Text
                    _parent.ChangeState(new ConnectionTellnetStateText(_parent));
                    break;
                default:
                    //if we dont recognise the code that came in it must be dodgy, so throw it away
                    //change our state back to Text
                    _parent.ChangeState(new ConnectionTellnetStateText(_parent));
                    break;
            }
            return returnByte;
        }
    }

    internal class ConnectionTelnetStateSubRequest : ConnectionTelnetState
    {

        public ConnectionTelnetStateSubRequest(TelnetCodeHandler parent) : base(parent)
        {
        }

        public override byte ProcessInput(byte data)
        {
            switch (data)
            {
                case IAC:
                    //when we see an IAC byte in this state we change state to SubRequestIAC
                    _parent.ChangeState(new ConnectionTelnetStateSubRequestIAC(_parent));
                    break;
                default:
                    //add the data to the buffer.
                    _parent.AddToSubRequestBuffer(data);
                    break;
            }
            return new byte();
        }
    }

    internal class ConnectionTelnetStateSubRequestIAC : ConnectionTelnetState
    {
        public ConnectionTelnetStateSubRequestIAC(TelnetCodeHandler parent) : base(parent)
        {
        }

        public override byte ProcessInput(byte data)
        {
            switch (data)
            {
                case IAC:
                    _parent.AddToSubRequestBuffer(data);
                    break;
                case SE:
                    //this signifies the end of the sub negotiation string, so we need to process it.

                    _parent.ChangeState(new ConnectionTellnetStateText(_parent));
                    break;
            }
            return new byte();
        }
    }
}