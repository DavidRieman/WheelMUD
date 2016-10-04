//-----------------------------------------------------------------------------
// <copyright file="TestActionInput.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WheelMUD.Core;

    /// <summary>Tests the base ActionInput class.</summary>
    /// <remarks>In particular, the input parsing method that is called from within the ActionInput constructor.</remarks>
    [TestFixture]
    [TestClass]
    public class TestActionInput
    {
        /// <summary>Common preparation for all ActionInput tests.</summary>
        [TestInitialize]
        [SetUp]
        public void Init()
        {
        }

        /// <summary>Test parsing of command text.</summary>
        /// <remarks>
        /// Currently this sends a null IController to the ActionInput constructor, but AFAIK the
        /// constructor is not expected to handle this cleanly (although it is nice). If the null
        /// causes this test to fail in the future, it might be the test that needs to be fixed
        /// to use a fake or mock IController.
        /// </remarks>
        [TestMethod]
        [Test]
        public void TestParseText()
        {
            // Test empty string
            var actionInput = new ActionInput(string.Empty, null);
            Verify.IsNull(actionInput.Noun);
            Verify.IsNull(actionInput.Tail);
            Verify.IsNull(actionInput.Params);

            // Test simple 1-word command
            var oneWordInput = new ActionInput("look", null);
            Verify.AreEqual(oneWordInput.Noun, "look");
            Verify.IsNull(actionInput.Tail);
            Verify.IsNull(actionInput.Params);

            // Test 2-word command
            var twoWordInput = new ActionInput("look foo", null);
            Verify.AreEqual(twoWordInput.Noun, "look");
            Verify.AreEqual(twoWordInput.Tail, "foo");
            Verify.IsNotNull(twoWordInput.Params);
            Verify.AreEqual(twoWordInput.Params.Length, 1);

            // Test 2-word command
            var threeWordInput = new ActionInput("create consumable metal", null);
            Verify.AreEqual(threeWordInput.Noun, "create");
            Verify.AreEqual(threeWordInput.Tail, "consumable metal");
            Verify.IsNotNull(threeWordInput.Params);
            Verify.AreEqual(threeWordInput.Params.Length, 2);

            // Test input with leading/trailing spaces
            var spacedWordInput = new ActionInput(" look  foo ", null);
            Verify.AreEqual(spacedWordInput.Noun, "look");
            Verify.AreEqual(spacedWordInput.Tail, "foo");
            Verify.IsNotNull(spacedWordInput.Params);
            Verify.AreEqual(spacedWordInput.Params.Length, 1);
        }
    }
}