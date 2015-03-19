using WheelMUD.Core;
using WheelMUD.Main;

namespace TestHarness.Commands
{
    public class UpdateActionsCommand : ITestHarnessCommand
    {
        private readonly string[] _names = { "UPDATE-ACTIONS", "UPDATE", "update", "u" };

        public string[] Names { get{ return _names; } }

        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            // @@@ TODO: Test, migrate to file system watcher (at the Application layer) instead?
            CommandManager.Instance.Recompose();
        }
    }
}
