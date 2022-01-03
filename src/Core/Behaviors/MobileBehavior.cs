//-----------------------------------------------------------------------------
// <copyright file="MobileBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace WheelMUD.Core
{
    /// <summary>TODO: Might be better renamed to AIBrainBehavior or something...?</summary>
    public class MobileBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the MobileBehavior class.</summary>
        public MobileBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the MobileBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public MobileBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceID;
        }

        /// <summary>Receives an event.</summary>
        /// <param name="root">The root.</param>
        /// <param name="theEvent">The event to be received.</param>
        public void Receive(Thing root, GameEvent theEvent)
        {
            ////brain.ProcessStimulus(theEvent);
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            // TODO: For now, rigging MobileBehavior back up to all events like Mobile used to; I don't
            //       want to mess with AI too much right now especially since Fasta is working on it.
            // Prepare to handle receiving all relevant sensory events (not requests) which have 
            // happened within the player's perception, and relay the sensory message to the player.
            if (Parent != null)
            {
                Parent.Eventing.CombatEvent += Receive;
                Parent.Eventing.MovementEvent += Receive;
                Parent.Eventing.CommunicationEvent += Receive;
                Parent.Eventing.MiscellaneousEvent += Receive;
            }
        }

        // TODO: After moving, do ProcessSurroundings();
    }
}