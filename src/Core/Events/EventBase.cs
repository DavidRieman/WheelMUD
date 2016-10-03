//-----------------------------------------------------------------------------
// <copyright file="EventBase.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The base class for in-game events.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>The base class for passing game event arguments.</summary>
    public class GameEvent
    {
        /// <summary>Initializes a new instance of the GameEvent class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="sensoryMessage">The sensory message.</param>
        public GameEvent(Thing activeThing, SensoryMessage sensoryMessage)
        {
            this.ActiveThing = activeThing;
            if (sensoryMessage != null)
            {
                this.SensoryMessage = sensoryMessage;

                // TODO: This if-condition was added to deal with some cases where
                // two ActiveThings are attempted for one action, e.g. "get".
                // Should multiple ActiveThings be supported instead, or maybe
                // there's a way to prevent this scenario?
                if (!this.SensoryMessage.Context.ContainsKey("ActiveThing"))
                {
                    this.SensoryMessage.Context.Add("ActiveThing", this.ActiveThing);
                }

                this.SensoryMessage.Context.Add(this.GetType().Name, this);
            }
        }

        /// <summary>Gets the thing that activated this event (normally an entity)</summary>
        public Thing ActiveThing { get; private set; }

        /// <summary>Gets the place that broadcast this event.</summary>
        public Thing RootLocation { get; private set; }

        /// <summary>Gets the message associated with this event.</summary>
        public SensoryMessage SensoryMessage { get; private set; }
    }

    /// <summary>Represents a game event that can be cancelled prior to actually raising the event.</summary>
    public class CancellableGameEvent : GameEvent
    {
        private bool sentCancelMessage;

        /// <summary>Initializes a new instance of the <see cref="CancellableGameEvent"/> class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="sensoryMessage">The sensory message.</param>
        public CancellableGameEvent(Thing activeThing, SensoryMessage sensoryMessage)
            : base(activeThing, sensoryMessage)
        {
        }

        /// <summary>Gets a value indicating whether this event has been cancelled.</summary>
        /// <value><c>true</c> if this instance is cancelled; otherwise, <c>false</c>.</value>
        public bool IsCancelled { get; private set; }

        /// <summary>Cancels the event, with the cancellation described by the specified cancel message.</summary>
        /// <param name="cancelMessage">The cancel message.</param>
        public void Cancel(string cancelMessage)
        {
            this.IsCancelled = true;
            if (!string.IsNullOrEmpty(cancelMessage) && !this.sentCancelMessage)
            {
                // Write up to one cancellation message directly to the user/initiator if appropriate.
                var userControlledBehavior = ActiveThing.Behaviors.FindFirst<UserControlledBehavior>();
                if (userControlledBehavior != null && userControlledBehavior.Controller != null)
                {
                    userControlledBehavior.Controller.Write(cancelMessage);
                }

                this.sentCancelMessage = true;
            }
        }
    }
}