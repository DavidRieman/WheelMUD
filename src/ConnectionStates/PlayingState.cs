//-----------------------------------------------------------------------------
// <copyright file="PlayingState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Diagnostics;
using WheelMUD.Core;
using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.ConnectionStates
{
    /// <summary>The 'playing' session state.</summary>
    public class PlayingState : SessionState
    {
        private static readonly OutputBuilder basicPrompt = new OutputBuilder().Append("> ");
        private SimpleWeakReference<PlayerBehavior> playerBehavior;

        /// <summary>Initializes a new instance of the PlayingState class.</summary>
        /// <param name="session">The session entering this state.</param>
        public PlayingState(Session session)
            : base(session)
        {
            Debug.Assert(session.Thing != null);
            var behavior = session.Thing.FindBehavior<PlayerBehavior>();
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
            Session.WriteLine($"Welcome, {Session.Thing.FullName}.", includePrompt);
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                var actionInput = new ActionInput(command, Session, Session.Thing);

                // TODO: This session/player should have it's own queue which migrates to the global queue whenever the global
                //       queue does not have a pending action from this player.  IE the player should not be able to have two
                //       simultaneous actions in progress even though there may be multiple CommandProcessors consuming input.
                //       Also, a player who rapid-fired a ton of action input already should not get to starve other players
                //       from getting their own simpler action queues (like combat survival actions) processed.
                CommandManager.Instance.EnqueueAction(actionInput);
            }
            else
            {
                // Just give the player a fresh prompt. (They may want to see stat recovery details in the prompt, or just verify their connection, etc.)
                Session.WritePrompt();
            }
        }

        public override OutputBuilder BuildPrompt()
        {
            PlayerBehavior playerBehavior = this.playerBehavior.Target;
            if (playerBehavior != null)
            {
                return playerBehavior.BuildPrompt(Session.TerminalOptions);
            }

            Debug.Assert(false, "A non-Player is in PlayingState, receiving a Prompt?");
            return basicPrompt;
        }

        public override bool SupportsPaging { get; } = true;
    }
}