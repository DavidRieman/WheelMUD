//-----------------------------------------------------------------------------
// <copyright file="UserControlledBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: May 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Imports.Newtonsoft.Json;
    using WheelMUD.Interfaces;

    /// <summary>A security role.</summary>
    public class Role
    {
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name.</value>
        public string Name { get; set; }
    }

    /// <summary>Sets this Thing to be directly user-controlled.</summary>
    /// <remarks>
    /// Usually this will be accompanied by a PlayerBehavior, but @@@ this behavior may also be used
    /// for things like a player controlling a mobile directly, an admin is controlling a PC, etc.
    /// @@@ It may be interesting for multiple UserControlledBehaviors to be attached to the same Thing
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
            this.ID = instanceId;

            // @@@ TODO: Subscribe to movement events for the attached Thing: Whenever a user-controlled
            // thing is moved from one location to another, we want to respond by automatically "looking"
            // for the player, immediately.  (Possibly not though, depending on their settings and such,
            // IE possibly having a setting to disable descriptions upon rooms that have already been
            // marked as visited by the player, or possibly not if they are flagged as "running" through 
            // multiple rooms, etc.)
        }

        //// @@@ TODO: Clean up Roles to remove DAL-bound RoleRecords and such...?
        ///// <summary>Gets the current list of <see cref="RoleRecord"/> associated with this player.</summary>
        ////public List<RoleRecord> RoleRecords { get; private set; }

        /// <summary>Gets the current list of <see cref="Role"/> names associated with this user.</summary>
        [JsonIgnore]
        public List<Role> Roles { get; private set; }

        /// <summary>Gets or sets the controller of the Thing.</summary>
        [JsonIgnore]
        public IController Controller { get; set; }
        
        /// <summary>Gets or sets the number of rows this user's client can handle displaying as a single "page".</summary>
        public int PagingRowLimit { get; set; }

        /// <summary>Gets the view engine.</summary>
        /// <value>The view engine.</value>
        [JsonIgnore]
        public ViewEngine ViewEngine { get; private set; }

        /// <summary>Gets the role of the specified name, if present.</summary>
        /// <param name="roleName">The name of the role to search for.</param>
        /// <returns>The Role, if this user has it, else null.</returns>
        public Role FindRole(string roleName)
        {
            return (from r in this.Roles
                    where string.Equals(r.Name, roleName, System.StringComparison.CurrentCultureIgnoreCase)
                    select r).FirstOrDefault();
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.Controller = null;
            this.ViewEngine = new ViewEngine();
        }
    }
}