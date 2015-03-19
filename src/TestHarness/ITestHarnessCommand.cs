
using WheelMUD.Main;

namespace TestHarness
{
    public interface ITestHarnessCommand
    {
        string[] Names { get; }

        void Execute(Application app, MultiUpdater display, string[] words);
    }
}
