//-----------------------------------------------------------------------------
// <copyright file="HelpCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using WheelMUD.Main;

namespace ServerHarness
{
    /// <summary>Handle the 'help' command as specified by the administrator from the console.</summary>
    [ExportServerHarnessCommand(0)]
    public class HelpCommand : IServerHarnessCommand
    {
        public string Description => "Retrieve basic help for this ServerHarness, including a list of commands.";

        /// <summary>Gets the recognized names for this command.</summary>
        public IEnumerable<string> Names => new[] { "?", "HELP", "H" };

        /// <summary>Execute the Help command.</summary>
        /// <param name="app">The application to display help for.</param>
        /// <param name="display">Where to capture display output (such as console and text logs).</param>
        /// <param name="words">TODO: Establish what this should be or remove; perhaps meant to be all supplied words beyond "help"?</param>
        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            display.Notify(DisplayHelp());
        }

        /// <summary>Present the console command-line "HELP" response to the administrator.</summary>
        private string DisplayHelp()
        {
            var ab = new StringBuilder();
            ab.AppendLine(Application.Instance.BasicAdministrativeGameInfo);
            ab.AppendLine("----------------------------------");
            ab.AppendLine("Available administrative commands:");
            ab.AppendLine("----------------------------------");

            foreach (var command in ServerHarnessCommands.Instance.AllCommands)
            {
                ab.AppendLine($"{string.Join(",", command.Names.Take(2).ToArray()),10} - {command.Description}");
            }

            return ab.ToString();
        }
    }
}