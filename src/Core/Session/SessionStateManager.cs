//-----------------------------------------------------------------------------
// <copyright file="SessionStateManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using WheelMUD.Interfaces;

    /// <summary>Manages session states.</summary>
    public class SessionStateManager : IRecomposable
    {
        /// <summary>The synchronization locking object.</summary>
        private static readonly object LockObject = new object();

        /// <summary>The SessionStateManager singleton instance.</summary>
        private static readonly SessionStateManager SingletonInstance = new SessionStateManager();

        /// <summary>The current default session state constructor (as found by MEF).</summary>
        private ConstructorInfo defaultSessionStateConstructor;

        private SessionStateManager()
        {
            this.Recompose();
        }

        /// <summary>Gets the SessionStateManager singleton instance.</summary>
        public static SessionStateManager Instance
        {
            get { return SingletonInstance; }
        }

        [ImportMany]
        public Lazy<SessionState, ExportSessionStateAttribute>[] SessionStates { get; set; }

        public SessionState CreateDefaultState(Session session)
        {
            // Lock to prevent using the defaultSessionStateConstructor while it is being recomposed, etc.
            lock (LockObject)
            {
                var parameters = new object[] { session };
                return (SessionState)this.defaultSessionStateConstructor.Invoke(parameters);
            }
        }

        public void Recompose()
        {
            lock (LockObject)
            {
                DefaultComposer.Container.ComposeParts(this);

                // Search the SessionStates for the one which has the highest priority.
                // @@@ TODO: assembly version number could be used as orderby tiebreaker to help ensure
                //     "latest" is always prioritized over an equal StatePriority of an older version.
                Type defaultSessionStateType;
                if (this.SessionStates.Length > 0)
                {
                    defaultSessionStateType = (from s in this.SessionStates
                                               orderby s.Metadata.StatePriority descending
                                               select s.Value.GetType()).First();
                }
                else
                {
                    defaultSessionStateType = typeof(DefaultState);
                }

                // Find the constructor of that type which takes a Session.  We'll use this info to 
                // quickly create the default SessionStates for newly connected sessions.
                var constructorTypes = new Type[] { typeof(Session) };
                this.defaultSessionStateConstructor = defaultSessionStateType.GetConstructor(constructorTypes);
            }
        }
    }
}