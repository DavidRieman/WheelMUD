//-----------------------------------------------------------------------------
// <copyright file="PlayerBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using WheelMUD.Data.Entities;
using WheelMUD.Data.Repositories;
using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
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
            PlayerData = new PlayerRecord();
        }

        /// <summary>Initializes a new instance of the PlayerBehavior class.</summary>
        /// <param name="instanceId">The instance ID.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public PlayerBehavior(long instanceId, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            PlayerData = new PlayerRecord();
            ID = instanceId;
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
            get { return PlayerData.DisplayName; }
            set { PlayerData.DisplayName = value; }
        }

        /// <summary>Gets or sets the current room id.</summary>
        /// <value>The current room id.</value>
        /// <remarks>
        /// This is used to make it easier on external systems that need to set this. 
        /// For example the character creation system is one of those systems.
        /// </remarks>
        [JsonIgnore]
        public int CurrentRoomId { get; set; }

        /// <summary>Gets the event processor for this player.</summary>
        [JsonIgnore]
        public PlayerEventProcessor EventProcessor { get; private set; }

        /// <summary>Gets or sets the session ID for this player.</summary>
        [JsonIgnore]
        public string SessionId { get; set; }

        /// <summary>Gets or sets the prompt for this player.</summary>
        public string Prompt { get; set; }

        /// <summary>Gets or sets a value indicating whether this player is AFK.</summary>
        [JsonIgnore]
        public bool IsAFK { get; set; }

        /// <summary>Gets or sets the reason/why the person is AFK.</summary>
        [JsonIgnore]
        public string AFKReason { get; set; }

        /// <summary>Gets or sets the date/time when the player when AFK.</summary>
        public DateTime? WhenWentAFK { get; set; }

        /// <summary>Gets or sets the player specific data.</summary>
        /// <remarks>TODO: Migrate some of this data to be included in DB or otherwise saved? We're not persisting last logout time etc.</remarks>
        [JsonIgnore]
        public PlayerRecord PlayerData { get; set; }

        /// <summary>Gets or sets the roles for this player.</summary>
        [JsonIgnore]
        public List<PlayerRoleRecord> RoleData { get; set; }

        /// <summary>Gets the friends of this player.</summary>
        [JsonIgnore]
        public ReadOnlyCollection<string> Friends
        {
            get { return friends.AsReadOnly(); }
        }

        /// <summary>Gets or sets the player's race.</summary>
        public GameRace Race { get; set; }

        /// <summary>Gets or sets the player's gender.</summary>
        public GameGender Gender { get; set; }

        /// <summary>Save the whole player Thing (not just this PlayerBehavior).</summary>
        public void SavePlayer()
        {
            var player = Parent;
            if (player == null)
            {
                throw new ArgumentNullException("Cannot save a detached PlayerBehavior with no Parent Thing.");
            }

            DocumentRepository<Thing>.Save(player);
        }

        /// <summary>Releases unmanaged and, optionally, managed resources.</summary>
        public void Dispose()
        {
            PlayerManager.Instance.GlobalPlayerLogInEvent -= ProcessPlayerLogInEvent;
            PlayerManager.Instance.GlobalPlayerLogOutEvent -= ProcessPlayerLogOutEvent;
            if (EventProcessor != null)
            {
                EventProcessor.Dispose();
                EventProcessor = null;
            }
        }

        /// <summary>Adds the friend.</summary>
        /// <param name="friendName">Name of the friend.</param>
        public void AddFriend(string friendName)
        {
            if (!IsFriend(friendName))
            {
                lock (friendsLock)
                {
                    friends.Add(friendName);
                }
            }
        }

        /// <summary>Removes the friend.</summary>
        /// <param name="friendName">Name of the friend.</param>
        public void RemoveFriend(string friendName)
        {
            if (IsFriend(friendName))
            {
                lock (friendsLock)
                {
                    friends.Remove(friendName);
                }
            }
        }

        /// <summary>Try to log this player into the game.</summary>
        /// <param name="session">The session.</param>
        /// <returns>True if the player successfully logged in.</returns>
        public bool LogIn(Session session)
        {
            var player = Parent;

            // If the player isn't located anywhere yet, try to drop them in the default room.
            // (Expect that even new characters may gain a starting position via custom character generation
            // flows which let the user to select a starting spawn area.)
            var targetPlayerStartingPosition = player.Parent != null ? player.Parent : FindDefaultRoom();
            if (targetPlayerStartingPosition == null)
            {
                session.WriteLine("Could not place character in the game world. Please contact an administrator.");
                return false;
            }

            // Prepare a login request and event.
            var csb = new ContextualStringBuilder(player, targetPlayerStartingPosition);
            csb.Append($"{session.Thing.Name} enters the world.", ContextualStringUsage.WhenNotBeingPassedToOriginator);
            csb.Append($"You enter the world.", ContextualStringUsage.OnlyWhenBeingPassedToOriginator);
            var message = new SensoryMessage(SensoryType.Sight, 100, csb);
            var e = new PlayerLogInEvent(player, message);

            // Broadcast the login request to the player's current location (IE their parent room), if applicable.
            player.Eventing.OnMiscellaneousRequest(e, EventScope.ParentsDown);

            // Also broadcast the request to any registered global listeners.
            PlayerManager.Instance.OnPlayerLogInRequest(player, e);

            // If nothing canceled this event request, carry on with the login.
            if (!e.IsCanceled)
            {
                ClearAFK();

                targetPlayerStartingPosition.Add(player);

                DateTime universalTime = DateTime.Now.ToUniversalTime();
                PlayerData.LastLogin = universalTime.ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";
                PlayerData.LastIPAddress = session.Connection.CurrentIPAddress.ToString();

                session.Thing = player;
                session.User.LastLogInTime = DateTime.Now; // Should this occur when user was authenticated instead?
                player.FindBehavior<UserControlledBehavior>().Session = session;

                // Broadcast that the player successfully logged in, to their login location.
                player.Eventing.OnMiscellaneousEvent(e, EventScope.ParentsDown);

                // TODO: Fix: Sending this before the player has registered as being in the PlayingState causes some problems,
                //       including the last character creation or password prompt from printing again as the user's prompt.
                //       Need to figure out if this can be straightened out in a clean way.
                //       See: https://github.com/DavidRieman/WheelMUD/issues/86#issuecomment-787057858
                PlayerManager.Instance.OnPlayerLogIn(player, e);

                return true;
            }

            return false;
        }

        /// <summary>Clear all AFK status (for a player who is detected to be no longer AFK).</summary>
        public void ClearAFK()
        {
            IsAFK = false;
            AFKReason = null;
        }

        /// <summary>Builds the player's prompt.</summary>
        /// <returns>The player's current prompt.</returns>
        public virtual OutputBuilder BuildPrompt(TerminalOptions terminalOptions)
        {
            return Renderer.Instance.RenderPrompt(terminalOptions, Parent);
        }

        /// <summary>Try to log this player out of the game.</summary>
        /// <param name="force">When true, logging out ignores normal restrictions and forces the logout. For administrative use only.</param>
        /// <returns>Indicates whether the logout was successful or not.</returns>
        public bool LogOut(bool force = false)
        {
            var player = Parent;

            // Prepare a logout request and event.
            var csb = new ContextualStringBuilder(player, player.Parent);
            csb.Append($"{player.Name} exits the world.", ContextualStringUsage.WhenNotBeingPassedToOriginator);
            csb.Append($"You exit the world.", ContextualStringUsage.OnlyWhenBeingPassedToOriginator);
            var message = new SensoryMessage(SensoryType.Sight, 100, csb);
            var e = new PlayerLogOutEvent(player, message);

            // Broadcast the logout request to the player's current location (if applicable).
            player.Eventing.OnMiscellaneousRequest(e, EventScope.ParentsDown);

            // Also broadcast the request to any registered global listeners.
            PlayerManager.Instance.OnPlayerLogOutRequest(player, e);

            // If nothing canceled this event request, carry on with the logout.
            if (!e.IsCanceled || force)
            {
                player.FindBehavior<UserControlledBehavior>()?.Disconnect();
                DateTime universalTime = DateTime.Now.ToUniversalTime();
                PlayerData.LastLogout = universalTime.ToString("s", DateTimeFormatInfo.InvariantInfo) + "Z";

                player.FindBehavior<PlayerBehavior>()?.SavePlayer();
                Dispose();
                player.Dispose();

                // Broadcast that the player successfully logged out, to their parent (IE room).
                player.Eventing.OnMiscellaneousEvent(e, EventScope.ParentsDown);
                PlayerManager.Instance.OnPlayerLogOut(player, e);

                return true;
            }

            return false;
        }

        /// <summary>Called when a parent has just been assigned to this behavior. (Refer to Parent)</summary>
        protected override void OnAddBehavior()
        {
            var sensesBehavior = Parent.FindBehavior<SensesBehavior>();
            var userControlledBehavior = Parent.FindBehavior<UserControlledBehavior>();
            EventProcessor = new PlayerEventProcessor(this, sensesBehavior, userControlledBehavior);
            EventProcessor.AttachEvents();
        }

        protected override void OnRemoveBehavior()
        {
            if (EventProcessor != null)
            {
                EventProcessor.DetachEvents();
                EventProcessor.Dispose();
                EventProcessor = null;
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            SessionId = null;
            friends = new List<string>();

            PlayerManager.Instance.GlobalPlayerLogInEvent += ProcessPlayerLogInEvent;
            PlayerManager.Instance.GlobalPlayerLogOutEvent += ProcessPlayerLogOutEvent;
        }

        private void ProcessPlayerLogInEvent(Thing root, GameEvent e)
        {
            // If this is a friend, ensure we get a 'your friend logged in' message regardless of location.
            if (IsFriend(e.ActiveThing.Name) && e is PlayerLogInEvent)
            {
                var userControlledBehavior = Parent.FindBehavior<UserControlledBehavior>();
                userControlledBehavior?.Session?.WriteLine($"Your friend {e.ActiveThing.Name} has logged in.");
            }
        }

        private void ProcessPlayerLogOutEvent(Thing root, GameEvent e)
        {
            // If this is a friend, ensure we get a 'your friend logged out' message regardless of location.
            if (IsFriend(e.ActiveThing.Name) && e is PlayerLogOutEvent)
            {
                var userControlledBehavior = Parent.FindBehavior<UserControlledBehavior>();
                userControlledBehavior?.Session?.WriteLine($"Your friend {e.ActiveThing.Name} has logged out.");
            }
        }

        private static Thing FindDefaultRoom()
        {
            // TODO: Cache a weak reference to the Thing and acquire/reacquire when needed?
            return (from t in ThingManager.Instance.Things
                    where t.Id == "room/" + GameConfiguration.DefaultRoomID
                    select t).FirstOrDefault();
        }

        private bool IsFriend(string friendName)
        {
            lock (friendsLock)
            {
                return Friends.Contains(friendName.ToLower());
            }
        }
    }
}