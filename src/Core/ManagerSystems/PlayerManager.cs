//-----------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   High level manager that provides tracking and global collection of *online* players.
//   Created: August 2006 by Foxedup.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WheelMUD.Core.Events;
    using WheelMUD.Data.RavenDb;
    using WheelMUD.Interfaces;

    /// <summary>High level manager that provides tracking and global collection of online players.</summary>
    /// <remarks>
    /// Following what is still the standard C# event pattern.
    /// http://blogs.msdn.com/b/cburrows/archive/2010/03/30/events-get-a-little-overhaul-in-c-4-afterward-effective-events.aspx
    /// </remarks>
    public class PlayerManager : ManagerSystem
    {
        /// <summary>The singleton instance of this class.</summary>
        private static readonly PlayerManager SingletonInstance = new PlayerManager();

        /// <summary>A list of managed players.</summary>
        private readonly List<PlayerBehavior> playersList = new List<PlayerBehavior>();

        /// <summary>Prevents a default instance of the <see cref="PlayerManager"/> class from being created.</summary>
        private PlayerManager()
        {
        }

        /// <summary>Global player log-in request handler.</summary>
        public static event CancellableGameEventHandler GlobalPlayerLogInRequest;

        /// <summary>Global player log-out request handler.</summary>
        public static event CancellableGameEventHandler GlobalPlayerLogOutRequest;

        /// <summary>Global player log-in event handler.</summary>
        public static event GameEventHandler GlobalPlayerLogInEvent;

        /// <summary>Global player log-out event handler.</summary>
        public static event GameEventHandler GlobalPlayerLogOutEvent;

        /// <summary>Gets the singleton instance of the PlayerManager.</summary>
        /// <value>The instance.</value>
        public static PlayerManager Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Gets a read only collection of the players currently online.</summary>
        public ICollection<PlayerBehavior> Players
        {
            get { return this.playersList.FindAll(p => p.Parent.Name.Length > 0).AsReadOnly(); }
        }

        /// <summary>Prepares the base character.</summary>
        /// <param name="session">The session.</param>
        /// <returns>Filled out base character.</returns>
        public static Thing PrepareBaseCharacter(Session session)
        {
            var movableBehavior = new MovableBehavior();
            var livingBehavior = new LivingBehavior();
            var sensesBehavior = new SensesBehavior();
            var userControlledBehavior = new UserControlledBehavior()
            {
                Controller = session,
            };
            var playerBehavior = new PlayerBehavior(sensesBehavior, userControlledBehavior)
            {
                SessionId = session.ID,
            };

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
        public static void OnPlayerLogIn(Thing player, GameEvent e)
        {
            var eventHandler = GlobalPlayerLogInEvent;
            if (eventHandler != null)
            {
                eventHandler(player.Parent, e);
            }
        }

        /// <summary>Called when a player logs out, to raise the player log out events.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The event arguments.</param>
        public static void OnPlayerLogOut(Thing player, GameEvent e)
        {
            var eventHandler = GlobalPlayerLogOutEvent;
            if (eventHandler != null)
            {
                eventHandler(player.Parent, e);
            }
        }

        /// <summary>Called when a player is trying to log in, to raise the player log in request.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The event arguments.</param>
        public static void OnPlayerLogInRequest(Thing player, CancellableGameEvent e)
        {
            var eventHandler = GlobalPlayerLogInRequest;
            if (eventHandler != null)
            {
                eventHandler(player.Parent, e);
            }
        }

        /// <summary>Called when a player is trying to log out, to raise the player log out request.</summary>
        /// <param name="player">The player.</param>
        /// <param name="e">The event arguments.</param>
        public static void OnPlayerLogOutRequest(Thing player, CancellableGameEvent e)
        {
            var eventHandler = GlobalPlayerLogOutRequest;
            if (eventHandler != null)
            {
                eventHandler(player.Parent, e);
            }
        }

        /// <summary>Finds a player using the predicate passed.</summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The player found.</returns>
        public PlayerBehavior FindPlayer(Predicate<PlayerBehavior> predicate)
        {
            lock (this.lockObject)
            {
                return this.playersList.Find(predicate);
            }
        }

        /// <summary>Finds a player using a name or part name.</summary>
        /// <param name="name">The name of the player to return.</param>
        /// <param name="partialMatch">Used to indicate whether the search criteria can look at just the start of the name.</param>
        /// <returns>The IPlayer found.</returns>
        public Thing FindPlayerByName(string name, bool partialMatch)
        {
            name = name.ToLower();

            lock (this.lockObject)
            {
                PlayerBehavior playerBehavior = this.playersList.Find(p => p.Parent.Name.ToLower().Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (playerBehavior == null && partialMatch)
                {
                    playerBehavior = this.playersList.Find(p => p.Parent.Name.ToLower().StartsWith(name));
                }

                return playerBehavior == null ? null : playerBehavior.Parent;
            }
        }

        /// <summary>Finds the player by their ID.</summary>
        /// <param name="id">Identifier for the player.</param>
        /// <returns>Player object</returns>
        public Thing FindPlayerById(string id)
        {
            lock (this.lockObject)
            {
                PlayerBehavior playerBehavior = this.playersList.Find(p => p.Parent.ID.Equals(id));
                return playerBehavior.Parent;
            }
        }

        /// <summary>Called upon authentication of a session.</summary>
        /// <param name="session">The authenticated session.</param>
        public void OnSessionAuthenticated(Session session)
        {
            // If there was already a connected player for this new, authentic user session, 
            // kick the old one (as it may have been a prior disconnect or whatnot).
            PlayerBehavior previousPlayer = this.FindLoggedInPlayer(session.UserName);
            if (previousPlayer != null)
            {
                var msg = "Duplicate player match, kicking session id " + previousPlayer.SessionId;
                this.SystemHost.UpdateSystemHost(this, msg);

                var existingUserControlledBehavior = previousPlayer.Parent.Behaviors.FindFirst<UserControlledBehavior>();
                if (existingUserControlledBehavior != null)
                {
                    existingUserControlledBehavior.Controller.Write("Another connection has logged in as you; closing this connection.");
                }

                // @@@ TEST: Ensure this closes the connection correctly, etc; used to be rigged 
                //     dangerously directly through the ServerManager.
                previousPlayer.LogOut();
                this.RemovePlayer(previousPlayer.Parent);
            }

            bool wasPlayerMissingDocument = false;

            // If this session doesn't have a player thing attached yet, load it up.  Note that
            // for situations like character creation, we might already have our Thing, so we
            // don't want to load a duplicate version of the just-created player Thing.
            if (session.Thing == null)
            {
                var playerBehavior = new PlayerBehavior();
                playerBehavior.Load(session.UserName);

                var player = new Thing(null)
                {
                    Name = playerBehavior.PlayerData.DisplayName
                };

                // Make sure that the playerBehavior has a parent set.
                playerBehavior.Parent = player;

                // Load game data from disk (RavenDb/NoSQL)
                PlayerDocument pd = this.LoadPlayerDocument(playerBehavior.PlayerData.ID);
                if (pd == null)
                {
                    // If we are here, this means that the player that we are trying to
                    // load does not have a corresponding player document in the NoSQL
                    // (RavenDb) store. Let's go and create a player document with default
                    // values.
                    player = PrepareBaseCharacter(session);
                    player.Name = playerBehavior.PlayerData.DisplayName;
                    playerBehavior.Parent = player;
                    playerBehavior.CreateMissingPlayerDocument();

                    var sb = new StringBuilder();

                    sb.AppendLine("This character is missing gaming data.");
                    sb.AppendFormat("The MUD engine is creating a default game settings for {0}.", playerBehavior.PlayerData.DisplayName);
                    sb.Append(Environment.NewLine);
                    sb.AppendLine("The system will now log you out. Please login again, to continue playing.");

                    session.Write(sb.ToString());

                    playerBehavior.LogOut();
                }

                // Get SensesBehavior and UserControlledBehavior from the PlayerDocument.
                var sensesBehavior = pd.Behaviors.OfType<SensesBehavior>().FirstOrDefault();
                var userControlledBehavior = pd.Behaviors.OfType<UserControlledBehavior>().FirstOrDefault();

                // Setup the controlled behavior controller.
                userControlledBehavior.Controller = session;

                // Initialize the player behavior event processor.
                playerBehavior.InitEventProcessor(sensesBehavior, userControlledBehavior);

                // Get the player behavior with the game data
                var persistedPlayerBehavior = pd.Behaviors.OfType<PlayerBehavior>().FirstOrDefault();

                // Get data from the persisted player behavior and merge it into the manually created one
                playerBehavior.Gender = persistedPlayerBehavior.Gender;
                playerBehavior.Race = persistedPlayerBehavior.Race;
                playerBehavior.SessionId = session.ID;
                playerBehavior.Name = session.UserName;
                playerBehavior.Prompt = pd.PlayerPrompt;
                playerBehavior.RoleData = persistedPlayerBehavior.RoleData;
                playerBehavior.ID = persistedPlayerBehavior.ID;

                // We don't need the persisted player behavior anymore, so remove it
                pd.Behaviors.Remove(persistedPlayerBehavior);

                if (!wasPlayerMissingDocument)
                {
                    // Put all the persisted game data onto the right objects.
                    this.TranslateFromPlayerDocument(ref player, pd);

                    // Make sure to add the player behavior to the player Thing object.
                    playerBehavior.Parent = player;
                    player.Behaviors.Add(playerBehavior);
                    player.ID = "player/" + playerBehavior.ID;
                }

                if (playerBehavior.LogIn(session))
                {
                    lock (this.lockObject)
                    {
                        if (!this.playersList.Contains(playerBehavior))
                        {
                            this.playersList.Add(playerBehavior);
                        }
                    }

                    // Determine the screen buffer size.
                    if (session.Connection != null)
                    {
                        if (userControlledBehavior.PagingRowLimit >= 0)
                        {
                            session.Connection.PagingRowLimit = userControlledBehavior.PagingRowLimit;
                        }
                        else
                        {
                            int terminalHeight = session.Terminal.Height;

                            // If a broken client doesn't provide a valid terminal height, who knows
                            // what it might contain. In that case, default to 0 (no paging).
                            // 100 is an arbitrary realistic number. If this changes, the "buffer"
                            // command should also be changed for consistency. Or define the
                            // max/min as constants somewhere.
                            if (terminalHeight >= 0 && terminalHeight <= 100)
                            {
                                session.Connection.PagingRowLimit = session.Terminal.Height;
                            }
                            else
                            {
                                session.Connection.PagingRowLimit = 0;
                            }
                        }
                    }

                    session.Thing = player;

                    // @@@ HACK: Add player to Krondor's first room
                    PlacesManager.Instance.World.Children[0].Children[0].Add(player);

                    // Finally give the player some initial sensory feedback by having them look.
                    CommandManager.Instance.EnqueueAction(new ActionInput("look", userControlledBehavior.Controller));
                }
                else
                {
                    // @@@ TODO: Login denied? Back out of the session, disconnect, etc.
                    throw new NotImplementedException("Cancellation of login event is not yet supported.");
                }
            }

            // Finally, if the newly-logged in character replaced an old connection, notify the new 
            // user of the problem.  We could also vary behavior/logging based on whether the IP 
            // addresses match; same IP is safer to assume as replaced connection instead of breach.
            if (previousPlayer != null)
            {
                // @@@ TODO: Implement
            }
        }

        /// <summary>Called upon session disconnect.</summary>
        /// <param name="session">The disconnected session.</param>
        public void OnSessionDisconnected(Session session)
        {
            PlayerBehavior playerBehavior = this.playersList.Find(p => p.SessionId.Equals(session.ID));
            if (playerBehavior != null)
            {
                // For now we're just going to log out a player who disconnects.
                // @@@ TODO: Session disconnect should not necessarily remove the player from the world
                //     immediately, as this can be used to avoid imminent death from mobs, etc.  Instead
                //     the player object could linger for a while even uncontrolled before removal.
                playerBehavior.LogOut();
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

            lock (this.lockObject)
            {
                this.playersList.Clear();
            }

            this.SystemHost.UpdateSystemHost(this, "Stopped.");
        }

        /// <summary>Remove the specified player from the PlayerManager.</summary>
        /// <param name="player">The player to remove.</param>
        private void RemovePlayer(Thing player)
        {
            var playerBehavior = player.Behaviors.FindFirst<PlayerBehavior>();
            if (playerBehavior != null)
            {
                lock (this.lockObject)
                {
                    this.playersList.Remove(playerBehavior);
                }
            }
        }

        /// <summary>Load the player document for the specified player ID.</summary>
        /// <param name="databaseId">The database ID of the player.</param>
        /// <returns>The associated PlayerDocument, if there is one, else null.</returns>
        private PlayerDocument LoadPlayerDocument(long databaseId)
        {
            return DocumentManager.LoadPlayerDocument(databaseId);
        }

        /// <summary>Translates from <see cref="PlayerDocument"/> to a <see cref="Thing"/>.</summary>
        /// <param name="player">The player.</param>
        /// <param name="playerDocument">The player document.</param>
        private void TranslateFromPlayerDocument(ref Thing player, PlayerDocument playerDocument)
        {
            foreach (var persistedAsBehavior in playerDocument.Behaviors.ToArray())
            {
                player.Behaviors.Add(persistedAsBehavior as Behavior);
            }

            foreach (var persistedStat in playerDocument.Stats)
            {
                player.Stats.Add(persistedStat.Key, persistedStat.Value as GameStat);
            }

            foreach (var persistedSecondary in playerDocument.SecondaryStats)
            {
                player.Attributes.Add(persistedSecondary.Key, persistedSecondary.Value as GameAttribute);
            }

            foreach (var persistedSkill in playerDocument.Skills)
            {
                player.Skills.Add(persistedSkill.Key, persistedSkill.Value as GameSkill);
            }

            foreach (var persistedChild in playerDocument.SubThings)
            {
                player.Children.Add(persistedChild as Thing);
            }
        }

        /// <summary>Find a logged-in player by user name.</summary>
        /// <param name="userName">The user name to search for.</param>
        /// <returns>The PlayerBehavior of the user, if found, else null.</returns>
        private PlayerBehavior FindLoggedInPlayer(string userName)
        {
            return this.FindPlayer(p => p.Parent.Name.Equals(userName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>Exports an instance of the PlayerManager singleton through MEF.</summary>
        [ExportSystem]
        public class PlayerManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton instance of the <see cref="PlayerManager"/> class.</summary>
            public override ISystem Instance
            {
                get { return PlayerManager.Instance; }
            }

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType
            {
                get { return typeof(PlayerManager); }
            }
        }
    }
}