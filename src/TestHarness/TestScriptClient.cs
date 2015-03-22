//-----------------------------------------------------------------------------
// <copyright file="TestScriptClient.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Scriptable client for full integration testing.
// </summary>
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestHarness
{
    /// <summary>Scriptable client for full integration testing.</summary>
    public class TestScriptClient
    {
        private TcpClient _client;
        private NetworkStream _netstr;
        private MultiUpdater _display;

        /// <summary>Gets a value indicating whether the client is currently connected.</summary>
        public bool Connected
        {
            get
            {
                if (_client == null)
                {
                    return false;
                }

                return _client.Connected;
            }
        }

        /// <summary>Instructs the client to connect to the server.</summary>
        /// <param name="display">Where to capture display output (such as console and text logs).</param>
        /// <returns>True if successfully connected.</returns>
        public bool Connect(MultiUpdater display)
        {
            try
            {
                _client = new TcpClient();

                // Use default mud port. TODO: Read and use configured port?
                _client.Connect(new IPEndPoint(IPAddress.Loopback, 4000));

                int attempts = 0;
                while (!_client.Connected && attempts++ < 10)
                {
                    display.Notify("> Connecting to mud server on localhost port 4000...");
                    Thread.Sleep(1000);
                }

                _display = display;
                _netstr = _client.GetStream();

                return true;
            }
            catch (Exception ex)
            {
                _display.Notify("> Fatal Error: " + ex);
                _client = null;
                return false;
            }
        }

        /// <summary>Sends the specified data, from the client, to the server.</summary>
        /// <param name="data">The data to send.</param>
        /// <returns>True if the data was successfully sent.</returns>
        public bool Send(string data)
        {
            byte[] buf = Encoding.UTF8.GetBytes(string.Format("{0}{1}", data, Environment.NewLine));

            try
            {
                _netstr.Write(buf, 0, buf.Length);
            }
            catch (Exception ex)
            {
                _display.Notify(">> ERROR: " + ex);
                return false;
            }

            return true;
        }

        /// <summary>Receive data from the server.</summary>
        /// <param name="data">Where to receive the data.</param>
        /// <returns>True if data read was successful.</returns>
        public bool Recieve(out string data)
        {
            data = null;

            try
            {
                var buf = new byte[1024];

                _netstr.Read(buf, 0, buf.Length);

                data = Encoding.ASCII.GetString(buf);
            }
            catch (Exception ex)
            {
                _display.Notify(">> FATAL Error: " + ex);
                return false;
            }

            return true;
        }

        /// <summary>Disconnects the client from the server.</summary>
        public void Disconnect()
        {
            if (Connected)
            {
                Send("quit");
                _netstr.Close();
                _netstr = null;
                _client = null;
            }
        }
    }
}