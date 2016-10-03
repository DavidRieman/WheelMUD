//-----------------------------------------------------------------------------
// <copyright file="EnterEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: May 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>Event associated with a thing entering another thing.</summary>
    public class EnterEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="EnterEvent"/> class.</summary>
        /// <param name="thingEntered">The thing entered.</param>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="sensoryMessage">The sensory message.</param>
        /// <param name="startLocation">The start location.</param>
        /// <param name="endLocation">The end location.</param>
        public EnterEvent(Thing thingEntered, Thing activeThing, SensoryMessage sensoryMessage, Thing startLocation, Thing endLocation)
            : base(activeThing, sensoryMessage)
        {
            this.ThingEntered = thingEntered;
            this.StartLocation = startLocation;
            this.EndLocation = endLocation;
        }

        /// <summary>Gets the thing that is being entered.</summary>
        public Thing ThingEntered { get; private set; }

        /// <summary>Gets the start location.</summary>
        /// <value>The start location.</value>
        public Thing StartLocation { get; private set; }

        /// <summary>Gets the end location.</summary>
        /// <value>The end location.</value>
        public Thing EndLocation { get; private set; }
    }
}
