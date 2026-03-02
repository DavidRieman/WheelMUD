// Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is subject to the Microsoft Public License.  All other rights reserved.

using System;
using System.Threading.Tasks;
using WheelMUD.Core;
using WheelMUD.Server;

namespace WheelMUD.ConnectionStates
{
    /// <summary>The handshaking session state gives time for initial negotiations to occur before displaying the splash screen.</summary>
    /// <remarks>
    /// This state is particularly useful for the Telnet protocol, where client detection, screen size negotiation, and other display
    /// capabilities are useful for rendering an optimal splash screen for this specific client connection. This state simply bides a
    /// little time before transitioning to the ConnectedState.  For now, we employ a higher priority than ConnectedState to perform
    /// this strategy.  TODO: Eventually we may want to consider optionally avoiding this strategy (and printing splash immediately)
    /// for future client connection protocols which don't need the delay.
    /// </remarks>
    /// <param name="session">The session entering this state.</param>
    [ExportSessionState(10)]
    public class HandshakingState(Session session) : SessionState(session)
    {
        /// <summary>Initializes a new instance of the HandshakingState class.</summary>
        /// <remarks>This constructor is required to support MEF discovery as our default connection state.</remarks>
        public HandshakingState() : this(null) { }

        /// <summary>Begin the handshaking state.</summary>
        /// <remarks>TODO: Perhaps the delay time should be configurable via App.config?</remarks>
        public override void Begin()
        {
            // Don't send anything yet: Wait a brief moment for telnet negotiations to occur.
            // This might not be enough for especially long ping time connections, but should generally be sufficient. The worst
            // case scenario is that the splash screen just doesn't render optimally for SOME clients in that bucket.
            Task.Delay(150).ContinueWith(_ => TransitionToConnectedState());
        }

        /// <summary>Would process input, but we'll ignore input during this phase.</summary>
        /// <remarks>TODO: Can/should early input (E.G. automated character name input) stay in the buffer explicitly?</remarks>
        /// <param name="input">The input.</param>
        public override void ProcessInput(string input)
        {
            // During brief handshaking, we currently ignore user input.
        }

        /// <summary>Builds an empty prompt since we don't show prompts during handshaking.</summary>
        /// <returns>An empty output builder.</returns>
        public override OutputBuilder BuildPrompt()
        {
            return new OutputBuilder();
        }

        /// <summary>Transitions to the ConnectedState and displays the splash screen.</summary>
        private void TransitionToConnectedState()
        {
            try
            {
                // Now that negotiations have had time to settle, show the splash screen.
                Session.Write(Renderer.Instance.RenderSplashScreen(Session.TerminalOptions), false);

                // Transition to the normal connected state.
                Session.SetState(new ConnectedState(Session));
            }
            catch (Exception ex)
            {
                // Log any errors during transition.
                Session.InformSubscribedSystem($"Error transitioning a user from HandshakingState: {ex.Message}");
            }
        }
    }
}
