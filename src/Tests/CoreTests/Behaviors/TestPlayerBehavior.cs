//-----------------------------------------------------------------------------
// <copyright file="TestPlayerBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Behaviors
{
    using NUnit.Framework;
    using WheelMUD.Core;

    /// <summary>Tests for the ExitBehavior class.</summary>
    [TestFixture]
    public class TestPlayerBehavior
    {
        /// <summary>The behavior being tested.</summary>
        private PlayerBehavior playerBehavior;

        /// <summary>Common preparation for the PlayerBehavior tests.</summary>
        [SetUp]
        public void Init()
        {
            this.playerBehavior = new PlayerBehavior();
        }

        [Test]
        public void TestAddAndRemoveFriends()
        {
            Assert.AreEqual(this.playerBehavior.Friends.Count, 0);
            this.playerBehavior.AddFriend("fufa");
            Assert.AreEqual(this.playerBehavior.Friends.Count, 1);
            this.playerBehavior.AddFriend("fufa");
            Assert.AreEqual(this.playerBehavior.Friends.Count, 1);
            this.playerBehavior.AddFriend("another");
            Assert.AreEqual(this.playerBehavior.Friends.Count, 2);

            this.playerBehavior.RemoveFriend("invalid");
            Assert.AreEqual(this.playerBehavior.Friends.Count, 2);
            this.playerBehavior.RemoveFriend("fufa");
            Assert.AreEqual(this.playerBehavior.Friends.Count, 1);
        }
    }
}