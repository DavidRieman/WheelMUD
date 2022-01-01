//-----------------------------------------------------------------------------
// <copyright file="TestSessionStateManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using WheelMUD.Core;

namespace WheelMUD.Tests
{
    /// <summary>Tests the Session class.</summary>
    [TestClass]
    public class TestSessionStateManager
    {
        /// <summary>Common preparation for all Session tests.</summary>
        [TestInitialize]
        public void Init()
        {
            DefaultComposer.Container = new CompositionContainer();
            DefaultComposer.Container.ComposeExportedValue<SessionState>(new TestSession.FakeSessionState());
        }

        /// <summary>Test that automatic composition during singleton instantiation establishes at least one SessionState object.</summary>
        [TestMethod]
        public void TestCompositionFindsSessionStates()
        {
            var sessionStates = SessionStateManager.Instance.SessionStates;
            Assert.IsTrue(sessionStates.Length > 0, "Singleton instantiation should establish at least one SessionState.");
        }
    }
}