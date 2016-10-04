//-----------------------------------------------------------------------------
// <copyright file="GameSystemController.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 5/15/2009 5:30:30 PM
//   Purpose   : Top class for the GameEngine.
// </summary>
// <history>
//   2011-06-01 by Karak - Removed CSScript in favor of MEF discovery w/compiled parts
// </history>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using WheelMUD.Interfaces;
    using WheelMUD.Rules;

    /// <summary>Controls the GamingEngine.</summary>
    public class GameSystemController : ISystem, IRecomposable
    {
        /// <summary>The singleton instance of this class.</summary>
        private static readonly GameSystemController SingletonInstance = new GameSystemController();

        /// <summary>The host system that the GameEngine is subscribing to.</summary>
        private ISystemHost host;

        /// <summary>Prevents a default instance of the GameSystemController class from being created.</summary>
        /// <remarks>This is a private controller, because it is using the singleton pattern to instantiate a copy.</remarks>
        private GameSystemController()
        {
        }

        /// <summary>Gets the singleton instance of the <see cref="GameSystemController"/> class.</summary>
        public static GameSystemController Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Gets or sets the game attributes used by the current gaming system.</summary>
        public List<GameAttribute> GameAttributes { get; set; }

        /// <summary>Gets or sets the genders available to the current gaming system.</summary>
        public List<GameGender> GameGenders { get; set; }

        /// <summary>Gets or sets the game modifiers for the current gaming system.</summary>
        public List<GameModifier> GameModifiers { get; set; }

        /// <summary>Gets or sets the racial templates.</summary>
        public List<GameRace> GameRaces { get; set; }

        /// <summary>Gets or sets a list of rules associated with the current gaming system.</summary>
        /// <remarks>This is a generic store for rules that don't fit into a specific category.</remarks>
        public List<GameRule> GameRules { get; set; }

        /// <summary>Gets or sets the game skills for the current gaming system.</summary>
        public List<GameSkill> GameSkills { get; set; }

        /// <summary>Gets or sets a list of game stats used by the current gaming system.</summary>
        public List<GameStat> GameStats { get; set; }

        /// <summary>Gets or sets the combat engine to the current gaming system.</summary>
        public ICombat GameCombatEngine { get; set; }

        /// <summary>Gets or sets the game attributes used by the current gaming system.</summary>
        [ImportMany]
        private List<GameAttribute> ImportedGameAttributes { get; set; }

        /// <summary>Gets or sets the genders available to the current gaming system.</summary>
        [ImportMany]
        private List<GameGender> ImportedGameGenders { get; set; }

        /// <summary>Gets or sets the game modifiers for the current gaming system.</summary>
        [ImportMany]
        private List<GameModifier> ImportedGameModifiers { get; set; }

        /// <summary>Gets or sets the racial templates.</summary>
        [ImportMany]
        private List<GameRace> ImportedGameRaces { get; set; }

        /// <summary>Gets or sets a list of rules associated with the current gaming system.</summary>
        /// <remarks>This is a generic store for rules that don't fit into a specific category.</remarks>
        [ImportMany]
        private List<GameRule> ImportedGameRules { get; set; }

        /// <summary>Gets or sets the game skills for the current gaming system.</summary>
        [ImportMany]
        private List<GameSkill> ImportedGameSkills { get; set; }

        /// <summary>Gets or sets a list of game stats used by the current gaming system.</summary>
        [ImportMany]
        private List<GameStat> ImportedGameStats { get; set; }

        /// <summary>Allows the sub system host to receive an update when subscribed to this system.</summary>
        /// <param name="sender">The sender of the update.</param>
        /// <param name="msg">The message to be sent.</param>
        public void UpdateSubSystemHost(ISubSystem sender, string msg)
        {
            this.host.UpdateSystemHost(this, msg);
        }

        /// <summary>Subscribes this system to the specified system host, so that host can receive updates.</summary>
        /// <param name="sender">The system host to receive our updates.</param>
        public void SubscribeToSystemHost(ISystemHost sender)
        {
            this.host = sender;
        }

        /// <summary>Starts this system's individual components.</summary>
        public void Start()
        {
            this.host.UpdateSystemHost(this, "Starting...");
            this.Recompose();
            this.host.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops this system's individual components.</summary>
        public void Stop()
        {
            this.host.UpdateSystemHost(this, "Stopping");
            this.host.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Recompose the GameSystemController with the latest components available.</summary>
        public void Recompose()
        {
            // Recompose the private Imported properties, then prepare a new usable list of each game element,
            // and replace the public list with a new one.  NOTE: we do not modify the existing public lists
            // at any time because they may be actively being iterated by other threads.
            DefaultComposer.Container.ComposeParts(this);

            this.GameAttributes = DefaultComposer.GetLatestDistinctTypeInstances(this.ImportedGameAttributes);
            this.GameGenders = DefaultComposer.GetLatestDistinctTypeInstances(this.ImportedGameGenders);
            this.GameModifiers = DefaultComposer.GetLatestDistinctTypeInstances(this.ImportedGameModifiers);
            this.GameRaces = DefaultComposer.GetLatestDistinctTypeInstances(this.ImportedGameRaces);
            this.GameRules = DefaultComposer.GetLatestDistinctTypeInstances(this.ImportedGameRules);
            this.GameSkills = DefaultComposer.GetLatestDistinctTypeInstances(this.ImportedGameSkills);
            this.GameStats = DefaultComposer.GetLatestDistinctTypeInstances(this.ImportedGameStats);
        }

        /// <summary>Exports an instance of the GameSystemController to MEF.</summary>
        [ExportSystem]
        public class GameSystemManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            /// <returns>A new instance of the singleton system.</returns>
            public override ISystem Instance
            {
                get { return GameSystemController.Instance; }
            }

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType
            {
                get { return typeof(GameSystemController); }
            }
        }
    }
}