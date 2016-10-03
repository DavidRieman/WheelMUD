//-----------------------------------------------------------------------------
// <copyright file="FollowedBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A behavior that indicates who is currently following the player or mob.
//   Currently not too useful, but this is the companion to FollowingBehavior,
//   which leads to movement when following someone.
//   Created: 3/23/2012 by James McManus.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Behaviors
{
    using System.Collections.Generic;
    using Raven.Imports.Newtonsoft.Json;

    /// <summary>
    /// <para>
    /// The FollowedBehavior class is applied to players or mobile things that are being
    /// followed. For example, a player may follow another player or an aggressive mobile
    /// might try to follow a fleeing opponent.
    /// </para>
    /// <para>
    /// FollowedBehavior exposes a list of followers, so that evasive or retaliatory actions
    /// may be taken.
    /// </para>
    /// </summary>
    /// <remarks>TODO: Change this class to use WeakReference for the Followers collection.</remarks>
    public class FollowedBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the FollowedBehavior class.</summary>
        public FollowedBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the FollowedBehavior class.</summary>
        /// <param name="instanceID">The instance ID.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public FollowedBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets the list of followers.</summary>
        [JsonIgnore]
        public virtual HashSet<Thing> Followers { get; private set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.Followers = new HashSet<Thing>();
        }
    }
}
