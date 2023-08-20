//-----------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Data.Repositories;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>High level manager that provides tracking and global collection of online (and linkdead but still-loaded) players.</summary>
    public class PlayerManager : ManagerSystem
    {
        /// <summary>A list of managed, loaded players.</summary>
        private readonly List<PlayerBehavior> playersList = new();

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
            AddPlayer(player);
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

        /// <summary>Attach the specified player ID to the specified, already-authenticated session.</summary>
        /// <param name="characterId">The ID of the player character to attach.</param>
        /// <param name="session">The session to attach the player character to.</param>
        /// <returns>True if successfully attached (or reattached), else false.</returns>
        /// <remarks>TODO: Perhaps also reset player command queue to have exactly one "look" command?</remarks>
        public bool AttachPlayerToSession(string characterId, Session session)
        {
            PlayerBehavior existingPlayer;
            lock (lockObject)
            {
                existingPlayer = playersList.Where(p => p.Parent?.Id == characterId).FirstOrDefault();
            }
            var userControlled = existingPlayer?.Parent?.FindBehavior<UserControlledBehavior>();

            // If the target player isn't already in the world, load the player and make them user-controlled.
            if (existingPlayer == null)
            {
                session.Thing = DocumentRepository<Thing>.Load(characterId);
                session.Thing.RepairParentTree();
                existingPlayer = session.Thing?.FindBehavior<PlayerBehavior>();
                userControlled = session.Thing?.FindBehavior<UserControlledBehavior>();
                if (existingPlayer == null || userControlled == null)
                {
                    session.WriteLine("This character player state is broken. You may need to contact an administrator for a possible recovery attempt.");
                    session.InformSubscribedSystem(session.ID + " failed to load due to missing Thing or core Behavior.");
                    session.Connection.Disconnect();
                    return false;
                }
                return existingPlayer.LogIn(session);
            }

            // If this player is already associated with this session somehow, we're already done.
            if (userControlled?.Session == session)
            {
                return true;
            }

            // If this player is already in this world but associated with a different session, kick the
            // existing session and assign this session to the player. (Freshest login wins, as it may be
            // a "reconnect" where the old session is just pending link-death.)
            if (userControlled?.Session != null)
            {
                SystemHost.UpdateSystemHost(this, $"New session {session.ID} replacing old session for {characterId}.");
                userControlled.Disconnect("Another connection has logged in as you; closing this connection.");
                userControlled.Session = session;
                session.Thing = userControlled.Parent;
                existingPlayer.ClearAFK();
                return true;
            }

            throw new Exception("Could not attach player to session; Unexpected scenario?");
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
                if (playerBehavior != null)
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

        /// <summary>Exports an instance of the PlayerManager singleton through MEF.</summary>
        [CoreExports.System(0)]
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