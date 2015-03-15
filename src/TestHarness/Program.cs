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

namespace TestHarness
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using WheelMUD.Core;
    using WheelMUD.Interfaces;
    using WheelMUD.Main;

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
            var display = new MultiUpdater(new ISuperSystemSubscriber[] { consoleDisplay, textLogWriter });

            var app = Application.Instance;

            app.SubscribeToSystem(display);

            app.Start();

            var input = string.Empty;

            if (input != null)
            {
                while (input.ToUpper() != "SHUTDOWN")
                {
                    input = Console.ReadLine();
                    if (input != null)
                    {
                        if (input.StartsWith("?") || input.ToUpper().StartsWith("HELP"))
                        {
                            HandleHelpCommand(app, consoleDisplay, input);
                        }
                        else if (input.ToUpper().StartsWith("UPDATE-ACTIONS"))
                        {
                            // @@@ TODO: Test, migrate to file system watcher (at the Application layer) instead?
                            CommandManager.Instance.Recompose();
                        }
                        else if (input.ToUpper().StartsWith("RUN-TESTS"))
                        {
                            if (!Directory.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Tests"))
                            {
                                display.Notify("> FATAL ERROR: Tests Directory does not exist.");
                                continue;
                            }

                            var failed = new List<string>();
                            string[] files = Directory.GetFiles(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Tests", "*.testscript", SearchOption.AllDirectories);

                            foreach (string file in files)
                            {
                                string[] lines = File.ReadAllLines(file);
                                var tsc = new TestScriptClient();

                                if (!tsc.Connect(display))
                                {
                                    // Test invalid, consider it a failure.
                                    failed.Add(string.Format("FATAL Error: Could not connect to run file {0}.", file));
                                    continue;
                                }

                                // Should be connected by now.
                                foreach (string line in lines)
                                {
                                    if (line.Trim() == string.Empty)
                                    {
                                        // We ignore empty lines or lines with whitespace.
                                        continue;
                                    }

                                    if (line[0] == '#')
                                    {
                                        // Its a comment.. We ignore it.
                                        continue;
                                    }

                                    if (line.IndexOf("|", 0, StringComparison.OrdinalIgnoreCase) == -1)
                                    {
                                        // Test invalid, consider it a failure.
                                        failed.Add(string.Format("FATAL Error: In file {0}, line has no seperator. Contents: {1}", file, line));
                                        break;
                                    }

                                    string[] data = line.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                                    try
                                    {
                                        switch (data[0])
                                        {
                                            // s|data as string
                                            case "s":
                                            case "send":
                                                if (!tsc.Send(data[1]))
                                                {
                                                    throw new Exception(string.Format("FATAL Error: Data could not be sent: '{0}'.", data[1]));
                                                }
                                                break;
                                            case "r":
                                            case "recv":
                                                // r|regex as string
                                                string rcv;
                                                tsc.Recieve(out rcv);
                                                if (false == Regex.IsMatch(rcv, data[1]))
                                                {
                                                    throw new Exception(string.Format("FATAL Error: Data Returned does not match regex of '{0}'.", data[1]));
                                                }
                                                break;
                                            case "w":
                                            case "wait":
                                            case "waitfor":
                                                // w|timeout as int|regex as string
                                                string wtr;
                                                int c = 0;
                                                int to = int.Parse(data[1]);
                                                do
                                                {
                                                    Thread.Sleep(1000);
                                                    tsc.Recieve(out wtr);
                                                } while (c++ < to && !Regex.IsMatch(wtr, data[2]));

                                                if (c > to)
                                                {
                                                    throw new Exception(string.Format("FATAL Error: Timed out while waiting for match of regex '{0}'.", data[2]));
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        failed.Add(ex.Message);
                                    }
                                } // End foreach line

                                if (!tsc.Connected)
                                {
                                    // Thats bad.
                                    failed.Add(string.Format("FATAL Error: Still connected after test run."));
                                }

                                tsc.Disconnect();
                                tsc = null;
                            }

                            if (failed.Count > 0)
                            {
                                display.Notify("> A total of " + failed.Count + " Tests FAILED.");
                                foreach (string error in failed)
                                {
                                    display.Notify(">> " + error);
                                }
                                display.Notify(">> FATAL ERROR. Tests FAILED.");
                                failed.Clear();
                            }
                        }
                        else
                        {
                            display.Notify(string.Format("> Command Not Recognized. [{0}]", input));
                        }
                    }

					// This is for Mono compatability.
					input = string.Empty;
                }
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

        /// <summary>
        /// Handle the 'help' command as specified by the administrator from the console.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <param name="display">The console updater used to display output.</param>
        /// <param name="input">The full line of input, in case additional parameters need to be parsed.</param>
        private static void HandleHelpCommand(Application app, ConsoleUpdater display, string input)
        {
            app.DisplayHelp();
        }

        public class TestScriptClient
        {
            TcpClient client;
            NetworkStream str;
            MultiUpdater display;

            public bool Connected
            {
                get
                {
                    if (this.client == null)
                    {
                        return false;
                    }

                    return this.client.Connected;
                }
            }

            public bool Connect(MultiUpdater display)
            {
                try
                {
                    this.client = new TcpClient();

                    // Use default mud port.
                    this.client.Connect(new IPEndPoint(IPAddress.Loopback, 4000));

                    int attempts = 0;
                    while (!this.client.Connected && attempts++ < 10)
                    {
                        display.Notify("> Connecting to mud server on localhost port 4000..");
                        Thread.Sleep(1000);
                    }

                    this.display = display;
                    this.str = this.client.GetStream();

                    return true;
                }
                catch (Exception ex)
                {
                    this.display.Notify("> Fatal Error: " + ex.ToString());
                    this.client = null;
                    return false;
                }
            }

            public bool Send(string data)
            {
                byte[] buf = Encoding.UTF8.GetBytes(string.Format("{0}{1}", data, Environment.NewLine));

                try
                {
                    this.str.Write(buf, 0, buf.Length);
                }
                catch (Exception ex)
                {
                    this.display.Notify(">> ERROR: " + ex.ToString());
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

                    this.str.Read(buf, 0, buf.Length);

                    data = Encoding.ASCII.GetString(buf);
                }
                catch (Exception ex)
                {
                    this.display.Notify(">> FATAL Error: " + ex.ToString());
                    return false;
                }

                return true;
            }

            public void Disconnect()
            {
                if (this.Connected)
                {
                    this.Send("quit");
                    this.str.Close();
                    this.str = null;
                    this.client = null;
                }
            }
        }
    }
}
