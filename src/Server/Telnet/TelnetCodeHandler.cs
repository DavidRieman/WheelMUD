//-----------------------------------------------------------------------------
// <copyright file="TelnetCodeHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server.Interfaces;

namespace WheelMUD.Server.Telnet
{
    using System.Collections.Generic;
    using WheelMUD.Interfaces;

    /// <summary>The possible response codes.</summary>
    internal enum TelnetResponseCode
    {
        /// <summary>The "will" telnet response code.</summary>
        WILL = 251,

        /// <summary>The "won't" telnet response code.</summary>
        WONT = 252,

        /// <summary>The "do" telnet response code.</summary>
        DO = 253,

        /// <summary>The "don't" telnet response code.</summary>
        DONT = 254
    }

    /// <summary>The telnet code handler class.</summary>
    /// <remarks>Handles the delegation of processing telnet option codes</remarks>
    public class TelnetCodeHandler : ITelnetCodeHandler
    {
        /// <summary>The buffer.</summary>
        private readonly List<byte> buffer = new List<byte>();

        /// <summary>The sub request buffer.</summary>
        private readonly List<byte> subRequestBuffer = new List<byte>();

        /// <summary>A list of telnet options negotiated for this connection.</summary>
        private readonly List<ITelnetOption> telnetOptions = new List<ITelnetOption>();

        /// <summary>The connection upon which this telnet code handler operates.</summary>
        private readonly Connection connection;

        /// <summary>The synchronization locking object for this class.</summary>
        private readonly object lockObject = new object();

        /// <summary>The connection telnet state.</summary>
        private ConnectionTelnetState connectionTelnetState;

        /// <summary>Initializes a new instance of the TelnetCodeHandler class.</summary>
        /// <param name="connection">The connection upon which this telnet code handler is based.</param>
        public TelnetCodeHandler(Connection connection)
        {
            connectionTelnetState = new ConnectionTelnetStateText(this);
            this.connection = connection;
            SetupRequiredOptions();
        }

        /// <summary>Gets the telnet options this handler supports.</summary>
        public List<ITelnetOption> TelnetOptions
        {
            get { return telnetOptions; }
        }

        /// <summary>Gets the connection this handler relates to.</summary>
        public Connection Connection
        {
            get { return connection; }
        }

        /// <summary>Gets the sub request buffer.</summary>
        public List<byte> SubRequestBuffer
        {
            get { return subRequestBuffer; }
        }

        /// <summary>We pass each byte that comes in to our state classes for processing.</summary>
        /// <param name="data">The input data to be processed.</param>
        /// <returns>The buffer, after clearing it.</returns>
        public byte[] ProcessInput(byte[] data)
        {
            foreach (byte bit in data)
            {
                connectionTelnetState.ProcessInput(bit);
            }

            byte[] rtnData = buffer.ToArray();
            buffer.Clear();
            return rtnData;
        }

        /// <summary>Begin the negotiation of telnet options.</summary>
        public void BeginNegotiation()
        {
            lock (lockObject)
            {
                ITelnetOption naws = telnetOptions.Find(delegate (ITelnetOption o) { return o.Name.Equals("naws"); });
                if (naws != null)
                {
                    naws.Enable();
                }

                ITelnetOption termType = telnetOptions.Find(delegate (ITelnetOption o) { return o.Name.Equals("termtype"); });
                if (termType != null)
                {
                    termType.Enable();
                }

                ITelnetOption mxp = telnetOptions.Find(delegate (ITelnetOption o) { return o.Name.Equals("mxp"); });
                if (mxp != null)
                {
                    mxp.Enable();
                }

                ITelnetOption mccp = telnetOptions.Find(delegate (ITelnetOption o) { return o.Name.Equals("compress2"); });
                if (mccp != null)
                {
                    mccp.Enable();
                }
            }
        }

        /// <summary>Allows our state classes to change the state being used.</summary>
        /// <param name="newState">The state to change to</param>
        internal void ChangeState(ConnectionTelnetState newState)
        {
            connectionTelnetState = newState;
        }

        /// <summary>Adds a byte to the data buffer.</summary>
        /// <param name="data">The data to be added to the buffer.</param>
        internal void AddToBuffer(byte data)
        {
            lock (lockObject)
            {
                buffer.Add(data);
            }
        }

        /// <summary>Sets up the options we want to negotiate.</summary>
        private void SetupRequiredOptions()
        {
            lock (lockObject)
            {
                telnetOptions.Add(new TelnetOptionEcho(false, Connection));
                telnetOptions.Add(new TelnetOptionNaws(true, Connection));
                telnetOptions.Add(new TelnetOptionTermType(true, Connection));
                telnetOptions.Add(new TelnetOptionMXP(true, Connection));
                telnetOptions.Add(new TelnetOptionMCCP(true, Connection));
            }
        }
    }
}