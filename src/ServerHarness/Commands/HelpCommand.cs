//-----------------------------------------------------------------------------
// <copyright file="HelpCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ServerHarness
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WheelMUD.Main;
    using WheelMUD.Utilities;

    /// <summary>Handle the 'help' command as specified by the administrator from the console.</summary>
    public class HelpCommand : IServerHarnessCommand
    {
        public string Description => "Retrieve basic help for this ServerHarness, including a list of commands.";

        /// <summary>Gets the recognized names for this command.</summary>
        public IEnumerable<string> Names => new string[] { "?", "HELP", "help", "h" };

        /// <summary>Execute the Help command.</summary>
        /// <param name="app">The application to display help for.</param>
        /// <param name="display">Where to capture display output (such as console and text logs).</param>
        /// <param name="words">TODO: Establish what this should be or remove; perhaps meant to be all supplied words beyond "help"?</param>
        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            display.Notify(this.DisplayHelp());
        }

        /// <summary>Present the console command-line "HELP" response to the administrator.</summary>
        private string DisplayHelp()
        {
            string fancyLine = "-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-";
            var sb = new StringBuilder();
            sb.AppendLine(fancyLine);
            sb.AppendLine($"  Active Game:  {GameConfiguration.Name}  version {GameConfiguration.Version}");
            if (!string.IsNullOrWhiteSpace(GameConfiguration.Website))
            {
                sb.AppendLine($"Game Website: {GameConfiguration.Website}");
            }
            sb.AppendLine(fancyLine);
            sb.AppendLine("This game is built from a base of WheelMUD. For more information about the");
            sb.AppendLine("base game engine, visit: https://github.com/WheelMUD/WheelMUD");
            sb.AppendLine();
            sb.AppendLine("Available administrative commands:");
            foreach (var command in ServerHarnessCommands.AllCommands)
            {
                sb.AppendLine($"{command.Names.First(),16} - {command.Description}");
            }

            return sb.ToString();
        }
    }
}