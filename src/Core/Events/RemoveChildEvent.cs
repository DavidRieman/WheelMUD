// <copyright file="RemoveChildEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A thing removal request/event.
//   Created: September 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A thing removal request/event.</summary>
    public class RemoveChildEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the RemoveChildEvent class.</summary>
        /// <param name="activeThing">The active thing being removed from it's current Parent.</param>
        public RemoveChildEvent(Thing activeThing)
            : base(activeThing, null)
        {
            this.OldParent = activeThing.Parent;
        }

        /// <summary>Gets the parent of the thing before being removed.</summary>
        /// <value>The parent of the thing before being removed.</value>
        public Thing OldParent { get; private set; }
    }
}
