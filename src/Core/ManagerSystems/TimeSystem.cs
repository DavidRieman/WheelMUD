//-----------------------------------------------------------------------------
// <copyright file="TimeSystem.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Provides a world time system to the mud. Actions can be scheduled to
//   occur at various times, with 1-second resolution, without spawning
//   new timers for each event. Necessary because there could be thousands
//   of temporary effects, delayed commands, and so on.
// </summary>
// <remarks>
//   This class was originally to be a custom calendar for the game, which
//   would differ from real-world time system and broadcast periodic updates
//   about the current time. Custom calendars are game-specific and usually
//   can be implemented by transforming DateTime.Now and scheduling
//   occasional broadcasts if desired.
// </remarks>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using WheelMUD.Core.Events;
    using WheelMUD.Interfaces;

    /// <summary>
    /// Provides a world time system to the mud. Actions can be scheduled to occur at various times, with
    /// a rounded off resolution, without spawning new timers for each event. Grouped processing like this
    /// is necessary because there could be thousands of temporary effects, delayed commands, and so on.
    /// </summary>
    public class TimeSystem : ManagerSystem
    {
        /// <summary>The singleton instance of this class.</summary>
        private static readonly Lazy<TimeSystem> SingletonInstance = new Lazy<TimeSystem>(() => new TimeSystem());

        /// <summary>The callback queue.</summary>
        private readonly TimerQueue callbackQueue = new TimerQueue();

        /// <summary>The timer provides a "heartbeat".</summary>
        private Timer timer;

        /// <summary>The interval between ticks of the shared timer, in milliseconds.</summary>
        /// <remarks>TODO: Test and tweak the default for this value based on typical gameplay timer loads.</remarks>
        private readonly int interval = 250;

        /// <summary>Gets the singleton instance of this class.</summary>
        public static TimeSystem Instance => SingletonInstance.Value;

        /// <summary>Start the time system.</summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");

            if (timer == null)
            {
                timer = new Timer(DoCallbacks, null, interval, interval);
            }

            SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops the time system.</summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            timer.Change(Timeout.Infinite, Timeout.Infinite);
            timer.Dispose();

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Schedules an action according to the supplied <see cref="WheelMUD.Core.TimeEvent"/>.</summary>
        /// <param name="timeEventArgs">The <see cref="WheelMUD.Core.TimeEvent"/> instance containing the event data.</param>
        public void ScheduleEvent(TimeEvent timeEventArgs)
        {
            if (timeEventArgs == null)
            {
                throw new ArgumentException("Cannot schedule a null event.", "args");
            }

            lock (callbackQueue)
            {
                callbackQueue.Enqueue(timeEventArgs);
            }
        }

        /// <summary>Calls any callbacks that have become due.</summary>
        /// <param name="state">Currently unused.</param>
        private void DoCallbacks(object state)
        {
            lock (callbackQueue)
            {
                // Loop over all events that have expired.
                DateTime currentTime = DateTime.Now;
                TimeEvent timeEvent = callbackQueue.Peek();
                while (timeEvent != null && timeEvent.EndTime <= currentTime)
                {
                    var callback = callbackQueue.Dequeue().Callback;

                    if (!timeEvent.IsCancelled)
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    }

                    timeEvent = callbackQueue.Peek();
                }
            }
        }

        /// <summary>A class for exporting/importing system singleton through MEF.</summary>
        [ExportSystem(0)]
        public class TimeSystemExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            /// <returns>A new instance of the singleton system.</returns>
            public override ISystem Instance => TimeSystem.Instance;

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType => typeof(TimeSystem);
        }

        /// <summary>
        /// Bare-bones priority queue for use with the timekeeping system.
        /// Associates an Action with a DateTime expiration, and every Peek/Dequeue operation
        /// will return the Action with the earliest expiration time in the collection.
        /// </summary>
        private class TimerQueue
        {
            /// <summary>Collection to store the timed events.</summary>
            private List<TimeEvent> heap = new List<TimeEvent>();

            /// <summary>Enqueues the specified element and the associated expiration DateTime.</summary>
            /// <param name="timeEventArgs">The <see cref="WheelMUD.Core.TimeEvent"/> instance containing the event data.</param>
            public void Enqueue(TimeEvent timeEventArgs)
            {
                lock (heap)
                {
                    heap.Add(timeEventArgs);
                    UpHeap();
                }
            }

            /// <summary>Returns the highest-priority <see cref="WheelMUD.Core.TimeEvent"/> item without removing it from the heap.</summary>
            /// <returns>The highest-priority <see cref="WheelMUD.Core.TimeEvent"/> item.</returns>
            public TimeEvent Peek()
            {
                lock (heap)
                {
                    return (heap.Count > 0) ? heap[0] : null;
                }
            }

            /// <summary>Returns the highest-priority <see cref="WheelMUD.Core.TimeEvent"/> item and removes it from the heap.</summary>
            /// <returns>The highest-priority <see cref="WheelMUD.Core.TimeEvent"/> item.</returns>
            public TimeEvent Dequeue()
            {
                lock (heap)
                {
                    if (heap.Count > 0)
                    {
                        var result = heap[0];
                        heap[0] = heap.Last();
                        heap.RemoveAt(heap.Count - 1);
                        DownHeap();
                        return result;
                    }

                    return null;
                }
            }

            /// <summary>Adjusts the heap for the case where a new item was added.</summary>
            private void UpHeap()
            {
                if (heap.Count < 2)
                {
                    return;
                }

                int child = heap.Count - 1;
                int parent = (child - 1) / 2;
                var newest = heap[child];

                while (newest.EndTime < heap[parent].EndTime)
                {
                    heap[child] = heap[parent];
                    child = parent;
                    if (parent == 0)
                    {
                        break;
                    }

                    parent = (parent - 1) / 2;
                }

                heap[child] = newest;
            }

            /// <summary>Adjusts the heap for the case where the highest-priority item was removed.</summary>
            private void DownHeap()
            {
                if (heap.Count < 2)
                {
                    return;
                }

                int parent = 0;
                int leftChild = 1;
                int rightChild = leftChild + 1;
                var item = heap[parent];

                if (rightChild < heap.Count - 1 && heap[rightChild].EndTime < heap[leftChild].EndTime)
                {
                    leftChild = rightChild;
                }

                while (leftChild < heap.Count - 1 && heap[leftChild].EndTime < item.EndTime)
                {
                    heap[parent] = heap[leftChild];
                    parent = leftChild;
                    leftChild = (parent * 2) + 1;
                    rightChild = leftChild + 1;

                    if (rightChild < heap.Count - 1 && heap[rightChild].EndTime < heap[leftChild].EndTime)
                    {
                        leftChild = rightChild;
                    }
                }

                heap[parent] = item;
            }
        }
    }
}