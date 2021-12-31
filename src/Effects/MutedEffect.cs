//-----------------------------------------------------------------------------
// <copyright file="MutedEffect.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Core;
using WheelMUD.Interfaces;

namespace WheelMUD.Effects
{
    /// <summary>A class that represents the muted effect.</summary>
    public class MutedEffect : Effect
    {
        /// <summary>Initializes a new instance of the <see cref="MutedEffect" /> class.</summary>
        /// <param name="activeThing">The active thing.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="sensoryMessage">The sensory message.</param>
        /// <param name="expirationMessage">The expiration message.</param>
        public MutedEffect(Thing activeThing, TimeSpan duration, SensoryMessage sensoryMessage, SensoryMessage expirationMessage)
            : base(duration)
        {
            Name = "Mute";
            ActiveThing = activeThing;
            SensoryMessage = sensoryMessage;
            ExpirationMessage = expirationMessage;
            Duration = duration;
        }

        /// <summary>Initializes a new instance of the <see cref="MutedEffect" /> class.</summary>
        /// <param name="instanceId">ID of the effect instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this effect instance.</param>
        public MutedEffect(long instanceId, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            Name = "Mute";
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

        /// <summary>Gets or sets the event handler that intercepts and prevents communication.</summary>
        /// <remarks>A reference is stored here so it can be removed at the appropriate time.</remarks>
        /// <value>The event handler that intercepts and prevents communication.</value>
        public CancellableGameEventHandler Interceptor { get; set; }

        /// <summary>Gets or sets the unmute event.</summary>
        public TimeEvent UnmuteEvent { get; set; }

        /// <summary>Called when a parent has just been assigned to this behavior.</summary>
        /// <remarks>
        /// This method first creates and broadcasts an event notifying users that
        /// the mute effect was applied. Then it creates another event that holds
        /// the Unmute method, and adds it to the TimeSystem scheduler to call
        /// after the duration time has passed.
        /// </remarks>
        protected override void OnAddBehavior()
        {
            // While this effect is attached to its parent, it denies all verbal communications from it.
            Interceptor = new CancellableGameEventHandler(DenyCommunicationRequest);
            Parent.Eventing.CommunicationRequest += Interceptor;

            // Create event and broadcast it to let the affected parties know the effect was applied.
            var muteEvent = new EffectEvent(ActiveThing, Parent, SensoryMessage);
            ActiveThing.Eventing.OnCommunicationRequest(muteEvent, EventScope.ParentsDown);
            if (!muteEvent.IsCanceled)
            {
                ActiveThing.Eventing.OnCommunicationEvent(muteEvent, EventScope.ParentsDown);
            }

            // Create an event to be broadcast when the mute effect expires,
            // and schedule it with the TimeSystem.
            UnmuteEvent = new TimeEvent(ActiveThing, Unmute, EndTime, ExpirationMessage);
            TimeSystem.Instance.ScheduleEvent(UnmuteEvent);

            base.OnAddBehavior();
        }

        /// <summary>Called when the current parent of this behavior is about to be removed. (Refer to Parent.)</summary>
        protected override void OnRemoveBehavior()
        {
            // Stop intercepting communication events.
            Parent.Eventing.CommunicationRequest -= Interceptor;
            Interceptor = null;

            // Broadcast the event that was created earlier in OnAddBehavior.
            ActiveThing.Eventing.OnCommunicationRequest(UnmuteEvent, EventScope.ParentsDown);
            if (!UnmuteEvent.IsCanceled)
            {
                ActiveThing.Eventing.OnCommunicationEvent(UnmuteEvent, EventScope.ParentsDown);
            }

            // In case the effect was removed manually (i.e. not by TimeSystem), we should cancel the event so
            // TimeSystem will know to ignore it when the EndTime is reached.
            if (UnmuteEvent != null && !UnmuteEvent.IsCanceled)
            {
                UnmuteEvent.Cancel(string.Empty);
            }

            base.OnRemoveBehavior();
        }

        /// <summary>Removes this effect from its parent Thing.</summary>
        public void Unmute()
        {
            Parent.Eventing.OnCommunicationEvent(UnmuteEvent, EventScope.ParentsDown);
            Parent.Behaviors.Remove(this);
        }

        /// <summary>Sets the default properties of this effect instance.</summary>
        protected override void SetDefaultProperties()
        {
            Duration = TimeSpan.FromMinutes(60);
        }

        /// <summary>Deny communication requests by the host Thing.</summary>
        /// <param name="root">The thing making the request.</param>
        /// <param name="e">The communication request event args.</param>
        private void DenyCommunicationRequest(IThing root, CancellableGameEvent e)
        {
            var communicationRequest = e as VerbalCommunicationEvent;
            if (communicationRequest != null && communicationRequest.ActiveThing == Parent)
            {
                e.Cancel("You are currently muted.");
            }
        }
    }
}