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
                Console.WriteLine($"Client {telnetConnection.CurrentIPAddress} has connected.");
                telnetConnection.Send(welcomeMessage);
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
                var clients = server.AllClients;
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
