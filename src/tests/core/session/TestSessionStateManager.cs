//-----------------------------------------------------------------------------
// <copyright file="TestSessionStateManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Session
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WheelMUD.Core;

    /// <summary>Tests the Session class.</summary>
    [TestFixture]
    [TestClass]
    public class TestSessionStateManager
    {
        /// <summary>Common preparation for all Session tests.</summary>
        [TestInitialize]
        [SetUp]
        public void Init()
        {
        }
        
        /// <summary>Test that automatic composition during singleton instantiation establishes at least one SessionState object.</summary>
        [TestMethod]
        [Test]
        public void TestCompositionFindsSessionStates()
        {
            var sessionStates = SessionStateManager.Instance.SessionStates;
            Verify.IsTrue(sessionStates.Length > 0, "Singleton instantiation should establish at least one SessionState.");
        }
    }
}