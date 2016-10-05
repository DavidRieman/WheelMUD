//-----------------------------------------------------------------------------
// <copyright file="PlayingState.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The 'playing' session state.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using WheelMUD.Core;
    using WheelMUD.Utilities;

    /// <summary>The 'playing' session state.</summary>
    public class PlayingState : SessionState
    {
        private WeakReference<PlayerBehavior> playerBehavior;

        /// <summary>Initializes a new instance of the PlayingState class.</summary>
        /// <param name="session">The session entering this state.</param>
        public PlayingState(Session session)
            : base(session)
        {
            // JFED -- Temporary workaround - null exceptions
            if (session.Thing != null)
            {
                var behavior = session.Thing.Behaviors.FindFirst<PlayerBehavior>();
                this.playerBehavior = new WeakReference<PlayerBehavior>(behavior);
            }

            string nl = Environment.NewLine;
            session.Write(string.Format("{0}Welcome, {1}.{0}{0}", nl, this.Session.UserName), false);
        }

        /// <summary>Process the specified input.</summary>
        /// <param name="command">The input to process.</param>
        public override void ProcessInput(string command)
        {
            this.Session.AtPrompt = false;
            if (command != string.Empty)
            {
                var actionInput = new ActionInput(command, this.Session);
                this.Session.ExecuteAction(actionInput);
            }
            else
            {
                this.Session.SendPrompt();
            }
        }

        public override string BuildPrompt()
        {
            // JFED -- Temporary workaround - null exceptions
            try
            {
                PlayerBehavior playerBehavior = this.playerBehavior.Target;
                if (playerBehavior != null)
                {
                    return playerBehavior.BuildPrompt();
                }

                return "> ";
            }
            catch (Exception)
            {
                return "Err> ";
            }
        }
    }
}