//-----------------------------------------------------------------------------
// <copyright file="TestPlayerBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using WheelMUD.Server;
using NUnit.Framework;
using WheelMUD.Utilities;

namespace WheelMUD.Tests.Output
{
    /// <summary>Tests for the OutputBuilder/Parser class.</summary>
    [TestFixture]
    public class OutputBuilderTest
    {
        private TerminalOptions terminalOptions;

        private readonly string baseMessage = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea <%leave%>commodo<%leave%> consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
        private readonly string ansiMessage = "<%red%>Lorem<%n%> ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea \\<%leave%>commodo\\<%leave%> consequat. Duis aute irure dolor in reprehenderit in <%blue%>voluptate<%n%> velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est <%yellow%>laborum.<%yellow%>";

        /// <summary>Preparation for output parsing params</summary>
        [SetUp]
        public void Init()
        {
            terminalOptions = new TerminalOptions();
            
        }

        [Test]
        public void TestWordwrap()
        {
            terminalOptions.Width = 80;
            
            var output = new OutputBuilder().Append(baseMessage);
            var parsedMessage = output.Parse(terminalOptions);
            
            var splitStrings = parsedMessage.Split(new[]{AnsiSequences.NewLine}, StringSplitOptions.None);

            foreach (var split in splitStrings)
            {
                Assert.Less(split.Length,terminalOptions.Width);
            }
        }

        [Test]
        public void TestAnsiTokenRemoval()
        {
            terminalOptions.UseANSI = false;
            terminalOptions.UseWordWrap = false;
            
            var output = new OutputBuilder().Append(ansiMessage);
            var parsedMessage = output.Parse(terminalOptions);
            
            Assert.AreEqual(baseMessage, parsedMessage);
        }
    }
}