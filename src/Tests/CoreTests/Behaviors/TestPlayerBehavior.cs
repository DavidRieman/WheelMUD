//-----------------------------------------------------------------------------
// <copyright file="TestPlayerBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using WheelMUD.Core;

namespace WheelMUD.Tests.Behaviors
{
    /// <summary>Tests for the ExitBehavior class.</summary>
    [TestClass]
    public class TestPlayerBehavior
    {
        /// <summary>The behavior being tested.</summary>
        private PlayerBehavior playerBehavior;

        /// <summary>Common preparation for the PlayerBehavior tests.</summary>
        [TestInitialize]
        public void Init()
        {
            playerBehavior = new PlayerBehavior();
        }

        [TestMethod]
        public void TestAddAndRemoveFriends()
        {
            Assert.AreEqual(playerBehavior.Friends.Count, 0);
            playerBehavior.AddFriend("fufa");
            Assert.AreEqual(playerBehavior.Friends.Count, 1);
            playerBehavior.AddFriend("fufa");
            Assert.AreEqual(playerBehavior.Friends.Count, 1);
            playerBehavior.AddFriend("another");
            Assert.AreEqual(playerBehavior.Friends.Count, 2);

            playerBehavior.RemoveFriend("invalid");
            Assert.AreEqual(playerBehavior.Friends.Count, 2);
            playerBehavior.RemoveFriend("fufa");
            Assert.AreEqual(playerBehavior.Friends.Count, 1);
        }
    }
}