//-----------------------------------------------------------------------------
// <copyright file="HelpCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Help command for the TestHarness.
// </summary>
//-----------------------------------------------------------------------------

using WheelMUD.Main;

namespace TestHarness.Commands
{
    /// <summary>
    /// Handle the 'help' command as specified by the administrator from the console.
    /// </summary>
    public class HelpCommand : ITestHarnessCommand
    {
        /// <summary>Recognized names for this command.</summary>
        private readonly string[] _names = { "?", "HELP", "help", "h" };

        /// <summary>Gets the recognized names for this command.</summary>
        public string[] Names
        {
            get { return _names; }
        }

        /// <summary>Execute the Help command.</summary>
        /// <param name="app">The application to display help for.</param>
        /// <param name="display">Where to capture display output (such as console and text logs).</param>
        /// <param name="words">TODO: Establish what this should be or remove; perhaps meant to be all supplied words beyond "help"?</param>
        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            app.DisplayHelp();
        }
    }
}