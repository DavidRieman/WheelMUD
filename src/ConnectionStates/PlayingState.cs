//-----------------------------------------------------------------------------
// <copyright file="PlayingState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Diagnostics;
using WheelMUD.Core;
using WheelMUD.Data;
using WheelMUD.Utilities;

namespace WheelMUD.ConnectionStates
{
    /// <summary>The 'playing' session state.</summary>
    public class PlayingState : SessionState
    {
        private SimpleWeakReference<PlayerBehavior> playerBehavior;

        /// <summary>Initializes a new instance of the PlayingState class.</summary>
        /// <param name="session">The session entering this state.</param>
        public PlayingState(Session session)
            : base(session)
        {
            Debug.Assert(session.Thing != null);
            var behavior = session.Thing.Behaviors.FindFirst<PlayerBehavior>();
            playerBehavior = new SimpleWeakReference<PlayerBehavior>(behavior);
        }

        public override void Begin()
        {
            // If we have no automatic command (like "look") configured for processing upon login, then we should print the prompt
            // after the welcome text to indicate we are ready for user input now. Else skip the prompt as the automatic command
            // should generate one once that command has been processed in response to login.
            // TODO: https://github.com/DavidRieman/WheelMUD/issues/56 - Finish implementing AutomaticLoginCommand, and try:
            //  bool includePrompt = !string.IsNullOrWhiteSpace(AppConfigInfo.Instance.AutomaticLoginCommand);
            bool includePrompt = true;
            Session.WriteAnsiLine($"Welcome, {Session.Thing.FullName}.", includePrompt);
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                var actionInput = new ActionInput(command, Session);
                Session.ExecuteAction(actionInput);
            }
            else
            {
                // Just give the player a fresh prompt. (They may want to see stat recovery details in the prompt, or just verify their connection, etc.)
                Session.WritePrompt();
            }
        }

        public override string BuildPrompt()
        {
            PlayerBehavior playerBehavior = this.playerBehavior.Target;
            if (playerBehavior != null)
            {
                return playerBehavior.BuildPrompt();
            }

            Debug.Assert(false, "A non-Player is in PlayingState, receiving a Prompt?");
            return "> ";
        }

        public override bool SupportsPaging { get; } = true;
    }
}