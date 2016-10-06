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
        private TcpClient client;
        private NetworkStream netStream;
        private MultiUpdater display;

        /// <summary>Gets a value indicating whether the client is currently connected.</summary>
        public bool Connected
        {
            get
            {
                if (client == null)
                {
                    return false;
                }

                return client.Connected;
            }
        }

        /// <summary>Instructs the client to connect to the server.</summary>
        /// <param name="display">Where to capture display output (such as console and text logs).</param>
        /// <returns>True if successfully connected.</returns>
        public bool Connect(MultiUpdater display)
        {
            try
            {
                client = new TcpClient();

                // Use default mud port. TODO: Read and use configured port?
                client.Connect(new IPEndPoint(IPAddress.Loopback, 4000));

                int attempts = 0;
                while (!client.Connected && attempts++ < 10)
                {
                    display.Notify("> Connecting to mud server on localhost port 4000...");
                    Thread.Sleep(1000);
                }

                this.display = display;
                netStream = client.GetStream();

                return true;
            }
            catch (Exception ex)
            {
                this.display.Notify("> Fatal Error: " + ex);
                client = null;
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
                netStream.Write(buf, 0, buf.Length);
            }
            catch (Exception ex)
            {
                display.Notify(">> ERROR: " + ex);
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

                netStream.Read(buf, 0, buf.Length);

                data = Encoding.ASCII.GetString(buf);
            }
            catch (Exception ex)
            {
                display.Notify(">> FATAL Error: " + ex);
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
                netStream.Close();
                netStream = null;
                client = null;
            }
        }
    }
}