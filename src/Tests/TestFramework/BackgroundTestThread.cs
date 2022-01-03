//-----------------------------------------------------------------------------
// <copyright file="BackgroundTestThread.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests
{
    using System;
    using System.Threading;

    /// <summary>A background test thread, for testing purposes.</summary>
    public class BackgroundTestThread
    {
        private Thread thread;

        /// <summary>Initializes a new instance of the BackgroundTestThread class.</summary>
        /// <param name="action">The action to perform on a background thread.</param>
        /// <param name="autoStart">If true, automatically starts the thread.</param>
        public BackgroundTestThread(Action action, bool autoStart = true)
        {
            ThreadStart wrappedAction = () =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Exception = ex;
                }
            };
            thread = new Thread(wrappedAction)
            {
                IsBackground = true,
            };

            if (autoStart)
            {
                Start();
            }
        }

        /// <summary>Gets the exception which occurred on the BackgroundTestThread, if any.</summary>
        public Exception Exception { get; private set; }

        /// <summary>Gets a message describing the Exception, as used for test assertion messages.</summary>
        public string ExceptionTestMessage
        {
            get
            {
                return Exception == null ? "No exception." : "Got exception: " + Exception.ToString();
            }
        }

        /// <summary>Starts running this thread.</summary>
        public void Start()
        {
            thread.Start();
        }

        /// <summary>Blocks the calling thread until this thread terminates.</summary>
        public void Join()
        {
            thread.Join();
        }
    }
}