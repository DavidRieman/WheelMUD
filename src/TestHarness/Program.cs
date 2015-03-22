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
using TestHarness.Commands;
using WheelMUD.Main;

namespace TestHarness
{
    /// <summary>The test harness program; runs the MUD as a console application.</summary>
    public class Program
    {
        /// <summary>Main entry point into the test harness.</summary>
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

            // TODO: Consider reflecting implementers of ITestHarnessCommand to keep this automatically up to date?
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

        /// <summary>Notifies the user of a message.</summary>
        /// <param name="message">The message to be notified.</param>
        public void Notify(string message)
        {
            Console.WriteLine(message);
        }
    }
}