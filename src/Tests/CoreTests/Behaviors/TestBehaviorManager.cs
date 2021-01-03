//-----------------------------------------------------------------------------
// <copyright file="TestBehaviorManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using NUnit.Framework;
    using System;
    using WheelMUD.Core;

    /// <summary>Tests for the BehaviorManager class.</summary>
    [TestFixture]
    public class TestBehaviorManager
    {
        /// <summary>The behavior manager being tested.</summary>
        private BehaviorManager behaviorManager;

        /// <summary>The thing which will own the BehaviorManager under test.</summary>
        private Thing thing;

        /// <summary>Common preparation for the PlayerBehavior tests.</summary>
        [SetUp]
        public void Init()
        {
            thing = new Thing();
            behaviorManager = new BehaviorManager(thing);
        }

        /// <summary>Tests BehaviorManager support for simultaneous add/remove/iterate attempts.</summary>
        [Test]
        public void TestRapidAddRemoveIterateAcrossThreads()
        {
            // Start with a bunch of behaviors already collected, to work with.
            for (int i = 0; i < 100; i++)
            {
                behaviorManager.Add(new SimpleTestBehavior());
            }

            // Prepare stress threads to determine if BehaviorManager is safe from simultaneous add/remove/iterate attempts.
            DateTime endTime = DateTime.Now.AddMilliseconds(100);
            Action addAction = () => behaviorManager.Add(new SimpleTestBehavior());
            Action removeAction = () =>
            {
                var toRemove = behaviorManager.FindFirst<SimpleTestBehavior>();
                if (toRemove != null)
                {
                    behaviorManager.Remove(toRemove);
                }
            };
            Action iterateAction = () =>
            {
                foreach (var behavior in behaviorManager.AllBehaviors)
                {
                    // Do nothing; just iterate.
                }
            };
            Action typedAction = () =>
            {
                foreach (var behavior in behaviorManager.OfType<SimpleTestBehavior>())
                {
                    // Do nothing; just iterate.
                }
            };
            var addThread = new BackgroundStressTestThread(endTime, addAction);
            var removeThread = new BackgroundStressTestThread(endTime, removeAction);
            var iterateThread = new BackgroundStressTestThread(endTime, iterateAction);
            var typedThread = new BackgroundStressTestThread(endTime, typedAction);

            addThread.Join();
            removeThread.Join();
            iterateThread.Join();
            typedThread.Join();
            Assert.IsNull(addThread.Exception, "Add: " + addThread.ExceptionTestMessage);
            Assert.IsNull(removeThread.Exception, "Remove: " + removeThread.ExceptionTestMessage);
            Assert.IsNull(iterateThread.Exception, "Iterate: " + iterateThread.ExceptionTestMessage);
            Assert.IsNull(typedThread.Exception, "OfType: " + typedThread.ExceptionTestMessage);
        }
    }
}