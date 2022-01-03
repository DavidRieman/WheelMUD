//-----------------------------------------------------------------------------
// <copyright file="OutputBuilderTest.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.Tests.Output
{
    /// <summary>Tests for the OutputBuilder/Parser class.</summary>
    [TestClass]
    public class OutputBuilderTest
    {
        private TerminalOptions terminalOptions;

        private readonly string baseMessage = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea <%leave%>commodo<%leave%> consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        private readonly string ansiMessage = "<%red%>Lorem<%n%> ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea \\<%leave%>commodo\\<%leave%> consequat. Duis aute irure dolor in reprehenderit in <%blue%>voluptate<%n%> velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est <%yellow%>laborum.<%yellow%>";
        private readonly string shortMessage = "The big brown dog.";

        [TestInitialize]
        public void Init()
        {
            terminalOptions = new TerminalOptions();
        }

        [TestMethod]
        public void TestWordwrap()
        {
            terminalOptions.Width = 80;

            var output = new OutputBuilder().Append(baseMessage);
            var parsedMessage = output.Parse(terminalOptions);

            var splitStrings = parsedMessage.Split(new[] { AnsiSequences.NewLine }, StringSplitOptions.None);

            foreach (var split in splitStrings)
            {
                Assert.IsTrue(split.Length < terminalOptions.Width);
            }
        }

        [TestMethod]
        public void TestAnsiTokenRemoval()
        {
            terminalOptions.UseANSI = false;
            terminalOptions.UseWordWrap = false;

            var output = new OutputBuilder().Append(ansiMessage);
            var parsedMessage = output.Parse(terminalOptions);

            Assert.AreEqual(baseMessage, parsedMessage);
        }

        [TestMethod]
        public void TestReplace()
        {
            var replaceWord = "big";
            var replaceWithWord = "small";
            var correctedMessage = "The small brown dog.";

            var output = new OutputBuilder().Append(shortMessage);
            output.Replace(replaceWord, replaceWithWord);
            var parsedMessage = output.Parse(terminalOptions);

            Assert.AreEqual(correctedMessage, parsedMessage);
        }

        [TestMethod]
        public void TestReplaceWithNullNewStr()
        {
            var replaceWord = "big brown";
            var correctedMessage = "The  dog.";

            var output = new OutputBuilder().Append(shortMessage);
            output.Replace(replaceWord, null);
            var parsedMessage = output.Parse(terminalOptions);
            Assert.AreEqual(correctedMessage, parsedMessage);
        }

        [TestMethod]
        public void TestReplaceWithNullOldStr()
        {
            var output = new OutputBuilder().Append(shortMessage);
            output.Replace(null, null);
            var parsedMessage = output.Parse(terminalOptions);
            Assert.AreEqual(shortMessage, parsedMessage);
        }

        [TestMethod]
        public void TestAppendInt0()
        {
            Assert.AreEqual("0", new OutputBuilder().Append(0).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendInt1()
        {
            Assert.AreEqual("1", new OutputBuilder().Append(1).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendIntPositive()
        {
            Assert.AreEqual("100", new OutputBuilder().Append(100).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendIntBig()
        {
            Assert.AreEqual("111222", new OutputBuilder().Append(111222).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendIntNearMax()
        {
            Assert.AreEqual("2147483646", new OutputBuilder().Append(int.MaxValue - 1).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendIntMax()
        {
            Assert.AreEqual("2147483647", new OutputBuilder().Append(int.MaxValue).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendIntNegative1()
        {
            Assert.AreEqual("-1", new OutputBuilder().Append(-1).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendIntNegative()
        {
            Assert.AreEqual("-100", new OutputBuilder().Append(-100).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendIntNegativeSmall()
        {
            Assert.AreEqual("-111222", new OutputBuilder().Append(-111222).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendIntNearMin()
        {
            Assert.AreEqual("-2147483647", new OutputBuilder().Append(int.MinValue + 1).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestAppendIntMin()
        {
            Assert.AreEqual("-2147483648", new OutputBuilder().Append(int.MinValue).Parse(terminalOptions));
        }

        [TestMethod]
        public void TestMultipleParseProducesSameResults()
        {
            var output = new OutputBuilder().Append(shortMessage);
            var result1 = output.Parse(terminalOptions);
            Assert.AreEqual(result1, shortMessage);

            var result2 = output.Parse(terminalOptions);
            Assert.AreEqual(result1, result2);
        }

        [TestMethod]
        public void TestClear()
        {
            var output = new OutputBuilder().Append(shortMessage);
            Assert.AreNotEqual(output.Parse(terminalOptions), "");
            output.Clear();
            Assert.AreEqual(output.Parse(terminalOptions), "");
        }

        [TestMethod]
        public void TestAppendLine()
        {
            var output = new OutputBuilder().AppendLine(shortMessage);
            // Should build line with \r\n (per Telnet spec) regardless of the environment running this test; Do not apply Environment.NewLine to code here.
            Assert.AreEqual(shortMessage + "\r\n", output.Parse(terminalOptions));
        }
    }
}