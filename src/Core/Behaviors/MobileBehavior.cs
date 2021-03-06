﻿//-----------------------------------------------------------------------------
// <copyright file="MobileBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;

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

        /* TODO: Ensure mobile entities are loaded just like any other entity...
        /// <summary>Loads the mobile.</summary>
        public void Load()
        {
            // TODO: Stuff should be loaded from the Data Layer instead of using temp values.
            Stats.Add("health", new StatHP(Controller, 100, 100));
            Stats.Add("power", new Stat(Controller, "Power", 100, 0, 100, true));
            Stats.Add("strength", new Stat(Controller, "Srength", 40, 0, 100, true));
            Stats.Add("balance", new Stat(Controller, "Balance", 1, 0, 1, false));
            Stats.Add("mobility", new Stat(Controller, "Mobility", 2, -10, 2, false));
            Stats.Add("agility", new Stat(Controller, "Agility", 2, -10, 2, true));
            Stats.Add("perception", new Stat(Controller, "Perception", 2, -10, 2, false));

            Commands.Add("punch", new Command("punch", SecurityRole.mobile));

            LoadInventory();
        }

        /// <summary>Saves the mobile.</summary>
        public override void Save()
        {
            var repository = new MobRepository();

            if (Id == 0)
            {
                repository.Add(DataRecord);
                Id = DataRecord.ID;
            }
            else
            {
                repository.Update(DataRecord);
            }
        }*/
    }
}