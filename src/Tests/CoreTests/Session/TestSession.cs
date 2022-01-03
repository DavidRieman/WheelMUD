//-----------------------------------------------------------------------------
// <copyright file="TestSession.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using TestHelpers;
using WheelMUD.Core;
using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.Tests
{
    /// <summary>Tests the Session class.</summary>
    [TestClass]
    public class TestSession
    {
        /// <summary>Common preparation for all Session tests.</summary>
        [TestInitialize]
        public void Init()
        {
            DefaultComposer.Container = new CompositionContainer();
            DefaultComposer.Container.ComposeExportedValue<SessionState>(new FakeSessionState());
        }

        /// <summary>Test that the initial SessionState, upon establishing a fake connection, is FakeSessionState.</summary>
        [TestMethod]
        public void TestInitialConnectionStateIsNotDefaultState()
        {
            var connection = new MockConnection();
            var session = new Session(connection);
            Assert.AreEqual(session.State.GetType(), typeof(FakeSessionState));
        }

        /// <summary>Tests that the initial connection receives appropriate login prompts.</summary>
        [TestMethod]
        public void TestInitialConnectionPromptsAfterEachWrite()
        {
            var connection = new MockConnection() { AtNewLine = true };
            var session = new Session(connection);

            // Ensure we Begin the session state with some introductory output, followed by the registered prompt.
            Assert.AreEqual(connection.MessagesSent.Count, 1);
            Assert.AreEqual(connection.MessagesSent[0], $"Begin FakeSessionState!{AnsiSequences.NewLine}FakePrompt > ");

            // Ensure writing another string from the prompt cursor position, writes the new text to a new line and can add in the prompt too.
            connection.ResetMessages();
            connection.AtNewLine = false;
            session.Write(new OutputBuilder().AppendLine("test 1"), true);
            Assert.AreEqual(connection.MessagesSent.Count, 1);
            Assert.AreEqual(connection.MessagesSent[0], $"{AnsiSequences.NewLine}test 1{AnsiSequences.NewLine}FakePrompt > ");

            // Ensure writing another string from the prompt cursor position, writes the new text to a new line and can omit adding the prompt too.
            connection.ResetMessages();
            connection.AtNewLine = false;
            session.Write(new OutputBuilder().AppendLine("test 2"), false);
            Assert.AreEqual(connection.MessagesSent.Count, 1);
            Assert.AreEqual(connection.MessagesSent[0], $"{AnsiSequences.NewLine}test 2{AnsiSequences.NewLine}");

            // Ensure writing a string from a new line position already, does not append an extra opening line.
            connection.ResetMessages();
            connection.AtNewLine = true;
            session.Write(new OutputBuilder().Append("test 3"), false);
            Assert.AreEqual(connection.MessagesSent.Count, 1);
            Assert.AreEqual(connection.MessagesSent[0], $"test 3");
        }

        /// <summary>A fake ConnectionState for testing purposes.</summary>
        /// <remarks>TODO: Consider which mocking framework we should use to create such things in a better way.</remarks>
        public class FakeSessionState : SessionState
        {
            /// <summary>Initializes a new instance of the <see cref="FakeSessionState"/> class.</summary>
            /// <param name="session">The session entering this state.</param>
            public FakeSessionState(Session session) : base(session)
            {
            }

            /// <summary>Initializes a new instance of the <see cref="FakeSessionState"/> class.</summary>
            /// <remarks>This constructor is required to support MEF discovery as our default connection state.</remarks>
            public FakeSessionState() : this(null)
            {
            }

            public override void Begin()
            {
                Session.Write(new OutputBuilder().AppendLine("Begin FakeSessionState!"));
            }

            public static string LastProcessedInput { get; private set; }

            public override OutputBuilder BuildPrompt()
            {
                return new OutputBuilder().Append("FakePrompt > ");
            }

            public override void ProcessInput(string command)
            {
                LastProcessedInput = command;
            }
        }
    }
}