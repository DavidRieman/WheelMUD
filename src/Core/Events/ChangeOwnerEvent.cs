//-----------------------------------------------------------------------------
// <copyright file="ChangeOwnerEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
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
            OldOwner = oldOwner;
            NewOwner = newOwner;
            Thing = thing;
            senseMessage.Context.Add("Thing", Thing);
            senseMessage.Context.Add("OldOwner", OldOwner);
            senseMessage.Context.Add("NewOwner", NewOwner);
        }

        /// <summary>Gets the new owner of the item.</summary>
        public Thing NewOwner { get; private set; }

        /// <summary>Gets the old owner of the item.</summary>
        public Thing OldOwner { get; private set; }

        /// <summary>Gets the item which changed owners.</summary>
        public Thing Thing { get; private set; }
    }
}