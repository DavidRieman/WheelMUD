//-----------------------------------------------------------------------------
// <copyright file="TestBehaviorManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelpers;
using WheelMUD.Core;

namespace WheelMUD.Actions.Tests
{
    [TestClass]
    public class TestDrop
    {
        private MockConnection mockConnection;
        private Session session;
        private Thing actor;
        private Drop drop;

        [TestInitialize]
        public void Init()
        {
            mockConnection = new MockConnection();
            session = new Session(mockConnection);
            drop = new Drop();
            actor = new Thing();
            actor.AddTo(new Thing());
        }

        [TestMethod]
        public void DropNeedsAnActor()
        {
            var actionInput = new ActionInput("drop", session, null);
            string result = drop.Guards(actionInput);
            Assert.IsTrue(result.Contains("can only be performed by an actor"));
        }

        [TestMethod]
        public void DropActorMustBeInWorld()
        {
            var detachedActor = new Thing();
            var actionInput = new ActionInput("drop", session, detachedActor);
            string result = drop.Guards(actionInput);
            Assert.IsTrue(result.Contains("while you are not in the world"));
        }

        [TestMethod]
        public void DropNeedsTarget()
        {
            var actionInput = new ActionInput("drop", session, actor);
            string result = drop.Guards(actionInput);
            Assert.IsTrue(result.Contains("This command needs more than that"));
        }

        // TODO: Verify more guards and actual execution effects.
        // TODO: Verify that all observers get correct messaging, to help fix the dropping actor getting multiple
        //       messages right now... or add said tests to MovableBehavior?
    }
}
