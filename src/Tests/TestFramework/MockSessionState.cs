using WheelMUD.Core;
using WheelMUD.Server;

namespace TestHelpers
{
    /// <summary>A mock SessionState for testing purposes.</summary>
    /// <remarks>TODO: Consider which mocking framework we should use to create such things in a better way.</remarks>
    public class MockSessionState : SessionState
    {
        /// <summary>Initializes a new instance of the <see cref="MockSessionState"/> class.</summary>
        /// <param name="session">The session entering this state.</param>
        public MockSessionState(Session session) : base(session)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MockSessionState"/> class.</summary>
        /// <remarks>This constructor is required to support MEF discovery as our default connection state.</remarks>
        public MockSessionState() : this(null)
        {
        }

        public override void Begin()
        {
            Session.Write(new OutputBuilder().AppendLine("Begin MockSessionState!"));
        }

        public static string LastProcessedInput { get; private set; }

        public override OutputBuilder BuildPrompt()
        {
            return new OutputBuilder().Append("FakePrompt > ");
        }

        public override void ProcessInput(string command)
        {
            LastProcessedInput = command;
        }
    }
}
