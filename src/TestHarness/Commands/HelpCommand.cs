
using WheelMUD.Main;

namespace TestHarness.Commands
{
    /// <summary>
    /// Handle the 'help' command as specified by the administrator from the console.
    /// </summary>
    public class HelpCommand : ITestHarnessCommand
    {
        private readonly string[] _names = {"?", "HELP", "help", "h"};

        public string[] Names
        {
            get { return _names; }
        }

        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            app.DisplayHelp();
        }
    }
}
