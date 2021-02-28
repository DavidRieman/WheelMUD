//-----------------------------------------------------------------------------
// <copyright file="FollowedBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace WheelMUD.Core
{
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
    /// <remarks>TODO: Change this class to use SimpleWeakReference for the Followers collection.</remarks>
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
            ID = instanceID;
        }

        /// <summary>Gets the list of followers.</summary>
        [JsonIgnore]
        public virtual HashSet<Thing> Followers { get; private set; }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            Followers = new HashSet<Thing>();
        }
    }
}
