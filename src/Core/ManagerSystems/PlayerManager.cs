//-----------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using WheelMUD.Server;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>High level manager that provides tracking and global collection of online (and linkdead but still-loaded) players.</summary>
    public class PlayerManager : ManagerSystem
    {
        /// <summary>A list of managed, loaded players.</summary>
        private readonly List<PlayerBehavior> playersList = new List<PlayerBehavior>();

        /// <summary>Prevents a default instance of the <see cref="PlayerManager"/> class from being created.</summary>
        private PlayerManager()
        {
        }

        /// <summary>Global player log-in request handler.</summary>
        public event CancellableGameEventHandler GlobalPlayerLogInRequest;

        /// <summary>Global player log-out request handler.</summary>
        public event CancellableGameEventHandler GlobalPlayerLogOutRequest;

        /// <summary>Global player log-in event handler.</summary>
        public event GameEventHandler GlobalPlayerLogInEvent;

        /// <summary>Global player log-out event handler.</summary>
        public event GameEventHandler GlobalPlayerLogOutEvent;

        /// <summary>Gets the singleton instance of the PlayerManager.</summary>
        /// <value>The instance.</value>
        public static PlayerManager Instance { get; } = new PlayerManager();

        /// <summary>Gets a read only collection of the players currently online.</summary>
        public ICollection<PlayerBehavior> Players
        {
            get
            {
                lock (playersList)
                {
                    return playersList.FindAll(p => p.Parent.Name.Length > 0).AsReadOnly();
                }
            }
        }

        /// <summary>Prepares the base character.</summary>
        /// <param name="session">The session.</param>
        /// <returns>Filled out base character.</returns>
        public static Thing PrepareBaseCharacter(Session session)
        {
            var movableBehavior = new MovableBehavior();
            var livingBehavior = new LivingBehavior();
            var sensesBehavior = new SensesBehavior();
            // TODO: Most characters should start as just tutorialPlayer or player role, unless FirstCreatedCharacterIsAdmin
            //       is set and there is no character in the DB yet. See: https://github.com/DavidRieman/WheelMUD/issues/39
            var userControlledBehavior = new UserControlledBehavior()
            {
                Session = session,
                SecurityRoles = SecurityRole.player | SecurityRole.helper | SecurityRole.minorBuilder | SecurityRole.fullBuilder | SecurityRole.minorAdmin | SecurityRole.fullAdmin
            };
            var playerBehavior = new PlayerBehavior() { SessionId = session.ID };
            var player = new Thing(livingBehavior, sensesBehavior, userControlledBehavior, playerBehavior, movableBehavior);
            var game = GameSystemController.Instance;

            // Load a fresh set of stats and attributes classes for the current gaming system.
            player.Attributes = game.CloneGameAttributes();
            player.Stats = game.CloneGameStats();

            return player;
        }

        /// <summary>Called when a player logs in, to raise the player log in events.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.GameEvent"/> instance containing the event data.</param>
        public void OnPlayerLogIn(Thing player, GameEvent e)
        {
            GlobalPlayerLogInEvent?.Invoke(player.Parent, e);
        }

        /// <summary>Called when a player logs out, to raise the player log out events.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The event arguments.</param>
        public void OnPlayerLogOut(Thing player, GameEvent e)
        {
            GlobalPlayerLogOutEvent?.Invoke(player.Parent, e);
            RemovePlayer(player);
        }

        /// <summary>Called when a player is trying to log in, to raise the player log in request.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The event arguments.</param>
        public void OnPlayerLogInRequest(Thing player, CancellableGameEvent e)
        {
            GlobalPlayerLogInRequest?.Invoke(player.Parent, e);
        }

        /// <summary>Called when a player is trying to log out, to raise the player log out request.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The event arguments.</param>
        public void OnPlayerLogOutRequest(Thing player, CancellableGameEvent e)
        {
            GlobalPlayerLogOutRequest?.Invoke(player.Parent, e);
        }

        /// <summary>Finds a player using the predicate passed.</summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The player found.</returns>
        public PlayerBehavior FindPlayer(Predicate<PlayerBehavior> predicate)
        {
            lock (playersList)
            {
                return playersList.Find(predicate);
            }
        }

        /// <summary>Finds a loaded player using a name or part name.</summary>
        /// <param name="name">The name of the player to return.</param>
        /// <param name="allowPartialMatch">Used to indicate whether the search criteria can look at just the start of the name.</param>
        /// <returns>The player Thing found, or null if not found.</returns>
        public Thing FindLoadedPlayerByName(string name, bool allowPartialMatch)
        {
            name = name.ToLower();
            var playerBehavior = FindPlayer(p => p.Parent.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (playerBehavior == null && allowPartialMatch)
            {
                playerBehavior = FindPlayer(p => p.Parent.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
            }

            return playerBehavior?.Parent;
        }

        /// <summary>Called upon authentication of a session.</summary>
        /// <param name="session">The authenticated session.</param>
        public void OnSessionAuthenticated(Session session)
        {
            // If there was already a connected player for this new, authentic user session, kick the old
            // one (as it may have been a prior disconnect or whatnot or even a different player character
            // controlled by the same user account).
            // TODO: We can probably handle this more gracefully (without the extra logouts and messaging
            //       the room about it and so on) by having the login process notice the target player is
            //       already in the world sooner, and more directly taking fresh control of THAT Thing.
            PlayerBehavior previousPlayer = FindLoggedInPlayer(session.User.UserName);
            if (previousPlayer != null)
            {
                var msg = $"Duplicate player match, replacing session {previousPlayer.SessionId} with new session {session.ID}";
                SystemHost.UpdateSystemHost(this, msg);

                var previousUser = previousPlayer.Parent.FindBehavior<UserControlledBehavior>();
                Debug.Assert(previousUser != null, "Existing Player found must always also be a UserControlled Thing.");
                previousUser.Disconnect("Another connection has logged in as you; closing this connection.");

                previousUser.Session = session;
                previousPlayer.SessionId = session.ID;
                session.Thing = previousPlayer.Parent;
            }
            else
            {
                // Track this new player in the loaded players list.
                AddPlayer(session.Thing);
            }

            // A freshly (re)connected player is assumed not to be AFK anymore.
            PlayerBehavior playerBehavior = session.Thing.FindBehavior<PlayerBehavior>();
            if (playerBehavior != null)
            {
                playerBehavior.IsAFK = false;
                playerBehavior.AFKReason = null;
            }

            // If this session doesn't have a player thing attached yet, load it up.  Note that
            // for situations like character creation, we might already have our Thing, so we
            // don't want to load a duplicate version of the just-created player Thing.
            if (session.Thing == null)
            {
                var output = new OutputBuilder();
                output.AppendLine("User was authenticated but the player character could not be loaded.");
                output.AppendLine("Please contact an administrator. Disconnecting.");
                session.Write(output);
                session.Connection.Disconnect();
            }

            // TODO: Perhaps reset player command queue to have exactly one "look" command?
        }

        /// <summary>Called upon session disconnect.</summary>
        /// <param name="session">The disconnected session.</param>
        public void OnSessionDisconnected(Session session)
        {
            // For now we'll just set the player to an automatic "AFK" state.
            // TODO: Track known link-death state separately. (Note that not every socket will notice link-death imminently.)
            if (session.Thing != null)
            {
                var playerBehavior = session.Thing.FindBehavior<PlayerBehavior>();
                if (playerBehavior != null && playerBehavior.SessionId != null)
                {
                    playerBehavior.IsAFK = true;
                    playerBehavior.AFKReason = "Disconnect detected.";
                }
            }
        }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");
            SystemHost.UpdateSystemHost(this, "Started.");
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            lock (playersList)
            {
                playersList.Clear();
            }

            SystemHost.UpdateSystemHost(this, "Stopped.");
        }

        private void AddPlayer(Thing player)
        {
            var playerBehavior = player.FindBehavior<PlayerBehavior>();
            lock (playersList)
            {
                if (playerBehavior != null && !playersList.Contains(playerBehavior))
                {
                    playersList.Add(playerBehavior);
                }
            }
        }

        /// <summary>Remove the specified player from the PlayerManager.</summary>
        /// <param name="player">The player to remove.</param>
        private void RemovePlayer(Thing player)
        {
            var playerBehavior = player.FindBehavior<PlayerBehavior>();
            if (playerBehavior != null)
            {
                lock (playersList)
                {
                    playersList.Remove(playerBehavior);
                }
            }
        }

        /// <summary>Find a logged-in player by user name.</summary>
        /// <param name="userName">The user name to search for.</param>
        /// <returns>The PlayerBehavior of the user, if found, else null.</returns>
        private PlayerBehavior FindLoggedInPlayer(string userName)
        {
            // TODO: #62: Find via user name instead of player names which match this user name.
            return FindPlayer(p => p.Parent.Name.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>Exports an instance of the PlayerManager singleton through MEF.</summary>
        [ExportSystem(0)]
        public class PlayerManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton instance of the <see cref="PlayerManager"/> class.</summary>
            public override ISystem Instance => PlayerManager.Instance;

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType => typeof(PlayerManager);
        }
    }
}