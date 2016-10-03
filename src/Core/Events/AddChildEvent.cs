// <copyright file="AddChildEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A thing add request/event.
//   Created: September 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A thing add request/event.</summary>
    public class AddChildEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the AddChildEvent class.</summary>
        /// <param name="activeThing">The active thing becoming a child of the NewParent.</param>
        /// <param name="newParent">The intended new parent of the active child.</param>
        public AddChildEvent(Thing activeThing, Thing newParent)
            : base(activeThing, null)
        {
            this.NewParent = newParent;
        }

        /// <summary>Gets the new parent.</summary>
        /// <value>The new parent.</value>
        public Thing NewParent { get; private set; }
    }
}