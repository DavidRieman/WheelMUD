//-----------------------------------------------------------------------------
// <copyright file="SessionManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Server.Interfaces;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>High level manager that provides tracking and global collection of all connected sessions.</summary>
    public class SessionManager : ManagerSystem
    {
        /// <summary>Prevents a default instance of the <see cref="SessionManager"/> class from being created.</summary>
        private SessionManager()
        {
            Sessions = new Dictionary<string, Session>();
        }

        /// <summary>Gets the singleton instance of the <see cref="SessionManager"/> system.</summary>
        public static SessionManager Instance { get; } = new SessionManager();

        /// <summary>Gets the dictionary of sessions.</summary>
        public Dictionary<string, Session> Sessions { get; private set; }

        /// <summary>Called upon session connection.</summary>
        /// <param name="connection">The connected session.</param>
        public void OnSessionConnected(IConnection connection)
        {
            CreateSession(connection);
        }

        /// <summary>Called upon session disconnection.</summary>
        /// <param name="connection">The disconnected session.</param>
        public void OnSessionDisconnected(IConnection connection)
        {
            ////if (SessionDisconnected != null)
            ////    SessionDisconnected(Sessions[connection.ID]);#
            lock (Sessions)
            {
                if (Sessions.ContainsKey(connection.ID))
                {
                    PlayerManager.Instance.OnSessionDisconnected(Sessions[connection.ID]);
                }

                RemoveSession(connection.ID);
            }
        }

        /// <summary>Called upon input being received.</summary>
        /// <param name="connection">The connection upon which we received input.</param>
        /// <param name="input">The input that was received.</param>
        public void OnInputReceived(IConnection connection, string input)
        {
            // Currently it is possible to receive input before the session is fully
            // established.  For now, ignore any such early input and avoid locking 
            // the sessions collection for the duration of the command processing.
            Session session = null;
            lock (Sessions)
            {
                if (Sessions.ContainsKey(connection.ID))
                {
                    session = Sessions[connection.ID];
                }
            }

            session?.ProcessCommand(input);
        }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");

            SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            lock (Sessions)
            {
                Sessions.Clear();
            }

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Creates a new session for the specified connection.</summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A new Session for the connection.</returns>
        private Session CreateSession(IConnection connection)
        {
            // Negotiate our telnet options.
            connection.TelnetCodeHandler.BeginNegotiation();

            // Create a new session for this connection.
            var session = new Session(connection);

            // Load our splash screen.
            session.Write(Renderer.Instance.RenderSplashScreen(session.TerminalOptions), true);

            session.SubscribeToSystem(this);

            // Add the new session to our collection.
            lock (Sessions)
            {
                Sessions.Add(connection.ID, session);
            }

            return session;
        }

        /// <summary>Removes the specified session.</summary>
        /// <param name="sessionID">The ID of the session.</param>
        private void RemoveSession(string sessionID)
        {
            // We remove the session from the collection, but we also
            // have to trigger the player unloaded event as the session
            // object is not aware of being unloaded.
            lock (Sessions)
            {
                if (Sessions.ContainsKey(sessionID))
                {
                    //// Fire our player unloaded event so that it can be handled by
                    //// the player manager
                    ////if (PlayerUnloaded != null)
                    ////    PlayerUnloaded(session.Player);
                    Session session = Sessions[sessionID];
                    session.UnsubscribeToSystem();
                    Sessions.Remove(sessionID);
                }
            }
        }

        /// <summary>Registers the <see cref="SessionManager"/> system for export.</summary>
        /// <remarks>Assists with non-rebooting updates of the <see cref="SessionManager"/> system through MEF.</remarks>
        [ExportSystem(0)]
        public class SessionManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            public override ISystem Instance => SessionManager.Instance;

            /// <summary>Gets the Type of this system.</summary>
            public override Type SystemType => typeof(SessionManager);
        }
    }
}