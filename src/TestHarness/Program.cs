//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Main entry point for the application.
//   Created: August 2006 by Foxedup.
//   Modified: June 13, 2010 by Feverdream.
// </summary>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TestHarness.Commands;
using WheelMUD.Main;

namespace TestHarness
{
    /// <summary>
    /// The test harness program; runs the MUD as a console application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point into the test harness
        /// </summary>
        public static void Main()
        {
            string logFileName = "Log_" + DateTime.Now.ToShortDateString() + ".txt";
            logFileName = logFileName.Replace('\\', '_').Replace('/', '_');
            var consoleDisplay = new ConsoleUpdater();
            var textLogWriter = new TextLogUpdater(logFileName);
            var display = new MultiUpdater(consoleDisplay, textLogWriter);

            var app = Application.Instance;

            app.SubscribeToSystem(display);

            app.Start();

            ITestHarnessCommand[] commandObjects = { new HelpCommand(), new UpdateActionsCommand(), new RunTestsCommand() };
            IDictionary<string, ITestHarnessCommand> commands = new ConcurrentDictionary<string, ITestHarnessCommand>();

            foreach (var cmdObj in commandObjects)
            {
                foreach (var name in cmdObj.Names)
                {
                    commands[name] = cmdObj;
                }
            }

            var input = string.Empty;

            while (input.ToUpper() != "SHUTDOWN")
            {
                input = Console.ReadLine();
                if (input != null)
                {
                    string[] words = input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (words.Length == 0)
                    {
                        continue;
                    }

                    var cmd = words[0];
                    if (commands.ContainsKey(cmd))
                    {
                        commands[cmd].Execute(app, display, words);
                    }
                    else
                    {
                        display.Notify(string.Format("> Command Not Recognized. [{0}]", string.Join(" ", words)));
                    }

                }

                // This is for Mono compatability.
                input = string.Empty;
            }

            app.Stop();

            display.Notify("Press enter to quit...");
            Console.ReadLine();
        }

        /// <summary>
        /// Notifies the user of a message.
        /// </summary>
        /// <param name="message">The message to be notified.</param>
        public void Notify(string message)
        {
            Console.WriteLine(message);
        }

        public class TestScriptClient
        {
            TcpClient _client;
            NetworkStream _netstr;
            MultiUpdater _display;

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

            public bool Connect(MultiUpdater display)
            {
                try
                {
                    _client = new TcpClient();

                    // Use default mud port.
                    _client.Connect(new IPEndPoint(IPAddress.Loopback, 4000));

                    int attempts = 0;
                    while (!_client.Connected && attempts++ < 10)
                    {
                        display.Notify("> Connecting to mud server on localhost port 4000..");
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
}
