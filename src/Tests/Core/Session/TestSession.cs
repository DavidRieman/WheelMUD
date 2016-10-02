//-----------------------------------------------------------------------------
// <copyright file="TestSession.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests.Session
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NUnit.Framework;
    using WheelMUD.Core;
    using WheelMUD.Interfaces;

    /// <summary>Tests the Session class.</summary>
    [TestFixture]
    [TestClass]
    public class TestSession
    {
        /// <summary>Common preparation for all Session tests.</summary>
        [TestInitialize]
        [SetUp]
        public void Init()
        {
        }

        [TestMethod]
        [Test]
        public void TestInitialConnectionStateIsNotDefaultState()
        {
            string endl = Environment.NewLine;
            var connection = new FakeConnection();
            var session = new Session(connection);

            var typeName = session.State.GetType().Name ?? "(null)";
            Verify.AreEqual(typeName, "ConnectedState");
        }

        /// <summary>Tests that the initial connection receives appropriate login prompts.</summary>
        [TestMethod]
        [Test]
        public void TestInitialConnectionPromptsAfterEachWrite()
        {
            string endl = Environment.NewLine;
            var connection = new FakeConnection();
            var session = new Session(connection);

            // Send a prompt through the connection. (This should be asking which character to login, or to create a new one.)
            var prompt = session.State.BuildPrompt();
            Verify.IsTrue(prompt.Length > 0);
            Verify.AreEqual(1, connection.FakeMessagesSent.Count);

            connection.Reset();

            session.Write("test 1", true);
            Verify.AreEqual(1, connection.FakeMessagesSent.Count);
            Verify.AreEqual("test 1" + endl + prompt, connection.FakeMessagesSent[0]);

            connection.Reset();

            session.Write("test 2", false);
            Verify.AreEqual(1, connection.FakeMessagesSent.Count);
            Verify.AreEqual(endl + "test 2", connection.FakeMessagesSent[0]);

            connection.Reset();

            session.Write("test 3a");
            session.Write("test 3b");
            Verify.AreEqual(2, connection.FakeMessagesSent.Count);
            Verify.AreEqual("test 3a" + endl + prompt, connection.FakeMessagesSent[0]);
            Verify.AreEqual(endl + "test 3b" + endl + prompt, connection.FakeMessagesSent[1]);
        }

        private class FakeConnection : IConnection
        {
            public FakeConnection()
            {
                this.Reset();
            }

            public List<string> FakeMessagesSent { get; set; }

            public string ID
            {
                get { throw new NotImplementedException(); }
            }

            public string LastRawInput
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public System.Net.IPAddress CurrentIPAddress
            {
                get { throw new NotImplementedException(); }
            }

            public Core.Output.OutputBuffer OutputBuffer
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public ITerminal Terminal
            {
                get { throw new NotImplementedException(); }
            }

            public ITelnetCodeHandler TelnetCodeHandler
            {
                get { throw new NotImplementedException(); }
            }

            public byte[] Data
            {
                get { throw new NotImplementedException(); }
            }

            public StringBuilder Buffer
            {
                get { throw new NotImplementedException(); }
            }

            public int PagingRowLimit
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public string LastInputTerminator
            {
                get { throw new NotImplementedException(); }
                set { throw new NotImplementedException(); }
            }

            public void Reset()
            {
                this.FakeMessagesSent = new List<string>();
            }

            public void Disconnect()
            {
                throw new NotImplementedException();
            }

            public void Send(byte[] data)
            {
                this.Send(Encoding.ASCII.GetString(data));
            }

            public void Send(string data)
            {
                this.FakeMessagesSent.Add(data);
            }

            public void Send(string data, bool bypassDataFormatter)
            {
                throw new NotImplementedException();
            }

            public void Send(string data, bool bypassDataFormatter, bool sendAllData)
            {
                throw new NotImplementedException();
            }

            public void ProcessBuffer(Core.Enums.BufferDirection bufferDirection)
            {
                throw new NotImplementedException();
            }
        }
    }
}