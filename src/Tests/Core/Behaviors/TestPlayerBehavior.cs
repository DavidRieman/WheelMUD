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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WheelMUD.Core;

    /// <summary>Tests for the ExitBehavior class.</summary>
    [TestFixture]
    [TestClass]
    public class TestPlayerBehavior
    {
        /// <summary>The behavior being tested.</summary>
        private PlayerBehavior playerBehavior;

        /// <summary>Common preparation for the PlayerBehavior tests.</summary>
        [TestInitialize]
        [SetUp]
        public void Init()
        {
            this.playerBehavior = new PlayerBehavior();
        }

        [TestMethod]
        [Test]
        public void TestAddAndRemoveFriends()
        {
            Verify.AreEqual(this.playerBehavior.Friends.Count, 0);
            this.playerBehavior.AddFriend("fufa");
            Verify.AreEqual(this.playerBehavior.Friends.Count, 1);
            this.playerBehavior.AddFriend("fufa");
            Verify.AreEqual(this.playerBehavior.Friends.Count, 1);
            this.playerBehavior.AddFriend("another");
            Verify.AreEqual(this.playerBehavior.Friends.Count, 2);

            this.playerBehavior.RemoveFriend("invalid");
            Verify.AreEqual(this.playerBehavior.Friends.Count, 2);
            this.playerBehavior.RemoveFriend("fufa");
            Verify.AreEqual(this.playerBehavior.Friends.Count, 1);
        }
    }
}