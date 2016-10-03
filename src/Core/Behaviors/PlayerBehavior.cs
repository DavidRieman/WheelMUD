//-----------------------------------------------------------------------------
// <copyright file="PlayerBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: May 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using Raven.Imports.Newtonsoft.Json;
    using WheelMUD.Core.Events;
    using WheelMUD.Data.Entities;
    using WheelMUD.Data.RavenDb;
    using WheelMUD.Data.Repositories;
    using WheelMUD.Interfaces;

    /// <summary>The behavior for players.</summary>
    public class PlayerBehavior : Behavior
    {
        private readonly object friendsLock = new object();

        /// <summary>Gets the friends of this player.</summary>
        private List<string> friends = new List<string>(); 

        /// <summary>Initializes a new instance of the <see cref="PlayerBehavior"/> class.</summary>
        public PlayerBehavior()
            : base(null)
        {
            this.PlayerData = new PlayerRecord();
        }

        /// <summary>Initializes a new instance of the PlayerBehavior class.</summary>
        /// <param name="sensesBehavior">The senses Behavior.</param>
        /// <param name="userControlledBehavior">The user Controlled Behavior.</param>
        public PlayerBehavior(SensesBehavior sensesBehavior, UserControlledBehavior userControlledBehavior)
            : this()
        {
            this.EventProcessor = new PlayerEventProcessor(this, sensesBehavior, userControlledBehavior);
        }

        /// <summary>Initializes a new instance of the PlayerBehavior class.</summary>
        /// <param name="instanceId">The instance ID.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public PlayerBehavior(long instanceId, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.PlayerData = new PlayerRecord();
            this.ID = instanceId;
        }

        /// <summary>Gets or sets the player's name.</summary>
        /// <value>The name of the player.</value>
        /// <remarks>
        /// This is used to make it easier on external systems that need to set this. 
        /// For example the character creation system is one of those systems.
        /// </remarks>
        [JsonIgnore]
        public string Name 
        {
            get { return this.PlayerData.DisplayName; }
            set { this.PlayerData.DisplayName = value; }
        }

        /// <summary>Gets or sets the current room id.</summary>
        /// <value>The current room id.</value>
        /// <remarks>
        /// This is used to make it easier on external systems that need to set this. 
        /// For example the character creation system is one of those systems.
        /// </remarks>
        [JsonIgnore]
        public int CurrentRoomId { get; set; }

        /// <summary>Gets the player's password.</summary>
        /// <remarks>@@@ TODO: Gets the player's encrypted password.</remarks>
        [JsonIgnore]
        public string Password
        {
            get { return this.PlayerData.Password; }
            private set { this.PlayerData.Password = value; }
        }

        /// <summary>Gets the event processor for this player.</summary>
        [JsonIgnore]
        public PlayerEventProcessor EventProcessor { get; private set; }

        /// <summary>Gets or sets the session ID for this player.</summary>
        [JsonIgnore]
        public string SessionId { get; set; }

        /// <summary>Gets or sets the prompt for this player.</summary>
        public string Prompt { get; set; }

        /// <summary>Gets or sets a value indicating whether this player is AFK.</summary>
        public bool IsAFK { get; set; }

        /// <summary>Gets or sets the reason/why the person is AFK.</summary>
        public string AFKReason { get; set; }

        /// <summary>Gets or sets the date/time when the player when AFK.</summary>
        public DateTime? WhenWentAFK { get; set; }

        /// <summary>Gets or sets the player specific data.</summary>
        [JsonIgnore]
        public PlayerRecord PlayerData { get; set; }

        /// <summary>Gets or sets the roles for this player.</summary>
        [JsonIgnore]
        public List<PlayerRoleRecord> RoleData { get; set; }

        /// <summary>Gets the friends of this player.</summary>
        [JsonIgnore]
        public ReadOnlyCollection<string> Friends
        {
            get { return this.friends.AsReadOnly(); }
        }

        /// <summary>Gets or sets the player's race.</summary>
        public GameRace Race { get; set; }

        /// <summary>Gets or sets the player's gender.</summary>
        public GameGender Gender { get; set; }

        /// <summary>Loads the specified player by their id.</summary>
        /// <param name="playerId">The player's id.</param>
        public void Load(int playerId)
        {
            var repository = new PlayerRepository();
            this.PlayerData = repository.GetById(playerId);
        }

        /// <summary>Loads a player by name.</summary>
        /// <param name="playerName">Name of the player.</param>
        public void Load(string playerName)
        {
            var repository = new PlayerRepository();
            this.PlayerData = repository.GetPlayerByUserName(playerName);
        }

        /// <summary>Saves this player.</summary>
        public override void Save()
        {
            // Save the player's basic info.
            var repository = new PlayerRepository();

            if (this.ID == 0)
            {
                repository.Add(this.PlayerData);
                this.ID = this.PlayerData.ID;
            }
            else
            {
                repository.Update(this.PlayerData);
            }

            /* Disabling roles for now
            // Deal with the player roles.
            var roleRepository = new PlayerRoleRepository();
            ICollection<PlayerRoleRecord> existingRoles = roleRepository.FetchAllPlayerRoleRecordsForPlayer(this.Id);

            var toAdd = new List<PlayerRoleRecord>();

            foreach (var roleRecord in this.RoleData)
            {
                var currRole = this.FindRole(roleRecord.ID, existingRoles);

                if (currRole == null)
                {
                    // Add it to the list to add
                    toAdd.Add(currRole);
                }
                else
                {
                    // Remove the role since there is nothing to do.
                    existingRoles.Remove(currRole);
                }
            }

            roleRepository.AddRolesToPlayer(toAdd);

            // Delete any roles still in the existing roles collection as they
            // are no longer assigned to the person.
            foreach (PlayerRoleRecord existingRole in existingRoles)
            {
                roleRepository.Remove(existingRole);
            }
            */

            // @@@ TODO: Need to do this with all the other custom lists and collections, like friends and inventory.
        }

        /// <summary>Save the whole player Thing (not just this PlayerBehavior).</summary>
        public void SaveWholePlayer()
        {
            var player = this.Parent;

            if (player != null)
            {
                this.Save();

                // Set the behavior id to correspond to the database id of the player.
                // @@@ TODO: Why? Shouldn't Behavior IDs be unique too?
                var playerBehaviors = player.Behaviors.AllBehaviors;
                foreach (var behavior in playerBehaviors)
                {
                    behavior.ID = this.PlayerData.ID;
                }

                // Link our unique player ID and PlayerBehavior ID but in a RavenDB-friendly format.
                player.ID = "player/" + this.PlayerData.ID;

                // Create a PlayerDocument to be saved.
                var bundle = new PlayerDocument
                {
                    DatabaseId = this.PlayerData.ID,
                    Name = player.Name,
                    LastUpdatedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Behaviors = new List<IPersistsWithPlayer>(),
                    Stats = new Dictionary<string, IPersistsWithPlayer>(),
                    SecondaryStats = new Dictionary<string, IPersistsWithPlayer>(),
                    Skills = new Dictionary<string, IPersistsWithPlayer>(),
                    SubThings = new List<IThing>(),
                    PlayerPrompt = this.Prompt,
                };

                bundle.Behaviors.AddRange(playerBehaviors);

                foreach (var stat in player.Stats)
                {
                    bundle.Stats.Add(stat.Key, stat.Value);
                }

                foreach (var attribute in player.Attributes)
                {
                    bundle.SecondaryStats.Add(attribute.Key, attribute.Value);
                }

                foreach (var skill in player.Skills)
                {
                    bundle.Skills.Add(skill.Key, skill.Value);
                }

                foreach (var child in player.Children)
                {
                    // Do not save any player as a sub-document of this one.
                    if (child.Behaviors.FindFirst<PlayerBehavior>() == null)
                    {
                        bundle.SubThings.Add(child);
                    }
                }

                // Save to the document database
                DocumentManager.SavePlayerDocument(bundle); 
            }
        }

        /// <summary>Creates the missing player document in the NoSQL (RavenDb) data store.</summary>
        /// <remarks>
        /// This is usually called in the case where a test character was created in the
        /// relational store, but no player document was created in the NoSQL (RavenDb) data store 
        /// </remarks>
        public void CreateMissingPlayerDocument()
        {
            this.Prompt = ">";
            this.SaveWholePlayer();
        }

        /// <summary>Releases unmanaged and, optionally, managed resources.</summary>
        public void Dispose()
        {
            PlayerManager.GlobalPlayerLogInEvent -= this.ProcessPlayerLogInEvent;
            PlayerManager.GlobalPlayerLogOutEvent -= this.ProcessPlayerLogOutEvent;
            if (this.EventProcessor != null)
            {
                this.EventProcessor.Dispose();
                this.EventProcessor = null;
            }
        }

        /// <summary>Adds the friend.</summary>
        /// <param name="friendName">Name of the friend.</param>
        public void AddFriend(string friendName)
        {
            if (!this.IsFriend(friendName))
            {
                lock (this.friendsLock)
                {
                    this.friends.Add(friendName);
                }
            }
        }

        /// <summary>Removes the friend.</summary>
        /// <param name="friendName">Name of the friend.</param>
        public void RemoveFriend(string friendName)
        {
            if (this.IsFriend(friendName))
            {
                lock (this.friendsLock)
                {
                    this.friends.Remove(friendName);
                }
            }
        }

        /// <summary>Try to log this player into the game.</summary>
        /// <param name="session">The session.</param>
        /// <returns>True if the player successfully logged in.</returns>
        public bool LogIn(Session session)
        {
            var player = this.Parent;

            // Prepare a login request and event.
            var csb = new ContextualStringBuilder(player, player.Parent);
            csb.Append(@"$ActiveThing.Name enters the world.", ContextualStringUsage.WhenNotBeingPassedToOriginator);
            csb.Append(@"You enter the world.", ContextualStringUsage.OnlyWhenBeingPassedToOriginator);
            var message = new SensoryMessage(SensoryType.Sight, 100, csb);
            var e = new PlayerLogInEvent(player, message);

            // Broadcast the login request to the player's current location (IE their parent room), if applicable.
            player.Eventing.OnMiscellaneousRequest(e, EventScope.ParentsDown);

            // Also broadcast the request to any registered global listeners.
            PlayerManager.OnPlayerLogInRequest(player, e);

            // If nothing canceled this event request, carry on with the login.
            if (!e.IsCancelled)
            {
                DateTime universalTime = DateTime.Now.ToUniversalTime();
                this.PlayerData.LastLogin = universalTime.ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";
                this.PlayerData.LastIPAddress = session.Connection.CurrentIPAddress.ToString();

                session.Thing = player;

                // Broadcast that the player successfully logged in, to their login location.
                player.Eventing.OnMiscellaneousEvent(e, EventScope.ParentsDown);
                
                PlayerManager.OnPlayerLogIn(player, e);

                return true;
            }

            return false;
        }

        /// <summary>Builds the player's prompt.</summary>
        /// <returns>The player's current prompt.</returns>
        /// <remarks>
        /// Other game systems should have their own, deriving versions of PlayerBehavior override
        /// this default prompt printer with one which is aware of game-specific details...
        /// </remarks>
        public virtual string BuildPrompt()
        {
            return "> ";
        }

        /// <summary>Try to log this player out of the game.</summary>
        /// <returns>Indicates whether the logout was successful or not.</returns>
        public bool LogOut()
        {
            var player = this.Parent;

            // Prepare a logout request and event.
            var csb = new ContextualStringBuilder(player, player.Parent);
            csb.Append(@"$ActiveThing.Name exits the world.", ContextualStringUsage.WhenNotBeingPassedToOriginator);
            csb.Append(@"You exit the world.", ContextualStringUsage.OnlyWhenBeingPassedToOriginator);
            var message = new SensoryMessage(SensoryType.Sight, 100, csb);
            var e = new PlayerLogOutEvent(player, message);

            // Broadcast the logout request to the player's current location (if applicable).
            player.Eventing.OnMiscellaneousRequest(e, EventScope.ParentsDown);

            // Also broadcast the request to any registered global listeners.
            PlayerManager.OnPlayerLogOutRequest(player, e);

            // If nothing canceled this event request, carry on with the logout.
            if (!e.IsCancelled)
            {
                DateTime universalTime = DateTime.Now.ToUniversalTime();
                this.PlayerData.LastLogout = universalTime.ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";

                player.Save();
                this.Dispose();
                player.Dispose();

                // Broadcast that the player successfully logged out, to their parent (IE room).
                player.Eventing.OnMiscellaneousEvent(e, EventScope.ParentsDown);
                PlayerManager.OnPlayerLogOut(player, e);

                return true;
            }

            return false;
        }

        /// <summary>Sets the players password to the new value specified.</summary>
        /// <param name="newPassword">The new password.</param>
        public void SetPassword(string newPassword)
        {
            // @@@ TODO: Immediately encrypt the password; shouldn't keep it in memory as plain text
            // since a player or builder or whatnot might find a way to read them.
            this.Password = newPassword;
        }

        /// <summary>Check if the password matches.</summary>
        /// <param name="passwordAttempt">The password attempt.</param>
        /// <returns>Whether or not the passwords match.</returns>
        public bool PasswordMatches(string passwordAttempt)
        {
            // @@@ TODO: Encrypt the passed-in password and see if it matches the stored password.
            return this.Password == passwordAttempt;
        }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to this.Parent)</summary>
        public override void OnAddBehavior()
        {
            this.EventProcessor.AttachEvents();
        }

        /// <summary>Initializes a PlayerEventProcessor for this player.</summary>
        /// <param name="sensesBehavior">A valid SensesBehavior which has already been created for this player.</param>
        /// <param name="userControlledBehavior">A valid UserControlledBehavior which has already been created for this player.</param>
        internal void InitEventProcessor(SensesBehavior sensesBehavior, UserControlledBehavior userControlledBehavior)
        {
            this.EventProcessor = new PlayerEventProcessor(this, sensesBehavior, userControlledBehavior);
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.SessionId = null;
            this.friends = new List<string>();

            PlayerManager.GlobalPlayerLogInEvent += this.ProcessPlayerLogInEvent;
            PlayerManager.GlobalPlayerLogOutEvent += this.ProcessPlayerLogOutEvent;
        }

        private void ProcessPlayerLogInEvent(Thing root, GameEvent e)
        {
            // If this is a friend, ensure we get a 'your friend logged in' message regardless of location.
            if (this.IsFriend(e.ActiveThing.Name) && e is PlayerLogInEvent)
            {
                var userControlledBehavior = this.Parent.Behaviors.FindFirst<UserControlledBehavior>();
                string message = string.Format("Your friend {0} has logged in.", e.ActiveThing.Name);
                userControlledBehavior.Controller.Write(message);
            }
        }

        private void ProcessPlayerLogOutEvent(Thing root, GameEvent e)
        {
            // If this is a friend, ensure we get a 'your friend logged out' message regardless of location.
            if (this.IsFriend(e.ActiveThing.Name) && e is PlayerLogOutEvent)
            {
                var userControlledBehavior = this.Parent.Behaviors.FindFirst<UserControlledBehavior>();
                string message = string.Format("Your friend {0} has logged out.", e.ActiveThing.Name);
                userControlledBehavior.Controller.Write(message);
            }
        }

        private bool IsFriend(string friendName)
        {
            lock (this.friendsLock)
            {
                return this.Friends.Contains(friendName.ToLower());
            }
        } 

        /* @@@ This was part of the Quit command, so wouldn't execute if the player was Booted, etc...
         *     probably this should happen as part of PlayerBehavior.Quit()
        /// <summary>Sets the LastLogoutDate for the player, then saves it to the database.</summary>
        /// <param name="sender">Sender of the command.</param>
        private void SavePlayer(IController sender)
        {
            var player = (Player)sender.Thing;
            player.LastLogoutDate = DateTime.Now.ToUniversalTime().ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";
            player.CurrentRoomId = player.CurrentRoom.Id;
            player.Save();
        }*/
    }
}