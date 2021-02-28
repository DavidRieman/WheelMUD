//-----------------------------------------------------------------------------
// <copyright file="TelnetServer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server.Interfaces;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Server
{
    using System.Collections.Generic;

    /// <summary>A telnet server.</summary>
    /// <remarks>This class decorates the base server, providing telnet facilities to the application.</remarks>
    public class TelnetServer : ISubSystem
    {
        /// <summary>The subscribed sub system host.</summary>
        private ISubSystemHost subSystemHost;

        /// <summary>Start the telnet server.</summary>
        public void Start()
        {
        }

        /// <summary>Stop the telnet server.</summary>
        public void Stop()
        {
        }

        /// <summary>Subscribe to receive system updates from this system.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        public void SubscribeToSystem(ISubSystemHost sender)
        {
            subSystemHost = sender;
        }

        /// <summary>Inform subscribed system(s) of the specified update.</summary>
        /// <param name="msg">The message to be sent to subscribed system(s).</param>
        public void InformSubscribedSystem(string msg)
        {
            subSystemHost.UpdateSubSystemHost(this, msg);
        }

        /// <summary>Handles received data: Processes telnet escape codes and non-alpha characters.</summary>
        /// <param name="sender">The connection which sent the data.</param>
        /// <param name="data">The data that was sent.</param>
        /// <returns>The altered data, after removing any non printable characters.</returns>
        public byte[] OnDataReceived(IConnection sender, byte[] data)
        {
            byte[] returnData = sender.TelnetCodeHandler.ProcessInput(data);
            returnData = MXPHandler.ParseIncomingData(sender, returnData);
            return HandleNonPrintables(sender, returnData);
        }

        /// <summary>Handles incoming data and removes any non printable characters.</summary>
        /// <remarks>It also deals with backspaces on char by char connections.</remarks>
        /// <param name="sender">The connection which sent the data.</param>
        /// <param name="data">The data that was sent.</param>
        /// <returns>The altered data, after removing any non printable characters.</returns>
        private static byte[] HandleNonPrintables(IConnection sender, byte[] data)
        {
            List<byte> buffer = new List<byte>();
            foreach (byte bit in data)
            {
                if (bit == 10 || bit == 13 || (bit > 31 && bit < 127))
                {
                    buffer.Add(bit);
                }
                else if (bit == 8 || bit == 127)
                {
                    // Special case for a backspace when using char by char telnet
                    // we need to remove the last character in our buffer
                    if (sender.Buffer.Length > 0)
                    {
                        sender.Buffer.Remove(sender.Buffer.Length - 1, 1);
                    }
                }
            }

            return buffer.ToArray();
        }
    }
}