// Copyright (c) WheelMUD Development Team.  See LICENSE.txt or https://github.com/DavidRieman/WheelMUD/#license

using System.Text;

namespace WheelMUD.Telnet.Demo
{
    internal static class DemoServer
    {
        static readonly byte[] welcomeMessage = Encoding.UTF8.GetBytes($"Welcome!\r\n");

        internal static void Run()
        {
            var server = new TelnetServer(32111);
            server.ClientBeginConnection += (ip) =>
            {
                Console.WriteLine($"Client {ip} is attempting to connect. Accepting connection.");
                return true;
            };
            server.ClientConnected += (TelnetConnection telnetConnection) =>
            {
                Console.WriteLine($"Client {telnetConnection.CurrentIPAddress} (ID {telnetConnection.ID}) has connected.");
                telnetConnection.Send(welcomeMessage);
                telnetConnection.DataReceived += (int totalReceivedBytes, byte[] data) =>
                {
                    // Copy up to totalReceivedBytes, but only easily displayed ASCII characters range for this demo.
                    StringBuilder sb = new();
                    foreach (var b in data)
                    {
                        if (b >= 32 && b <= 126)
                        {
                            sb.Append((char)b);
                        }
                        else
                        {
                            sb.Append(b.ToString());
                        }
                    }
                    if (sb.Length > 0) Console.WriteLine($"Client {telnetConnection.CurrentIPAddress} sent: {sb}");
                };
            };
            server.ClientDisconnected += (TelnetConnection telnetConnection) =>
            {
                Console.WriteLine($"Client {telnetConnection.CurrentIPAddress} (ID {telnetConnection.ID}) has disconnected.");
            };

            try
            {
                server.Start();
            }
            catch (PortInUseException)
            {
                Console.WriteLine("Port was already in use. Exiting.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exiting due to unexpected exception: {ex.Message}");
                return;
            }

            // Periodically send the current time to each active connection.
            var timer = new Timer((state) =>
            {
                var serverTimeMessage = Encoding.UTF8.GetBytes($"Server time is: {DateTime.Now}\r\n");
                var clients = server.AllActiveClients;
                foreach (var client in clients)
                {
                    client.Send(serverTimeMessage);
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(20));

            Console.WriteLine("Server is running. Recommend also running another demo instance in client mode for testing.");
            Console.WriteLine("Press Q here when done testing to exit.");
            while (Console.ReadKey().Key != ConsoleKey.Q) { }

            server.Stop();
        }
    }
}
