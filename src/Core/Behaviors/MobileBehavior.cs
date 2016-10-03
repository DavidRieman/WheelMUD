//-----------------------------------------------------------------------------
// <copyright file="MobileBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: May 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using WheelMUD.Core.Events;

    /// <summary>@@@ Might be better renamed to AIBrainBehavior or something...?</summary>
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
            this.ID = instanceID;
        }

        /// <summary>Receives an event.</summary>
        /// <param name="root">The root.</param>
        /// <param name="theEvent">The event to be received.</param>
        public void Receive(Thing root, GameEvent theEvent)
        {
            ////this.brain.ProcessStimulus(theEvent);
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            // @@@ For now, rigging MobileBehavior back up to all events like Mobile used to; I don't
            //     want to mess with AI too much right now especially since Fasta is working on it.
            // Prepare to handle receiving all relevant sensory events (not requests) which have 
            // happened within the player's perception, and relay the sensory message to the player.
            if (this.Parent != null)
            {
                this.Parent.Eventing.CombatEvent += this.Receive;
                this.Parent.Eventing.MovementEvent += this.Receive;
                this.Parent.Eventing.CommunicationEvent += this.Receive;
                this.Parent.Eventing.MiscellaneousEvent += this.Receive;
            }
        }

        // @@@ TODO: After moving, do this.ProcessSurroundings();

        /* @@@ TODO: Ensure mobile entities are loaded just like any other entity...
        /// <summary>Loads the mobile.</summary>
        public void Load()
        {
            // @@@ TODO: Stuff should be loaded from the Data Layer instead of using temp values.
            this.Stats.Add("health", new StatHP(Controller, 100, 100));
            this.Stats.Add("power", new Stat(Controller, "Power", 100, 0, 100, true));
            this.Stats.Add("strength", new Stat(Controller, "Srength", 40, 0, 100, true));
            this.Stats.Add("balance", new Stat(Controller, "Balance", 1, 0, 1, false));
            this.Stats.Add("mobility", new Stat(Controller, "Mobility", 2, -10, 2, false));
            this.Stats.Add("agility", new Stat(Controller, "Agility", 2, -10, 2, true));
            this.Stats.Add("perception", new Stat(Controller, "Perception", 2, -10, 2, false));

            this.Commands.Add("punch", new Command("punch", SecurityRole.mobile));

            this.LoadInventory();
        }

        /// <summary>Saves the mobile.</summary>
        public override void Save()
        {
            var repository = new MobRepository();

            if (this.Id == 0)
            {
                repository.Add(this.DataRecord);
                this.Id = this.DataRecord.ID;
            }
            else
            {
                repository.Update(this.DataRecord);
            }
        }*/
    }
}