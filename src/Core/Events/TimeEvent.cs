//-----------------------------------------------------------------------------
// <copyright file="TimeEvent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Represents an event that is broadcast when a time period has elapsed.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    using System;

    /// <summary>Represents an event that is broadcast when a time period has elapsed.</summary>
    public class TimeEvent : CancellableGameEvent
    {
        /// <summary>Initializes a new instance of the <see cref="TimeEvent" /> class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="sensoryMessage">The sensory message.</param>
        public TimeEvent(Thing activeThing, Action callback, DateTime endTime, SensoryMessage sensoryMessage)
            : base(activeThing, sensoryMessage)
        {
            this.Callback = callback;
            this.EndTime = endTime;
        }

        /// <summary>Gets the end time.</summary>
        public DateTime EndTime { get; private set; }

        /// <summary>Gets or sets an Action to be invoked when the time has elapsed.</summary>
        public Action Callback { get; set; }
    }
}