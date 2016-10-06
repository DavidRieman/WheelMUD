//-----------------------------------------------------------------------------
// <copyright file="SessionManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   High level manager that provides tracking and global collection of all connected sessions.
//   Created: August 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;
    using WheelMUD.Utilities;

    /// <summary>High level manager that provides tracking and global collection of all connected sessions.</summary>
    public class SessionManager : ManagerSystem
    {
        /// <summary>The singleton instance of this class.</summary>
        private static readonly SessionManager SingletonInstance = new SessionManager();

        /// <summary>The login splash screen.</summary>
        ////private static string splash;

        private static List<string> splashScreens = new List<string>();

        /// <summary>Prevents a default instance of the <see cref="SessionManager"/> class from being created.</summary>
        private SessionManager()
        {
            this.Sessions = new Dictionary<string, Session>();
        }

        /// <summary>Gets the singleton instance of the <see cref="SessionManager"/> system.</summary>
        public static SessionManager Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Gets the dictionary of sessions.</summary>
        public Dictionary<string, Session> Sessions { get; private set; }

        /// <summary>Called upon session connection.</summary>
        /// <param name="connection">The connected session.</param>
        public void OnSessionConnected(IConnection connection)
        {
            this.CreateSession(connection);
        }

        /// <summary>Called upon session disconnection.</summary>
        /// <param name="connection">The disconnected session.</param>
        public void OnSessionDisconnected(IConnection connection)
        {
            ////if (SessionDisconnected != null)
            ////    SessionDisconnected(this.Sessions[connection.ID]);#
            lock (this.Sessions)
            {
                if (this.Sessions.ContainsKey(connection.ID))
                {
                    PlayerManager.Instance.OnSessionDisconnected(this.Sessions[connection.ID]);
                }

                this.RemoveSession(connection.ID);
            }
        }

        /// <summary>Called upon session authentication.</summary>
        /// <param name="session">The authenticated session.</param>
        public void OnSessionAuthenticated(Session session)
        {
            session.ActionReceived += this.Controller_ActionReceived;

            this.SystemHost.UpdateSystemHost(this, session.ID + " - Session Authenticated");

            // Tell the player manager about the new authenticated session.
            PlayerManager.Instance.OnSessionAuthenticated(session);
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
            lock (this.Sessions)
            {
                if (this.Sessions.ContainsKey(connection.ID))
                {
                    session = this.Sessions[connection.ID];
                }
            }

            if (session != null)
            {
                session.ProcessCommand(input);
            }
        }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
            this.SystemHost.UpdateSystemHost(this, "Starting...");

            this.SystemHost.UpdateSystemHost(this, "Loading splash screens...");
            this.LoadSplashScreens();
            this.SystemHost.UpdateSystemHost(this, "Done loading splash screens.");

            this.SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            this.SystemHost.UpdateSystemHost(this, "Stopping...");

            lock (this.Sessions)
            {
                this.Sessions.Clear();
            }

            this.SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Gets the splash screen, whether buffered or from file.</summary>
        /// <returns>The contents of the splash screen.</returns>
        private static string GetSplashScreen()
        {
            if (splashScreens.Count == 1)
            {
                return splashScreens[0];
            }

            var random = new Random();
            int fileNum = random.Next(0, splashScreens.Count);

            return splashScreens[fileNum];
        }

        /// <summary>Load the splash screens.</summary>
        private void LoadSplashScreens()
        {
            string name = Configuration.GetDataStoragePath();
            string path = Path.Combine(Path.GetDirectoryName(name), "Files");
            path = Path.Combine(path, "SplashScreens");

            var viewEngine = new ViewEngine();
            viewEngine.AddContext("MudAttributes", MudEngineAttributes.Instance);

            var dirInfo = new DirectoryInfo(path);
            var files = new List<FileInfo>(dirInfo.GetFiles());

            foreach (var fileInfo in files)
            {
                var sr = new StreamReader(fileInfo.FullName);
                string splashContent = sr.ReadToEnd();
                sr.Close();

                string renderedScreen = viewEngine.RenderView(splashContent);

                splashScreens.Add(renderedScreen);
                this.SystemHost.UpdateSystemHost(this, string.Format("{0} has been loaded.", fileInfo.Name));
            }
        }

        /// <summary>Creates a new session for the specified connection.</summary>
        /// <param name="connection">The connection.</param>
        /// <returns>A new Session for the connection.</returns>
        private Session CreateSession(IConnection connection)
        {
            // Negotiate our telnet options.
            connection.TelnetCodeHandler.BeginNegotiation();

            // Load our splash screen.
            connection.Send(GetSplashScreen(), false, true);

            // Create a new session for this connection.
            var session = new Session(connection);

            // Handle our session authenticated event.
            session.SessionAuthenticated += this.OnSessionAuthenticated;

            session.SubscribeToSystem(this);

            // Add the new session to our collection.
            lock (this.Sessions)
            {
                this.Sessions.Add(connection.ID, session);
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
            lock (this.Sessions)
            {
                if (this.Sessions.ContainsKey(sessionID))
                {
                    //// Fire our player unloaded event so that it can be handled by
                    //// the player manager
                    ////if (this.PlayerUnloaded != null)
                    ////    PlayerUnloaded(session.Player);
                    Session session = this.Sessions[sessionID];
                    session.SessionAuthenticated -= this.OnSessionAuthenticated;
                    session.UnsubscribeToSystem();

                    this.Sessions.Remove(sessionID);
                }
            }
        }

        /// <summary>Called upon receiving an action.</summary>
        /// <param name="sender">The sender of the action.</param>
        /// <param name="actionInput">The action input received.</param>
        private void Controller_ActionReceived(IController sender, ActionInput actionInput)
        {
            // @@@ TODO: This session/player should have it's own queue which migrates to the
            // global queue whenever the global queue does not have a pending action from this
            // player.  IE the player should not be able to have two simultaneous actions in
            // progress even though there may be multiple CommandProcessors consuming input.
            // http://www.wheelmud.net/Forums/tabid/59/afv/topic/aff/13/aft/1571/Default.aspx
            CommandManager.Instance.EnqueueAction(actionInput);
        }

        /// <summary>Registers the <see cref="SessionManager"/> system for export.</summary>
        /// <remarks>Assists with non-rebooting updates of the <see cref="SessionManager"/> system through MEF.</remarks>
        [ExportSystem]
        public class SessionManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            public override ISystem Instance
            {
                get { return SessionManager.Instance; }
            }

            /// <summary>Gets the Type of this system.</summary>
            public override Type SystemType
            {
                get { return typeof(SessionManager); }
            }
        }
    }
}