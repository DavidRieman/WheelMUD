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
        private TimerQueue callbackQueue = new TimerQueue();

        /// <summary>The timer provides a "heartbeat".</summary>
        private Timer timer;

        /// <summary>The interval between ticks of the shared timer, in milliseconds.</summary>
        /// <remarks>TODO: Test and tweak the default for this value based on typical gameplay timer loads.</remarks>
        private int interval = 250;

        /// <summary>Gets the singleton instance of this class.</summary>
        public static TimeSystem Instance
        {
            get { return SingletonInstance.Value; }
        }

        /// <summary>Start the time system.</summary>
        public override void Start()
        {
            this.SystemHost.UpdateSystemHost(this, "Starting...");

            if (this.timer == null)
            {
                this.timer = new Timer(this.DoCallbacks, null, this.interval, this.interval);
            }

            this.SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops the time system.</summary>
        public override void Stop()
        {
            this.SystemHost.UpdateSystemHost(this, "Stopping...");

            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            this.timer.Dispose();

            this.SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Schedules an action according to the supplied <see cref="WheelMUD.Core.TimeEvent"/>.</summary>
        /// <param name="timeEventArgs">The <see cref="WheelMUD.Core.TimeEvent"/> instance containing the event data.</param>
        public void ScheduleEvent(TimeEvent timeEventArgs)
        {
            if (timeEventArgs == null)
            {
                throw new ArgumentException("Cannot schedule a null event.", "args");
            }

            lock (this.callbackQueue)
            {
                this.callbackQueue.Enqueue(timeEventArgs);
            }
        }

        /// <summary>Calls any callbacks that have become due.</summary>
        /// <param name="state">Currently unused.</param>
        private void DoCallbacks(object state)
        {
            lock (this.callbackQueue)
            {
                // Loop over all events that have expired.
                DateTime currentTime = DateTime.Now;
                TimeEvent timeEvent = this.callbackQueue.Peek();
                while (timeEvent != null && timeEvent.EndTime <= currentTime)
                {
                    var callback = this.callbackQueue.Dequeue().Callback;

                    if (!timeEvent.IsCancelled)
                    {
                        if (callback != null)
                        {
                            callback();
                        }
                    }

                    timeEvent = this.callbackQueue.Peek();
                }
            }
        }

        /// <summary>A class for exporting/importing system singleton through MEF.</summary>
        [ExportSystem]
        public class TimeSystemExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            /// <returns>A new instance of the singleton system.</returns>
            public override ISystem Instance
            {
                get { return TimeSystem.Instance; }
            }

            /// <summary>Gets the Type of the singleton system, without instantiating it.</summary>
            /// <returns>The Type of the singleton system.</returns>
            public override Type SystemType
            {
                get { return typeof(TimeSystem); }
            }
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
                lock (this.heap)
                {
                    this.heap.Add(timeEventArgs);
                    this.UpHeap();
                }
            }

            /// <summary>Returns the highest-priority <see cref="WheelMUD.Core.TimeEvent"/> item without removing it from the heap.</summary>
            /// <returns>The highest-priority <see cref="WheelMUD.Core.TimeEvent"/> item.</returns>
            public TimeEvent Peek()
            {
                lock (this.heap)
                {
                    return (this.heap.Count > 0) ? this.heap[0] : null;
                }
            }

            /// <summary>Returns the highest-priority <see cref="WheelMUD.Core.TimeEvent"/> item and removes it from the heap.</summary>
            /// <returns>The highest-priority <see cref="WheelMUD.Core.TimeEvent"/> item.</returns>
            public TimeEvent Dequeue()
            {
                lock (this.heap)
                {
                    if (this.heap.Count > 0)
                    {
                        var result = this.heap[0];
                        this.heap[0] = this.heap.Last();
                        this.heap.RemoveAt(this.heap.Count - 1);
                        this.DownHeap();
                        return result;
                    }

                    return null;
                }
            }

            /// <summary>Adjusts the heap for the case where a new item was added.</summary>
            private void UpHeap()
            {
                if (this.heap.Count < 2)
                {
                    return;
                }

                int child = this.heap.Count - 1;
                int parent = (child - 1) / 2;
                var newest = this.heap[child];

                while (newest.EndTime < this.heap[parent].EndTime)
                {
                    this.heap[child] = this.heap[parent];
                    child = parent;
                    if (parent == 0)
                    {
                        break;
                    }

                    parent = (parent - 1) / 2;
                }

                this.heap[child] = newest;
            }

            /// <summary>Adjusts the heap for the case where the highest-priority item was removed.</summary>
            private void DownHeap()
            {
                if (this.heap.Count < 2)
                {
                    return;
                }

                int parent = 0;
                int leftChild = 1;
                int rightChild = leftChild + 1;
                var item = this.heap[parent];

                if (rightChild < this.heap.Count - 1 && this.heap[rightChild].EndTime < this.heap[leftChild].EndTime)
                {
                    leftChild = rightChild;
                }

                while (leftChild < this.heap.Count - 1 && this.heap[leftChild].EndTime < item.EndTime)
                {
                    this.heap[parent] = this.heap[leftChild];
                    parent = leftChild;
                    leftChild = (parent * 2) + 1;
                    rightChild = leftChild + 1;

                    if (rightChild < this.heap.Count - 1 && this.heap[rightChild].EndTime < this.heap[leftChild].EndTime)
                    {
                        leftChild = rightChild;
                    }
                }

                this.heap[parent] = item;
            }
        }
    }
}