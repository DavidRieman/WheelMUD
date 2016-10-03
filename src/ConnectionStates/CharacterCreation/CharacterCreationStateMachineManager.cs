//-----------------------------------------------------------------------------
// <copyright file="CharacterCreationStateMachineManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The character creation state machine manager.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.ConnectionStates
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using WheelMUD.Core;
    using WheelMUD.Data.Entities;
    using WheelMUD.Interfaces;

    /// <summary>A delegate for the completion of character creation.</summary>
    /// <param name="data">The player data structure.</param>
    public delegate void CharacterCreationCompleted(Thing data);

    /// <summary>A delegate for the abortion of character creation.</summary>
    public delegate void CharacterCreationAborted();

    /// <summary>A delegate for the prompt change of character creation.</summary>
    /// <param name="prompt">The new prompt.</param>
    public delegate void CharacterCreationChangePrompt(string prompt);

    /// <summary>The character creation handler.</summary>
    public class CharacterCreationStateMachineManager : IRecomposable
    {
        /// <summary>The synchronization locking object.</summary>
        private static readonly object LockObject = new object();

        /// <summary>The SessionStateManager singleton instance.</summary>
        private static readonly CharacterCreationStateMachineManager SingletonInstance = new CharacterCreationStateMachineManager();
        
        /// <summary>The default character creation state machine constructor, as identified during composition.</summary>
        private ConstructorInfo defaultCharacterCreationStateMachineConstructor;

        /// <summary>Prevents a default instance of the <see cref="CharacterCreationStateMachineManager"/> class from being created.</summary>
        private CharacterCreationStateMachineManager()
        {
            this.Recompose();
        }
        
        /// <summary>Gets the singleton instance of the <see cref="CharacterCreationStateMachineManager"/> class.</summary>
        public static CharacterCreationStateMachineManager Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Gets, via MEF composition, an enumerable collection of available state machine classes.</summary>
        [ImportMany]
        public Lazy<CharacterCreationStateMachine, ExportCharacterCreationStateMachineAttribute>[] CharacterCreationStateMachines 
        {
            get;
            private set;
        }

        /// <summary>Creates the default character creation state machine.</summary>
        /// <param name="session">The session.</param>
        /// <returns>The CharacterCreationStateMachine.</returns>
        public CharacterCreationStateMachine CreateDefaultCharacterCreationStateMachine(Session session)
        {
            // Lock to prevent using the defaultSessionStateConstructor while it is being recomposed, etc.
            lock (LockObject)
            {
                var parameters = new object[] { session };
                return (CharacterCreationStateMachine)this.defaultCharacterCreationStateMachineConstructor.Invoke(parameters);
            }
        }

        /// <summary>Recompose the subcomponents of this CharacterCreationStateMachineManager.</summary>
        public void Recompose()
        {
            lock (LockObject)
            {
                DefaultComposer.Container.ComposeParts(this);

                // Search the CharacterCreationStateMachines for the one which has the highest priority.
                // @@@ TODO: assembly version number could be used as orderby tiebreaker to help ensure
                //     "latest" is always prioritized over an equal priority of an older version.
                var defaultStateMachineType = (from s in this.CharacterCreationStateMachines
                                               orderby s.Metadata.StateMachinePriority descending
                                               select s.Value.GetType()).First();

                // Find the constructor of that type which takes a Session.  We'll use this info to 
                // quickly create the default CharacterCreationStateMachines for character creators.
                var constructorTypes = new Type[] { typeof(Session) };
                this.defaultCharacterCreationStateMachineConstructor = defaultStateMachineType.GetConstructor(constructorTypes);
            }
        }
    }
}