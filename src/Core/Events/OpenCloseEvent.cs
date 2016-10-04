//-----------------------------------------------------------------------------
// <copyright file="OpenCloseEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An 'open/close' event.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>An 'open/close' event.</summary>
    public class OpenCloseEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the OpenCloseEvent class.</summary>
        /// <param name="target">The thing being affected by this event.</param>
        /// <param name="isBeingOpened">Whether the thing is being opened (true) or closed (false).</param>
        /// <param name="activeThing">The actor causing the event (if applicable).</param>
        /// <param name="sensoryMessage">The message to display to those who can perceive the change.</param>
        public OpenCloseEvent(Thing target, bool isBeingOpened, Thing activeThing, SensoryMessage sensoryMessage)
            : base(activeThing, sensoryMessage)
        {
            this.Target = target;
            this.IsBeingOpened = isBeingOpened;
        }

        /// <summary>Gets a value indicating whether this event pertains to the target being opened (true) or closed (false).</summary>
        public bool IsBeingOpened { get; private set; }

        /// <summary>Gets the thing that is being affected.</summary>
        public Thing Target { get; private set; }
    }
}