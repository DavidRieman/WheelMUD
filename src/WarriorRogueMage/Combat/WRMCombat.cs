//-----------------------------------------------------------------------------
// <copyright file="WRMCombat.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : Jan 14, 2012
//   Purpose   : Warrior, Rogue, Mage based combat.
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;
    using WheelMUD.Interfaces;

    /// <summary>Basic combat system for Warrior, Rogue, and Mage RPG system.</summary>
    public class WrmCombat : ISystem, ICombat
    {
        /// <summary>The singleton instance of this class.</summary>
        private static WrmCombat singletonInstance = new WrmCombat();

        /// <summary>The combat queue that contains all combatants.</summary>
        private Queue combatQueue;

        /// <summary>Prevents a default instance of the <see cref="WrmCombat"/> class from being created.</summary>
        private WrmCombat()
        {
            this.CombatSession = new GameCombatSession();
        }

        /// <summary>Gets the singleton instance.</summary>
        /// <value>The instance.</value>
        public static WrmCombat Instance
        {
            get { return singletonInstance; }
        }

        /// <summary>Gets or sets the combat session.</summary>
        /// <value>The combat session.</value>
        public GameCombatSession CombatSession { get; set; }

        /// <summary>Gets the host of the manager system.</summary>
        public ISystemHost SystemHost { get; private set; }

        /// <summary>To be used in combat systems that are turn based.</summary>
        public void ProcessCombatRound()
        {
            this.combatQueue = new Queue(this.CombatSession.Combatants.Count);
            this.CreateCombatOrder();

            foreach (Thing combatant in this.CombatSession.Combatants)
            {
                this.ProcessCombatantRoundActions(combatant);
            }

            // When the round is over, clear the queue.
            this.combatQueue.Clear();
        }

        /// <summary>To be used in combat systems that are near-real time.</summary>
        public void ProcessCombatActions()
        {
            throw new NotImplementedException();
        }

        /// <summary>Creates the combat order.</summary>
        public void CreateCombatOrder()
        {
            Die combatDie = DiceService.Instance.GetDie(6);
            var bin = new Dictionary<string, int>();

            foreach (Thing combatant in this.CombatSession.Combatants)
            {
                // Find out who goes first, then add them to the queue.
                // Rinse and repeat, until all have been placed in the
                // correct order.

                // Do initiative roll.
                int combatantInitiative = DoInititiveRoll(combatDie, combatant);

                if (bin.ContainsValue(combatantInitiative))
                {
                    combatantInitiative = DoInititiveRoll(combatDie, combatant);
                }

                bin.Add(combatant.Name, combatantInitiative);
            }

            // Sort the bin dictionary
            ////var sortedBin = (from entry in bin orderby entry.Value ascending select entry).ToDictionary(pair => pair.Key, pair => pair.Value);

            ////foreach (var ent in sortedBin)
            ////{
            ////    Thing combatant = this.CombatSession.Combatants.Values
            ////}
        }

        /// <summary>Processes the combatant round actions.</summary>
        /// <param name="combatant">The combatant.</param>
        public void ProcessCombatantRoundActions(Thing combatant)
        {
            throw new NotImplementedException();
        }

        /// <summary>Starts this system.</summary>
        public void Start()
        {
            this.SystemHost.UpdateSystemHost(this, "Starting WRM combat engine...");
            this.SystemHost.UpdateSystemHost(this, "WRM combat engine has been started.");
        }

        /// <summary>Stops this system.</summary>
        public void Stop()
        {
            this.SystemHost.UpdateSystemHost(this, "Stopping WRM combat engine...");
            this.SystemHost.UpdateSystemHost(this, "WRM combat engine has been stopped.");
        }

        /// <summary>Allows the sub system host to receive an update when subscribed to this system.</summary>
        /// <param name="sender">The sender of the update.</param>
        /// <param name="message">The message to send to the host.</param>
        public void UpdateSubSystemHost(ISubSystem sender, string message)
        {
        }

        /// <summary>Subscribes this system to the specified system host, so that host can receive updates.</summary>
        /// <param name="sender">The system host to receive our updates.</param>
        public void SubscribeToSystemHost(ISystemHost sender)
        {
            this.SystemHost = sender;
        }

        private static int DoInititiveRoll(Die combatDie, Thing combatant)
        {
            int combatantInitiative = GetCombatantInitiative(combatDie);

            // Give them 2 extra points if they have the Awareness skill.
            combatantInitiative = CheckAwareness(combatant, combatantInitiative);

            return combatantInitiative;
        }

        private static int CheckAwareness(Thing combatant, int combatantInitiative)
        {
            if (combatant.Stats.ContainsKey("Awareness"))
            {
                combatantInitiative += 2;
            }

            return combatantInitiative;
        }

        private static int GetCombatantInitiative(Die combatDie)
        {
            return combatDie.Roll();
        }

        /// <summary>Warrior, Rogue, Mage combat exporter class.</summary>
        [ExportSystem]
        public class WrmCombatExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            /// <value></value>
            /// <returns>A new instance of the singleton system.</returns>
            public override ISystem Instance
            {
                get { return WrmCombat.Instance; }
            }

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType
            {
                get { return typeof(WrmCombat); }
            }
        }
    }
}