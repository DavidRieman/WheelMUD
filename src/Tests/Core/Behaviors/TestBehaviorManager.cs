//-----------------------------------------------------------------------------
// <copyright file="TestBehaviorManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: March 2014 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using System;
    using System.Linq;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WheelMUD.Core;

    /// <summary>Tests for the BehaviorManager class.</summary>
    [TestFixture]
    [TestClass]
    public class TestBehaviorManager
    {
        /// <summary>The behavior manager being tested.</summary>
        private BehaviorManager behaviorManager;

        /// <summary>The thing which will own the BehaviorManager under test.</summary>
        private Thing thing;

        /// <summary>Common preparation for the PlayerBehavior tests.</summary>
        [TestInitialize]
        [SetUp]
        public void Init()
        {
            this.thing = new Thing();
            this.behaviorManager = new BehaviorManager(this.thing);
        }

        /// <summary>Tests BehaviorManager support for simultaneous add/remove/iterate attempts.</summary>
        [TestMethod]
        [Test]
        public void TestRapidAddRemoveIterateAcrossThreads()
        {
            // Start with a bunch of behaviors already collected, to work with.
            for (int i = 0; i < 100; i++)
            {
                this.behaviorManager.Add(new SimpleTestBehavior());
            }

            // Prepare stress threads to determine if BehaviorManager is safe from simultaneous add/remove/iterate attempts.
            DateTime endTime = DateTime.Now.AddMilliseconds(100);
            Action addAction = () => this.behaviorManager.Add(new SimpleTestBehavior());
            Action removeAction = () =>
            {
                var toRemove = this.behaviorManager.FindFirst<SimpleTestBehavior>();
                if (toRemove != null)
                {
                    this.behaviorManager.Remove(toRemove);
                }
            };
            Action iterateAction = () =>
            {
                foreach (var behavior in this.behaviorManager.AllBehaviors)
                {
                    // Do nothing; just iterate.
                }
            };
            Action typedAction = () =>
            {
                foreach (var behavior in this.behaviorManager.OfType<SimpleTestBehavior>())
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
            Verify.IsNull(addThread.Exception, "Add: " + addThread.ExceptionTestMessage);
            Verify.IsNull(removeThread.Exception, "Remove: " + removeThread.ExceptionTestMessage);
            Verify.IsNull(iterateThread.Exception, "Iterate: " + iterateThread.ExceptionTestMessage);
            Verify.IsNull(typedThread.Exception, "OfType: " + typedThread.ExceptionTestMessage);
        }
    }
}