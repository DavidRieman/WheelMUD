//-----------------------------------------------------------------------------
// <copyright file="ChangeOwnerEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   A 'change owner' event.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>A 'change owner' event.</summary>
    public class ChangeOwnerEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the ChangeOwnerEvent class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="senseMessage">The sensory message.</param>
        /// <param name="oldOwner">The old owner of the item.</param>
        /// <param name="newOwner">The new owner of the item.</param>
        /// <param name="thing">The item which changed owners.</param>
        public ChangeOwnerEvent(Thing activeThing, SensoryMessage senseMessage, Thing oldOwner, Thing newOwner, Thing thing)
            : base(activeThing, senseMessage)
        {
            this.OldOwner = oldOwner;
            this.NewOwner = newOwner;
            this.Thing = thing;
            senseMessage.Context.Add("Thing", this.Thing);
            senseMessage.Context.Add("OldOwner", this.OldOwner);
            senseMessage.Context.Add("NewOwner", this.NewOwner);
        }

        /// <summary>Gets the new owner of the item.</summary>
        public Thing NewOwner { get; private set; }

        /// <summary>Gets the old owner of the item.</summary>
        public Thing OldOwner { get; private set; }

        /// <summary>Gets the item which changed owners.</summary>
        public Thing Thing { get; private set; }
    }
}