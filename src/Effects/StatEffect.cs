//-----------------------------------------------------------------------------
// <copyright file="StatEffect.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Effects
{
    /// <summary>An effect to alter a stat on a thing.</summary>
    public class StatEffect : Effect
    {
        /// <summary>Initializes a new instance of the StatEffect class.</summary>
        public StatEffect()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="StatEffect" /> class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="stat">The stat.</param>
        /// <param name="valueMod">The modification to the stat's value.</param>
        /// <param name="minimumMod">The modification to the stat's minimum.</param>
        /// <param name="maximumMod">The modification to the stat's maximum.</param>
        /// <param name="duration">The duration of the effect.</param>
        /// <param name="sensoryMessage">The sensory message.</param>
        /// <param name="expirationMessage">The expiration message.</param>
        public StatEffect(Thing activeThing, GameStat stat, int valueMod, int minimumMod, int maximumMod, TimeSpan duration, SensoryMessage sensoryMessage, SensoryMessage expirationMessage)
            : base(duration)
        {
            Name = stat.Name;
            Stat = stat;
            ValueMod = valueMod;
            MinimumMod = minimumMod;
            MaximumMod = maximumMod;
            ActiveThing = activeThing;
            SensoryMessage = sensoryMessage;
            ExpirationMessage = expirationMessage;
            Duration = duration;
        }

        /// <summary>Initializes a new instance of the StatEffect class.</summary>
        /// <param name="instanceId">ID of the effect instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this effect instance.</param>
        public StatEffect(long instanceId, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceId;
        }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the active thing.</summary>
        public Thing ActiveThing { get; set; }

        /// <summary>Gets or sets the sensory message.</summary>
        public SensoryMessage SensoryMessage { get; set; }

        /// <summary>Gets or sets the expiration message.</summary>
        public SensoryMessage ExpirationMessage { get; set; }

        /// <summary>Gets or sets the stat being modified.</summary>
        public GameStat Stat { get; set; }

        /// <summary>Gets or sets the modification to the stat's current value.</summary>
        public int ValueMod { get; set; }

        /// <summary>Gets or sets the modification to the stat's minimum value.</summary>
        public int MinimumMod { get; set; }

        /// <summary>Gets or sets the modification to the stat's maximum value.</summary>
        public int MaximumMod { get; set; }

        /// <summary>Gets or sets the <see cref="TimeEvent" /> related to this effect's expiration.</summary>
        /// <value>The <see cref="TimeEvent" /> related to this effect's expiration.</value>
        public TimeEvent RemoveStatEvent { get; set; }

        /// <summary>Expires this instance.</summary>
        public void Expire()
        {
            Parent.Behaviors.Remove(this);

            RemoveStatEvent.Cancel(string.Empty);
            RemoveStatEvent = null;
        }

        /// <summary>Applies the effect to its host.</summary>
        /// <remarks>Preconditions: Parent.Attributes must not be null if Parent is not null.</remarks>
        protected override void OnAddBehavior()
        {
            // Create and broadcast the event notifying players that the effect was applied.
            var addEvent = new EffectEvent(ActiveThing, Parent, SensoryMessage);
            ActiveThing.Eventing.OnCommunicationRequest(addEvent, EventScope.ParentsDown);
            if (!addEvent.IsCanceled)
            {
                ActiveThing.Eventing.OnCommunicationEvent(addEvent, EventScope.ParentsDown);
            }

            // Create and schedule an event that tells TimeSystem to call Expire()
            // after EndTime is reached.
            RemoveStatEvent = new TimeEvent(ActiveThing, Expire, EndTime, ExpirationMessage);
            TimeSystem.Instance.ScheduleEvent(RemoveStatEvent);
        }

        /// <summary>The method that is called when an effect is to be removed.</summary>
        /// <remarks>
        /// It is effectively the cleanup operation, i.e. reduce stats to normal level.
        /// Preconditions: Parent.Attributes must not be null if Parent is not null.
        /// Stat must not be null - provide defaults in SetDefaultProperties().
        /// </remarks>
        protected override void OnRemoveBehavior()
        {
            // Broadcast the removal event that we previously created in OnAddBehavior.
            ActiveThing.Eventing.OnCommunicationRequest(RemoveStatEvent, EventScope.ParentsDown);
            if (!RemoveStatEvent.IsCanceled)
            {
                ActiveThing.Eventing.OnCommunicationEvent(RemoveStatEvent, EventScope.ParentsDown);
            }

            // Normally the TimeSystem will remove this event when it expires.
            // If it is removed by some other means, we should cancel the RemoveStatEvent so
            // TimeSystem will know to ignore it when the EndTime is reached.
            if (RemoveStatEvent != null && !RemoveStatEvent.IsCanceled)
            {
                RemoveStatEvent.Cancel(string.Empty);
            }
        }

        /// <summary>Sets the default properties of this effect instance. Duration is given a default TimeSpan.</summary>
        protected override void SetDefaultProperties()
        {
            Duration = TimeSpan.FromMinutes(1);
        }
    }
}