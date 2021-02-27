//-----------------------------------------------------------------------------
// <copyright file="CharacterCreationStateMachineManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.Reflection;
using WheelMUD.Core;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.ConnectionStates
{
    /// <summary>A delegate for the completion of character creation.</summary>
    public delegate void CharacterCreationCompleted(Session session);

    /// <summary>A delegate for the abortion of character creation.</summary>
    public delegate void CharacterCreationAborted();

    /// <summary>A delegate for the prompt change of character creation.</summary>
    /// <param name="prompt">The new prompt.</param>
    public delegate void CharacterCreationChangePrompt(string prompt);

    /// <summary>The character creation state machine handler.</summary>
    public class CharacterCreationStateMachineManager : IRecomposable
    {
        /// <summary>The synchronization locking object.</summary>
        private static readonly object LockObject = new object();

        /// <summary>The default character creation state machine constructor, as identified during composition.</summary>
        private ConstructorInfo defaultCharacterCreationStateMachineConstructor;

        /// <summary>Prevents a default instance of the <see cref="CharacterCreationStateMachineManager"/> class from being created.</summary>
        private CharacterCreationStateMachineManager()
        {
            Recompose();
        }

        /// <summary>Gets the singleton instance of the <see cref="CharacterCreationStateMachineManager"/> class.</summary>
        public static CharacterCreationStateMachineManager Instance { get; } = new CharacterCreationStateMachineManager();

        /// <summary>Gets, via MEF composition, an enumerable collection of available state machine classes.</summary>
        [ImportMany]
        public Lazy<CharacterCreationStateMachine, ExportCharacterCreationStateMachineAttribute>[] CharacterCreationStateMachines { get; private set; }

        /// <summary>Creates the default character creation state machine.</summary>
        /// <param name="session">The session.</param>
        /// <returns>The CharacterCreationStateMachine.</returns>
        public CharacterCreationStateMachine CreateDefaultCharacterCreationStateMachine(Session session)
        {
            // Lock to prevent using the defaultSessionStateConstructor while it is being recomposed, etc.
            lock (LockObject)
            {
                var parameters = new object[] { session };
                return (CharacterCreationStateMachine)defaultCharacterCreationStateMachineConstructor.Invoke(parameters);
            }
        }

        /// <summary>Recompose the subcomponents of this CharacterCreationStateMachineManager.</summary>
        public void Recompose()
        {
            lock (LockObject)
            {
                DefaultComposer.Container.ComposeParts(this);

                // Find the constructor of the current priority character creation state machine that takes a Session parameter.
                // We'll use this info to quickly create each CharacterCreationStateMachines instance for new character creation sessions.
                var constructorTypes = new Type[] { typeof(Session) };
                defaultCharacterCreationStateMachineConstructor = DefaultComposer.GetConstructor(CharacterCreationStateMachines, constructorTypes);
            }
        }
    }
}