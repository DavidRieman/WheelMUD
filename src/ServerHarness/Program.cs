//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Topshelf;
using WheelMUD.Main;

namespace ServerHarness
{
    /// <summary>The test harness program. Entry point for the MUD, whether launching in command-line mode or as a Windows Service.</summary>
    public class Program
    {
        /// <summary>Main entry point into the test harness.</summary>
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                // Auto-starting Windows services will always have commandline arguments.  Or, if a user wants to do
                // something specific (even requesting help output) then we should go through the normal operation
                // via Topshelf, for Topshelf to handle those arguments.  Some examples may be "install --sudo" to
                // ask Topshelf to install WheelMUD as a Windows service, "start" to start said service, "stop" and
                // "uninstall --sudo" to ask Topshelf to stop and uninstall the service, etc.
                // http://docs.topshelf-project.com/en/latest/overview/commandline.html
                RunWithTopshelf();
            }
            else
            {
                // If we had no commandline arguments, such as when F5 launching inside a development environment
                // for debugging:  Launch in the interactive console mode where we can type server commands, start
                // integration tests, see server logs directly, and so on.
                RunWithConsoleHarness();
            }
        }

        /// <summary>Execute WheelMUD program via Topshelf.</summary>
        public static void RunWithTopshelf()
        {
            HostFactory.Run(x =>
            {
                x.DependsOnEventLog();
                x.StartAutomatically();
                x.RunAsLocalService();
                x.Service<WheelMudService>(s =>
                {
                    s.ConstructUsing(name => new WheelMudService());
                    s.WhenStarted(service => service.Start(null));
                    s.WhenStopped(service => service.Stop(null));
                });

                x.SetDescription("Allows the WheelMUD MUD Server to run as a Windows Service.");
                x.SetDisplayName("WheelMUD Server");
                x.SetServiceName("WheelMUDWindowsService");
            });
        }

        /// <summary>Execute WheelMUD program via our own console-mode server harness.</summary>
        public static void RunWithConsoleHarness()
        {
            string logFileName = "Log_" + DateTime.Now.ToShortDateString() + ".txt";
            logFileName = logFileName.Replace('\\', '_').Replace('/', '_');
            var consoleDisplay = new ConsoleUpdater();
            var textLogWriter = new TextLogUpdater(logFileName);
            var display = new MultiUpdater(consoleDisplay, textLogWriter);

            var app = Application.Instance;
            app.SubscribeToSystem(display);
            app.Start();

            bool done = false;
            var exitCommand = new DynamicServerHarnessCommand(() => done = true, new string[] { "SHUTDOWN", "EXIT" }, "Shuts down the game instance and server harness.");
            ServerHarnessCommands.Instance.DynamicCommands.Add(exitCommand);

            var commands = new Dictionary<string, IServerHarnessCommand>();
            foreach (var cmdObj in ServerHarnessCommands.Instance.AllCommands)
            {
                foreach (var name in cmdObj.Names)
                {
                    commands[name] = cmdObj;
                }
            }

            while (!done)
            {
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                {
                    continue;
                }

                var words = input.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                var command = FindCommand(commands, words[0]);
                if (command != null)
                {
                    command.Execute(app, display, words);
                }
                else
                {
                    display.Notify(string.Format("> Command not recognized: {0}", input));
                }
            }

            app.Stop();
        }

        private static IServerHarnessCommand FindCommand(IDictionary<string, IServerHarnessCommand> commands, string commandName)
        {
            return (from command in commands
                    where commandName.Equals(command.Key, StringComparison.OrdinalIgnoreCase)
                    select command.Value).FirstOrDefault();
        }

        /// <summary>Notifies the user of a message.</summary>
        /// <param name="message">The message to be notified.</param>
        public void Notify(string message)
        {
            Console.WriteLine(message);
        }
    }
}