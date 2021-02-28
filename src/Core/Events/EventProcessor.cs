//-----------------------------------------------------------------------------
// <copyright file="EventProcessor.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    /// <summary>An event processor for a Player.</summary>
    public class PlayerEventProcessor : IDisposable
    {
        private readonly PlayerBehavior playerBehavior;
        private readonly SensesBehavior sensesBehavior;
        private readonly UserControlledBehavior userControlledBehavior;

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

        /// <summary>Attaches all player-related events such as combat, movement, and communication.</summary>
        public void AttachEvents()
        {
            // Prepare to handle receiving all relevant sensory events (not requests) which have 
            // happened within the player's perception, and relay the sensory message to the player.
            var player = playerBehavior.Parent;
            player.Eventing.CombatEvent += ProcessEvent;
            player.Eventing.MovementEvent += ProcessEvent;
            player.Eventing.CommunicationEvent += ProcessEvent;
            player.Eventing.MiscellaneousEvent += ProcessEvent;
        }

        /// <summary>Detaches all player-related events such as combat, movement, and communication.</summary>
        public void DetachEvents()
        {
            var player = playerBehavior.Parent;
            player.Eventing.CombatEvent -= ProcessEvent;
            player.Eventing.MovementEvent -= ProcessEvent;
            player.Eventing.CommunicationEvent -= ProcessEvent;
            player.Eventing.MiscellaneousEvent -= ProcessEvent;
        }

        /// <summary>Process a specified event.</summary>
        /// <param name="root">The root.</param>
        /// <param name="e">The event to be processed.</param>
        public void ProcessEvent(Thing root, GameEvent e)
        {
            // Events with no sensory component have no chance to be percieved/relayed to player's terminal...
            if (e.SensoryMessage != null)
            {
                string output = ProcessMessage(e.SensoryMessage);
                if (output != string.Empty)
                {
                    userControlledBehavior.Controller.Write(output);
                }
            }
        }

        /// <summary>Dispose of any resources used by this EventProcessor.</summary>
        public void Dispose()
        {
            if (playerBehavior != null)
            {
                var player = playerBehavior.Parent;
                if (player != null)
                {
                    player.Eventing.CombatEvent -= ProcessEvent;
                    player.Eventing.MovementEvent -= ProcessEvent;
                    player.Eventing.CommunicationEvent -= ProcessEvent;
                    player.Eventing.MiscellaneousEvent -= ProcessEvent;
                }
            }
        }

        /// <summary>Process a sensory message.</summary>
        /// <param name="message">The sensory message to be processed.</param>
        /// <returns>The rendered view of this sensory message.</returns>
        private string ProcessMessage(SensoryMessage message)
        {
            if (sensesBehavior.Senses.CanProcessSensoryMessage(message) && message.Message != null && userControlledBehavior != null)
            {
                return message.Message.Parse(userControlledBehavior.Parent);
            }

            return string.Empty;
        }
    }
}