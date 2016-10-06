//-----------------------------------------------------------------------------
// <copyright file="RunTestsCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Command to run integration tests.
// </summary>
//-----------------------------------------------------------------------------

namespace TestHarness.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using WheelMUD.Main;

    public class RunTestsCommand : ITestHarnessCommand
    {
        private readonly string[] names = { "RUN", "RUN-TESTS", "run", "Run", "r" };

        public IEnumerable<string> Names
        {
            get { return names; }
        }

        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            if (!Directory.Exists(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Tests"))
            {
                display.Notify("> FATAL ERROR: Tests Directory does not exist.");
                return;
            }

            var failed = new List<string>();
            string[] files = Directory.GetFiles(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Tests", "*.testscript", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                var lines = File.ReadAllLines(file);
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
                                }
                                while (c++ < to && !Regex.IsMatch(wtr, data[2]));

                                if (c > to)
                                {
                                    throw new Exception(string.Format("FATAL Error: Timed out while waiting for match of regex '{0}'.", data[2]));
                                }

                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        failed.Add(ex.Message);
                    }
                }

                if (!tsc.Connected)
                {
                    // Thats bad.
                    failed.Add(string.Format("FATAL Error: Still connected after test run."));
                }

                tsc.Disconnect();
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
    }
}