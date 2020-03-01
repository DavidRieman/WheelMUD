//-----------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

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
                lock (this.playersList)
                {
                    return this.playersList.FindAll(p => p.Parent.Name.Length > 0).AsReadOnly();
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
            var userControlledBehavior = new UserControlledBehavior() { Controller = session };
            var playerBehavior = new PlayerBehavior() { SessionId = session.ID };
            var player = new Thing(livingBehavior, sensesBehavior, userControlledBehavior, playerBehavior, movableBehavior);
            var game = GameSystemController.Instance;

            // Load the default stats for the current gaming system
            foreach (var gameStat in game.GameStats)
            {
                var currStat = new GameStat(session, gameStat.Name, gameStat.Abbreviation, gameStat.Formula, gameStat.Value, gameStat.MinValue, gameStat.MaxValue, gameStat.Visible);
                player.Stats.Add(currStat.Abbreviation, currStat);
            }

            // Load the secondary stats\attributes for the current gaming system
            foreach (var attribute in game.GameAttributes)
            {
                var newAttribute = new GameAttribute(session, attribute.Name, attribute.Abbreviation, attribute.Formula, attribute.Value, attribute.MinValue, attribute.MaxValue, attribute.Visible);
                player.Attributes.Add(newAttribute.Abbreviation, newAttribute);
            }

            return player;
        }

        /// <summary>Called when a player logs in, to raise the player log in events.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.GameEvent"/> instance containing the event data.</param>
        public void OnPlayerLogIn(Thing player, GameEvent e)
        {
            this.GlobalPlayerLogInEvent?.Invoke(player.Parent, e);
        }

        /// <summary>Called when a player logs out, to raise the player log out events.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The event arguments.</param>
        public void OnPlayerLogOut(Thing player, GameEvent e)
        {
            this.GlobalPlayerLogOutEvent?.Invoke(player.Parent, e);
            this.RemovePlayer(player);
        }

        /// <summary>Called when a player is trying to log in, to raise the player log in request.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The event arguments.</param>
        public void OnPlayerLogInRequest(Thing player, CancellableGameEvent e)
        {
            this.GlobalPlayerLogInRequest?.Invoke(player.Parent, e);
        }

        /// <summary>Called when a player is trying to log out, to raise the player log out request.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The event arguments.</param>
        public void OnPlayerLogOutRequest(Thing player, CancellableGameEvent e)
        {
            this.GlobalPlayerLogOutRequest?.Invoke(player.Parent, e);
        }

        /// <summary>Finds a player using the predicate passed.</summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The player found.</returns>
        public PlayerBehavior FindPlayer(Predicate<PlayerBehavior> predicate)
        {
            lock (this.playersList)
            {
                return this.playersList.Find(predicate);
            }
        }

        /// <summary>Finds a loaded player using a name or part name.</summary>
        /// <param name="name">The name of the player to return.</param>
        /// <param name="allowPartialMatch">Used to indicate whether the search criteria can look at just the start of the name.</param>
        /// <returns>The player Thing found, or null if not found.</returns>
        public Thing FindLoadedPlayerByName(string name, bool allowPartialMatch)
        {
            name = name.ToLower();
            var playerBehavior = this.FindPlayer(p => p.Parent.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (playerBehavior == null && allowPartialMatch)
            {
                playerBehavior = this.FindPlayer(p => p.Parent.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
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
            PlayerBehavior previousPlayer = this.FindLoggedInPlayer(session.User.UserName);
            if (previousPlayer != null)
            {
                var msg = $"Duplicate player match, kicking session id {previousPlayer.SessionId} and keeping {session.ID}";
                this.SystemHost.UpdateSystemHost(this, msg);

                var existingUserControlledBehavior = previousPlayer.Parent.Behaviors.FindFirst<UserControlledBehavior>();
                if (existingUserControlledBehavior != null)
                {
                    existingUserControlledBehavior.Controller.Write("Another connection has logged in as you; closing this connection.");
                }

                previousPlayer.LogOut();
                this.RemovePlayer(previousPlayer.Parent);
            }

            // Track this player in the loaded players list.
            this.AddPlayer(session.Thing);

            // If this session doesn't have a player thing attached yet, load it up.  Note that
            // for situations like character creation, we might already have our Thing, so we
            // don't want to load a duplicate version of the just-created player Thing.
            if (session.Thing == null)
            {
                session.Write("User was authenticated but the player character could not be loaded.");
                session.Write("Please contact an administrator. Disconnecting.");
                session.Connection.Disconnect();
            }

            // TODO: Perhaps reset player command queue to have exactly one "look" command?
        }

        /// <summary>Called upon session disconnect.</summary>
        /// <param name="session">The disconnected session.</param>
        public void OnSessionDisconnected(Session session)
        {
            // For now we're just going to immediately log out a player who disconnects.
            // TODO: Session disconnect should not necessarily remove the player from the world immediately, as
            //       this could be used to cheat out of imminent death, etc.  Instead the player object should
            //       linger around a while, uncontrolled, before removal.  Mark the player as "linkdead" though.
            if (session.Thing != null)
            {
                var playerBehavior = session.Thing.FindBehavior<PlayerBehavior>();
                if (playerBehavior != null && playerBehavior.SessionId != null)
                {
                    playerBehavior.LogOut();
                    playerBehavior.SessionId = null;
                }
            }
        }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
            this.SystemHost.UpdateSystemHost(this, "Starting...");
            this.SystemHost.UpdateSystemHost(this, "Started.");
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            this.SystemHost.UpdateSystemHost(this, "Stopping...");

            lock (this.playersList)
            {
                this.playersList.Clear();
            }

            this.SystemHost.UpdateSystemHost(this, "Stopped.");
        }

        private void AddPlayer(Thing player)
        {
            var playerBehavior = player.FindBehavior<PlayerBehavior>();
            lock (this.playersList)
            {
                if (playerBehavior != null && !this.playersList.Contains(playerBehavior))
                {
                    this.playersList.Add(playerBehavior);
                }
            }
        }

        /// <summary>Remove the specified player from the PlayerManager.</summary>
        /// <param name="player">The player to remove.</param>
        private void RemovePlayer(Thing player)
        {
            var playerBehavior = player.Behaviors.FindFirst<PlayerBehavior>();
            if (playerBehavior != null)
            {
                lock (this.playersList)
                {
                    this.playersList.Remove(playerBehavior);
                }
            }
        }

        /// <summary>Find a logged-in player by user name.</summary>
        /// <param name="userName">The user name to search for.</param>
        /// <returns>The PlayerBehavior of the user, if found, else null.</returns>
        private PlayerBehavior FindLoggedInPlayer(string userName)
        {
            // TODO: #62: Find via user name instead of player names which match this user name.
            return this.FindPlayer(p => p.Parent.Name.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
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