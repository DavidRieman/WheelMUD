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
    using System.Linq;
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

        /// <summary>The connection telnet state.</summary>
        private ConnectionTelnetState connectionTelnetState;

        /// <summary>The synchronization locking object for this class.</summary>
        private readonly object lockObject = new object();

        /// <summary>The telnet options this handler supports.</summary>
        private readonly List<ITelnetOption> telnetOptions;

        /// <summary>Initializes a new instance of the TelnetCodeHandler class.</summary>
        /// <param name="connection">The connection upon which this telnet code handler is based.</param>
        public TelnetCodeHandler(Connection connection)
        {
            connectionTelnetState = new ConnectionTelnetStateText(this);
            Connection = connection;
            telnetOptions = new List<ITelnetOption>() {
                new TelnetOptionEcho(true, Connection),
                new TelnetOptionNaws(true, Connection),
                new TelnetOptionTermType(true, Connection),
                new TelnetOptionMXP(true, Connection),
                new TelnetOptionMCCP(true, Connection)
            };
        }

        /// <summary>Gets the connection this handler relates to.</summary>
        public Connection Connection { get; private set; }

        /// <summary>Gets the sub request buffer.</summary>
        public List<byte> SubRequestBuffer { get; } = new List<byte>();

        /// <summary>Find the TelnetOption class instance of the given type.</summary>
        /// <typeparam name="T">The ITelnetOption type to search for.</typeparam>
        /// <returns>The TelnetOption instance of the given type, or null if it can not be found.</returns>
        public T FindOption<T>() where T : ITelnetOption
        {
            lock (lockObject)
            {
                return telnetOptions.OfType<T>().FirstOrDefault();
            }
        }

        /// <summary>Find the TelnetOption class instance via the given option code.</summary>
        /// <param name="optionCode">The OptionCode to search for.</param>
        /// <returns>The TelnetOption instance, or null if it can not be found.</returns>
        public ITelnetOption FindOption(int optionCode)
        {
            lock (lockObject)
            {
                return telnetOptions.Find(o => o.OptionCode == optionCode);
            }
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

            byte[] returnData = buffer.ToArray();
            buffer.Clear();
            return returnData;
        }

        /// <summary>Begin the negotiation of telnet options.</summary>
        public void BeginNegotiation()
        {
            FindOption<TelnetOptionNaws>()?.Enable();
            FindOption<TelnetOptionTermType>()?.Enable();
            FindOption<TelnetOptionMXP>()?.Enable();
            FindOption<TelnetOptionMCCP>()?.Enable();
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
    }
}