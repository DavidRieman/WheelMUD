using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using WheelMUD.Core;

namespace TestHelpers
{
    public static class MockCoreComposition
    {
        /// <summary>Provides access to the mock session state, for asserting the last input sent to the session, etc.</summary>
        public static MockSessionState SessionState { get; } = new MockSessionState();

        /// <summary>Performs a standard set of useful Core tech MEF composition mocks.</summary>
        /// <remarks>This should be used for most unit test init which might rely on user session setup.</remarks>
        public static void Basic()
        {
            DefaultComposer.Container = new CompositionContainer();
            DefaultComposer.Container.ComposeExportedValue<SessionState>(SessionState);
        }
    }
}
