//-----------------------------------------------------------------------------
// <copyright file="ThingEventing.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>Houses eventing functionality for a Thing instance.</summary>
    public class ThingEventing
    {
        /// <summary>The owner Thing of this instance.</summary>
        private Thing owner;

        /// <summary>Initializes a new instance of the ThingEventing class.</summary>
        /// <param name="owner">The owner of this ThingEventing class.</param>
        internal ThingEventing(Thing owner)
        {
            Debug.Assert(owner != null, "ThingEventing must always have an owner Thing.");
            this.owner = owner;
        }

        /// <summary>A non-cancelled game combat event.</summary>
        public event GameEventHandler CombatEvent;

        /// <summary>A cancellable game combat request.</summary>
        public event CancellableGameEventHandler CombatRequest;

        /// <summary>A non-cancelled movement event.</summary>
        public event GameEventHandler MovementEvent;

        /// <summary>A cancellable movement request.</summary>
        public event CancellableGameEventHandler MovementRequest;

        /// <summary>A non-cancelled communication event.</summary>
        public event GameEventHandler CommunicationEvent;

        /// <summary>A cancellable communication request.</summary>
        public event CancellableGameEventHandler CommunicationRequest;

        /// <summary>A non-cancelled event which does not fit in the other eventing categories.</summary>
        public event GameEventHandler MiscellaneousEvent;

        /// <summary>A cancellable request which does not fit in the other request categories.</summary>
        public event CancellableGameEventHandler MiscellaneousRequest;

        /// <summary>Raises the <see cref="CombatRequest"/> event.</summary>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.CancellableGameEvent"/> instance containing the event data.</param>
        /// <param name="eventScope">The base target(s) to broadcast to, including their children.</param>
        public void OnCombatRequest(CancellableGameEvent e, EventScope eventScope)
        {
            OnRequest(t => t.CombatRequest, e, eventScope);
        }

        /// <summary>Raises the <see cref="MovementRequest"/> event.</summary>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.CancellableGameEvent"/> instance containing the event data.</param>
        /// <param name="eventScope">The base target(s) to broadcast to, including their children.</param>
        public void OnMovementRequest(CancellableGameEvent e, EventScope eventScope)
        {
            OnRequest(t => t.MovementRequest, e, eventScope);
        }

        /// <summary>Raises the <see cref="CommunicationRequest"/> event.</summary>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.CancellableGameEvent"/> instance containing the event data.</param>
        /// <param name="eventScope">The base target(s) to broadcast to, including their children.</param>
        public void OnCommunicationRequest(CancellableGameEvent e, EventScope eventScope)
        {
            OnRequest(t => t.CommunicationRequest, e, eventScope);
        }

        /// <summary>Raises the <see cref="MiscellaneousRequest"/> event.</summary>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.CancellableGameEvent"/> instance containing the event data.</param>
        /// <param name="eventScope">The base target(s) to broadcast to, including their children.</param>
        public void OnMiscellaneousRequest(CancellableGameEvent e, EventScope eventScope)
        {
            OnRequest(t => t.MiscellaneousRequest, e, eventScope);
        }

        /// <summary>Raises the <see cref="CombatEvent"/> event.</summary>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.GameEvent"/> instance containing the event data.</param>
        /// <param name="eventScope">The base target(s) to broadcast to, including their children.</param>
        public void OnCombatEvent(GameEvent e, EventScope eventScope)
        {
            OnEvent(t => t.CombatEvent, e, eventScope);
        }

        /// <summary>Raises the <see cref="MovementEvent"/> event.</summary>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.GameEvent"/> instance containing the event data.</param>
        /// <param name="eventScope">The base target(s) to broadcast to, including their children.</param>
        public void OnMovementEvent(GameEvent e, EventScope eventScope)
        {
            OnEvent(t => t.MovementEvent, e, eventScope);
        }

        /// <summary>Raises the <see cref="CommunicationEvent"/> event.</summary>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.GameEvent"/> instance containing the event data.</param>
        /// <param name="eventScope">The base target(s) to broadcast to, including their children.</param>
        public void OnCommunicationEvent(GameEvent e, EventScope eventScope)
        {
            OnEvent(t => t.CommunicationEvent, e, eventScope);
        }

        /// <summary>Raises the <see cref="MiscellaneousEvent"/> event.</summary>
        /// <param name="e">The <see cref="WheelMUD.Core.Events.GameEvent"/> instance containing the event data.</param>
        /// <param name="eventScope">The base target(s) to broadcast to, including their children.</param>
        public void OnMiscellaneousEvent(GameEvent e, EventScope eventScope)
        {
            OnEvent(t => t.MiscellaneousEvent, e, eventScope);
        }

        private void OnRequest(Func<ThingEventing, CancellableGameEventHandler> handlerSelector, CancellableGameEvent e, EventScope eventScope)
        {
            // Determine what layer(s) we're broadcasting to; beyond the first layer, these should all be to Children.
            switch (eventScope)
            {
                case EventScope.ParentsDown:
                    // Send the request to each parent, until cancellation is noticed or we've finished.
                    Queue<Thing> requestQueue = new Queue<Thing>(owner.Parents);
                    while (requestQueue.Count > 0 && !e.IsCancelled)
                    {
                        Thing currentParent = requestQueue.Dequeue();
                        currentParent.Eventing.OnRequest(handlerSelector, e, true);
                    }

                    break;
                case EventScope.SelfDown:
                    OnRequest(handlerSelector, e, true);
                    break;
                case EventScope.SelfOnly:
                    OnRequest(handlerSelector, e, false);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void OnEvent(Func<ThingEventing, GameEventHandler> handlerSelector, GameEvent e, EventScope eventScope)
        {
            // Determine what layer(s) we're broadcasting to; beyond the first layer, these should all be to Children.
            switch (eventScope)
            {
                case EventScope.ParentsDown:
                    // Send the event to each parent.
                    List<Thing> allParents = owner.Parents;
                    foreach (var parent in allParents)
                    {
                        parent.Eventing.OnEvent(handlerSelector, e, true);
                    }

                    break;
                case EventScope.SelfDown:
                    OnEvent(handlerSelector, e, true);
                    break;
                case EventScope.SelfOnly:
                    OnEvent(handlerSelector, e, false);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>Shared code for request handling.</summary>
        /// <param name="handlerSelector">A function which returns the appropriate Handler for a given Thing.</param>
        /// <param name="e">The game event args to pass through.</param>
        /// <param name="cascadeEventToChildren">If true, the event should be cascaded to child things as well.</param>
        private void OnRequest(Func<ThingEventing, CancellableGameEventHandler> handlerSelector, CancellableGameEvent e, bool cascadeEventToChildren)
        {
            // Build a request target queue which starts with our owner Thing and visits all it's Children.
            // (This is a queue instead of recursion to help avoid stack overflows and such with very large object trees.)
            Queue<Thing> requestTargetQueue = new Queue<Thing>();
            requestTargetQueue.Enqueue(owner);

            while (requestTargetQueue.Count > 0)
            {
                // If anything (like one of the thing's Behaviors) is subscribed to this request, send it there.
                Thing currentRequestTarget = requestTargetQueue.Dequeue();
                var handler = handlerSelector(currentRequestTarget.Eventing);
                if (handler != null)
                {
                    handler(currentRequestTarget, e);

                    // If the event has been cancelled by the handler, we no longer need to look for further permission.
                    if (e.IsCancelled)
                    {
                        break;
                    }
                }

                if (cascadeEventToChildren)
                {
                    // Enqueue all the current target's children for processing.
                    foreach (Thing child in currentRequestTarget.Children)
                    {
                        requestTargetQueue.Enqueue(child);
                    }
                }
            }
        }

        private void OnEvent(Func<ThingEventing, GameEventHandler> handlerSelector, GameEvent e, bool cascadeEventToChildren)
        {
            // Build an event target queue which starts with our owner Thing and visits all it's Children.
            // (This is a queue instead of recursion to help avoid stack overflows and such with very large object trees.)
            Queue<Thing> eventTargetQueue = new Queue<Thing>();
            eventTargetQueue.Enqueue(owner);

            while (eventTargetQueue.Count > 0)
            {
                // If anything (like one of the thing's Behaviors) is subscribed to this event, send it there.
                Thing currentEventTarget = eventTargetQueue.Dequeue();
                handlerSelector(currentEventTarget.Eventing)?.Invoke(currentEventTarget, e);

                if (cascadeEventToChildren)
                {
                    // Enqueue all the current target's children for processing.
                    foreach (Thing child in currentEventTarget.Children)
                    {
                        eventTargetQueue.Enqueue(child);
                    }
                }
            }
        }
    }
}