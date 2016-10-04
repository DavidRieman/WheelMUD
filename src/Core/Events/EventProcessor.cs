//-----------------------------------------------------------------------------
// <copyright file="EventProcessor.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An event processor for a Player.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections;
    using WheelMUD.Core.Events;

    /// <summary>An event processor for a Player.</summary>
    public class PlayerEventProcessor : IDisposable
    {
        private PlayerBehavior playerBehavior;
        private SensesBehavior sensesBehavior;
        private UserControlledBehavior userControlledBehavior;

        /// <summary>Initializes a new instance of the <see cref="PlayerEventProcessor"/> class.</summary>
        /// <param name="playerBehavior">The player behavior.</param>
        /// <param name="sensesBehavior">The senses behavior.</param>
        /// <param name="userControlledBehavior">The user controlled behavior.</param>
        public PlayerEventProcessor(PlayerBehavior playerBehavior, SensesBehavior sensesBehavior, UserControlledBehavior userControlledBehavior)
        {
            this.playerBehavior = playerBehavior;
            this.sensesBehavior = sensesBehavior;
            this.userControlledBehavior = userControlledBehavior;
        }

        /// <summary>Attaches player-related events such as combat, movement, and communication.</summary>
        public void AttachEvents()
        {
            Thing player = this.playerBehavior.Parent;

            // Prepare to handle receiving all relevant sensory events (not requests) which have 
            // happened within the player's perception, and relay the sensory message to the player.
            player.Eventing.CombatEvent += this.ProcessEvent;
            player.Eventing.MovementEvent += this.ProcessEvent;
            player.Eventing.CommunicationEvent += this.ProcessEvent;
            player.Eventing.MiscellaneousEvent += this.ProcessEvent;
        }

        /// <summary>Process a specified event.</summary>
        /// <param name="root">The root.</param>
        /// <param name="e">The event to be processed.</param>
        public void ProcessEvent(Thing root, GameEvent e)
        {
            // Events with no sensory component have no chance to be percieved/relayed to player's terminal...
            if (e.SensoryMessage != null)
            {
                string output = this.ProcessMessage(e.SensoryMessage);
                if (output != string.Empty)
                {
                    this.userControlledBehavior.Controller.Write(output);
                }
            }
        }

        /// <summary>Dispose of any resources used by this EventProcessor.</summary>
        public void Dispose()
        {
            if (this.playerBehavior != null)
            {
                Thing player = this.playerBehavior.Parent;
                if (player != null)
                {
                    player.Eventing.CombatEvent -= new GameEventHandler(this.ProcessEvent);
                    player.Eventing.MovementEvent -= new GameEventHandler(this.ProcessEvent);
                    player.Eventing.CommunicationEvent -= new GameEventHandler(this.ProcessEvent);
                    player.Eventing.MiscellaneousEvent -= new GameEventHandler(this.ProcessEvent);
                }
            }
        }

        /// <summary>Process a sensory message.</summary>
        /// <param name="message">The sensory message to be processed.</param>
        /// <returns>The rendered view of this sensory message.</returns>
        private string ProcessMessage(SensoryMessage message)
        {
            if (this.sensesBehavior.Senses.CanProcessSensoryMessage(message))
            {
                // Build the contexts from those added to the sensory message builder 
                // and those added to the sensory message.
                Hashtable context = message.Context;

                if (message.Message != null)
                {
                    foreach (DictionaryEntry entry in message.Message.ViewEngineContext)
                    {
                        context.Add(entry.Key, entry.Value);
                    }

                    if (this.userControlledBehavior != null)
                    {
                        string parsedMessage = message.Message.Parse(this.userControlledBehavior.Parent);
                        string output = this.userControlledBehavior.ViewEngine.RenderView(parsedMessage, context);
                        return output;
                    }
                }
            }

            return string.Empty;
        }

        /* @@@ FIX?
        /// <summary>Process the specified DebugEvent.</summary>
        /// <param name="debugEvent">The DebugEvent to be processed.</param>
        private void ProcessDebugEvent(DebugEvent debugEvent)
        {
            if (this.sensesBehavior.Senses.CanProcessSensoryMessage(debugEvent.Message))
            {
                string message = this.ProcessMessage(debugEvent.Message);
                if (!string.IsNullOrEmpty(message))
                {
                    this.userControlledBehavior.Controller.Write(string.Format("(debug - {0}) - {1}", debugEvent.ActiveThing.Name, message));
                }
            }
        }*/
    }
}