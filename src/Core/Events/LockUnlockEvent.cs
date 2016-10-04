//-----------------------------------------------------------------------------
// <copyright file="LockUnlockEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: March 2012 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A lock/unlock event.</summary>
    public class LockUnlockEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="LockUnlockEvent"/> class.</summary>
        /// <param name="target">The thing being affected by this event.</param>
        /// <param name="isBeingLocked">Whether the thing is being locked (true) or unlocked (false).</param>
        /// <param name="activeThing">The actor causing the event (if applicable).</param>
        /// <param name="sensoryMessage">The message to display to those who can perceive the change.</param>
        public LockUnlockEvent(Thing target, bool isBeingLocked, Thing activeThing, SensoryMessage sensoryMessage)
            : base(activeThing, sensoryMessage)
        {
            this.Target = target;
            this.IsBeingLocked = isBeingLocked;
        }

        /// <summary>Gets a value indicating whether this event pertains to the target being locked (true) or unlocked (false).</summary>
        public bool IsBeingLocked { get; private set; }

        /// <summary>Gets the thing that was affected.</summary>
        public Thing Target { get; private set; }
    }
}