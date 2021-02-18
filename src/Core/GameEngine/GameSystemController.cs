//-----------------------------------------------------------------------------
// <copyright file="GameSystemController.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using WheelMUD.Interfaces;

    /// <summary>Controls the games systems.</summary>
    public class GameSystemController : ISystem, IRecomposable
    {
        private static readonly Type[] NoTypes = new Type[] { };

        /// <summary>The host system that the GameEngine is subscribing to.</summary>
        private ISystemHost host;

        /// <summary>Prevents a default instance of the GameSystemController class from being created.</summary>
        /// <remarks>This is a private controller, because it is using the singleton pattern to instantiate a copy.</remarks>
        private GameSystemController()
        {
        }

        /// <summary>Gets the singleton instance of the <see cref="GameSystemController"/> class.</summary>
        public static GameSystemController Instance { get; } = new GameSystemController();

        /// <summary>Gets or sets the genders available to the current gaming system.</summary>
        public List<GameGender> GameGenders { get; set; }

        /// <summary>Gets or sets the game modifiers for the current gaming system.</summary>
        public List<GameModifier> GameModifiers { get; set; }

        /// <summary>Gets or sets the racial templates.</summary>
        public List<GameRace> GameRaces { get; set; }

        /// <summary>Gets or sets the game skills for the current gaming system.</summary>
        public List<GameSkill> GameSkills { get; set; }

        /// <summary>Gets or sets the combat engine to the current gaming system.</summary>
        public ICombat GameCombatEngine { get; set; }

        /// <summary>Gets or sets constructors for the game attributes used by the current gaming system.</summary>
        private List<ConstructorInfo> GameAttributeConstructors { get; set; }

        /// <summary>Gets or sets constructors for the game stats used by the current gaming system.</summary>
        public List<ConstructorInfo> GameStatConstructors { get; set; }

        /// <summary>Gets or sets an imported list of the game attributes used by the current gaming system.</summary>
        [ImportMany]
        private List<Lazy<GameAttribute, ExportGameAttributeAttribute>> ImportedGameAttributes { get; set; }

        /// <summary>Gets or sets an imported list of the genders available to the current gaming system.</summary>
        [ImportMany]
        private List<GameGender> ImportedGameGenders { get; set; }

        /// <summary>Gets or sets an imported list of the game modifiers for the current gaming system.</summary>
        [ImportMany]
        private List<GameModifier> ImportedGameModifiers { get; set; }

        /// <summary>Gets or sets an imported list of the racial templates.</summary>
        [ImportMany]
        private List<GameRace> ImportedGameRaces { get; set; }

        /// <summary>Gets or sets an imported list of the game skills for the current gaming system.</summary>
        [ImportMany]
        private List<GameSkill> ImportedGameSkills { get; set; }

        /// <summary>Gets or sets an imported list of game stats used by the current gaming system.</summary>
        [ImportMany]
        private List<Lazy<GameStat, ExportGameStatAttribute>> ImportedGameStats { get; set; }

        /// <summary>Allows the sub system host to receive an update when subscribed to this system.</summary>
        /// <param name="sender">The sender of the update.</param>
        /// <param name="msg">The message to be sent.</param>
        public void UpdateSubSystemHost(ISubSystem sender, string msg)
        {
            host.UpdateSystemHost(this, msg);
        }

        /// <summary>Subscribes this system to the specified system host, so that host can receive updates.</summary>
        /// <param name="sender">The system host to receive our updates.</param>
        public void SubscribeToSystemHost(ISystemHost sender)
        {
            host = sender;
        }

        /// <summary>Starts this system's individual components.</summary>
        public void Start()
        {
            host.UpdateSystemHost(this, "Starting...");
            Recompose();
            host.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops this system's individual components.</summary>
        public void Stop()
        {
            host.UpdateSystemHost(this, "Stopping");
            host.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Recompose the GameSystemController with the latest components available.</summary>
        public void Recompose()
        {
            lock (this)
            {
                // Recompose the private Imported properties, then prepare a new usable list of each game element,
                // and replace the public list with a new one.  NOTE: we do not modify the existing public lists
                // at any time because they may be actively being iterated by other threads.
                DefaultComposer.Container.ComposeParts(this);

                GameAttributeConstructors = DefaultComposer.GetConstructors(ImportedGameAttributes, NoTypes);
                GameGenders = DefaultComposer.GetNonPrioritizedInstances(ImportedGameGenders);
                GameModifiers = DefaultComposer.GetNonPrioritizedInstances(ImportedGameModifiers);
                GameRaces = DefaultComposer.GetNonPrioritizedInstances(ImportedGameRaces);
                GameSkills = DefaultComposer.GetNonPrioritizedInstances(ImportedGameSkills);
                GameStatConstructors = DefaultComposer.GetConstructors(ImportedGameStats, NoTypes);
            }
        }

        public Dictionary<string, GameAttribute> CloneGameAttributes()
        {
            lock (this)
            {
                return GameAttributeConstructors.Select(ctor => ctor.Invoke(null) as GameAttribute).ToDictionary(a => a.Abbreviation);
            }
        }

        public Dictionary<string, GameStat> CloneGameStats()
        {
            lock (this)
            {
                return GameStatConstructors.Select(ctor => ctor.Invoke(null) as GameStat).ToDictionary(a => a.Abbreviation);
            }
        }

        /// <summary>Exports an instance of the GameSystemController to MEF.</summary>
        [ExportSystem(0)]
        public class GameSystemManagerExporter : SystemExporter, IExportWithPriority
        {
            /// <summary>Gets the singleton system instance.</summary>
            /// <returns>A new instance of the singleton system.</returns>
            public override ISystem Instance => GameSystemController.Instance;

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType => typeof(GameSystemController);

            /// <summary>Gets or sets the priority of the exported system instance. Only the highest priority version gets utilized.</summary>
            /// <remarks>See DefaultComposer for detailed usage information.</remarks>
            public int Priority { get; set; }
        }
    }
}