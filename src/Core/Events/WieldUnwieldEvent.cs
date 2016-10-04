// -----------------------------------------------------------------------
// <copyright file="WieldUnwieldEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Event raised when an item is wielded or unwielded.
// </summary>
// -----------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>Event raised when an item is wielded or unwielded.</summary>
    public class WieldUnwieldEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="WieldUnwieldEvent"/> class.</summary>
        /// <param name="wieldedItem">The thing being affected by this event.</param>
        /// <param name="isBeingWielded">Whether the thing is being wielded (true) or unwielded (false).</param>
        /// <param name="activeThing">The actor causing the event (if applicable).</param>
        /// <param name="sensoryMessage">The message to display to those who can perceive the change.</param>
        public WieldUnwieldEvent(Thing wieldedItem, bool isBeingWielded, Thing activeThing, SensoryMessage sensoryMessage)
            : base(activeThing, sensoryMessage)
        {
            this.WieldedItem = wieldedItem;
            this.IsBeingWielded = isBeingWielded;
            sensoryMessage.Context.Add("WieldedItem", this.WieldedItem);
        }

        /// <summary>Gets a value indicating whether this event pertains to the target being wielded (true) or unwielded (false).</summary>
        public bool IsBeingWielded { get; private set; }

        /// <summary>Gets the thing that was affected.</summary>
        public Thing WieldedItem { get; private set; }
    }
}