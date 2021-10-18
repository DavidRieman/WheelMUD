//-----------------------------------------------------------------------------
// <copyright file="UserControlledBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace WheelMUD.Core
{
    /// <summary>A security role.</summary>
    public class Role
    {
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>Sets this Thing to be directly user-controlled.</summary>
    /// <remarks>
    /// Usually this will be accompanied by a PlayerBehavior, but this behavior may also be used
    /// for things like a player controlling a mobile directly, an admin is controlling a PC, etc.
    /// TODO: It may be interesting for multiple UserControlledBehaviors to be attached to the same Thing
    /// in more cases, such as two player fighting for control of a mob they both dominated with spells,
    /// or for an admin to assume control of a player but setting the player's instance to ignore all
    /// commands, so both can see the results of the admin's direction but only the admin can execute 
    /// any commands.
    /// </remarks>
    public class UserControlledBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the <see cref="UserControlledBehavior"/> class.</summary>
        public UserControlledBehavior()
            : this(0, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="UserControlledBehavior"/> class.</summary>
        /// <param name="instanceId">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public UserControlledBehavior(long instanceId, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceId;

            // TODO: Subscribe to movement events for the attached Thing: Whenever a user-controlled
            // thing is moved from one location to another, we want to respond by automatically "looking"
            // for the player, immediately.  (Possibly not though, depending on their settings and such,
            // IE possibly having a setting to disable descriptions upon rooms that have already been
            // marked as visited by the player, or possibly not if they are flagged as "running" through 
            // multiple rooms, etc.)
        }

        /// <summary>Gets the combined set of security roles associated with this user.</summary>
        public SecurityRole SecurityRoles { get; set; }

        /// <summary>Gets or sets the user Session of the Thing.</summary>
        [JsonIgnore]
        public Session Session { get; set; }

        /// <summary>Gets or sets the number of rows this user's client can handle displaying as a single "page".</summary>
        public int PagingRowLimit { get; set; }

        /// <summary>Gets the human-readable list of role names attached to this user-controlled thing.</summary>
        public IEnumerable<string> GetRoleNames()
        {
            return from role in GetIndividualSecurityRoles() select role.ToString();
        }

        /// <summary>Drop any existing connection for this user.</summary>
        /// <param name="disconnectMessage">If non-empty, send this message to the user before disconnecting.</param>
        public void Disconnect(string disconnectMessage = "")
        {
            if (Session != null)
            {
                if (!string.IsNullOrEmpty(disconnectMessage))
                {
                    Session.WriteLine(disconnectMessage, false);
                }
                Session.Connection.Disconnect();
            }
            Session = null;
        }

        /// <summary>Gets the individual security roles associated with this user.</summary>
        public IEnumerable<SecurityRole> GetIndividualSecurityRoles()
        {
            foreach (var role in SecurityRoleHelpers.IndividualSecurityRoles)
            {
                if ((SecurityRoles & role) != SecurityRole.none)
                {
                    yield return role;
                }
            }
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            Session = null;
        }
    }
}