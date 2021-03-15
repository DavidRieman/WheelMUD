//-----------------------------------------------------------------------------
// <copyright file="OutputBuilderTest.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using WheelMUD.Server;
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
        private readonly string originalMessage = "The big brown dog.";

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
        
        [Test]
        public void TestReplace()
        {
            var replaceWord = "big";
            var replaceWithWord = "small";
            var correctedMessage = "The small brown dog.";
            
            var output = new OutputBuilder().Append(originalMessage);
            output.Replace(replaceWord, replaceWithWord);
            var parsedMessage = output.Parse(terminalOptions);
            
            Assert.AreEqual(correctedMessage, parsedMessage);
        }
        
        [Test]
        public void TestReplaceWithNullNewStr()
        {
            var replaceWord = "big brown";
            var correctedMessage = "The  dog.";
            
            var output = new OutputBuilder().Append(originalMessage);
            output.Replace(replaceWord, null);
            var parsedMessage = output.Parse(terminalOptions);
            Assert.AreEqual( correctedMessage, parsedMessage );
        }
                
        [Test]
        public void TestReplaceWithNullOldStr()
        {
            var output = new OutputBuilder().Append(originalMessage);
            output.Replace(null, null);
            var parsedMessage = output.Parse(terminalOptions);
            Assert.AreEqual( originalMessage, parsedMessage );
        }

        [Test]
        public void TestAppend_IntOverload()
        {
            var testData = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>( "Negative number: ", -100 ),
                new Tuple<string, int>( "Test: ", 100 ),
                new Tuple<string, int>( "Pretty long text: ", 10000 ),
                new Tuple<string, int>( "Very long text with very long number: ", int.MaxValue - 1 ),
                new Tuple<string, int>( "Edge positive: ", int.MaxValue ),
                new Tuple<string, int>( "Edge negative: ", int.MinValue )
            };
            var expectedResults = new List<string>()
            {
                "Negative number: -100",
                "Test: 100",
                "Pretty long text: 10000",
                "Very long text with very long number: 2147483646",
                "Edge positive: 2147483647",
                "Edge negative: -2147483648"
            };
            var actualResults = new List<string>();
            
            foreach ( var testCase in testData )
                actualResults.Add( new OutputBuilder().Append( testCase.Item1 ).Append( testCase.Item2 ).Parse( terminalOptions ) );

            CollectionAssert.AreEqual( expectedResults, actualResults );
        }

        [Test]
        public void TestClear()
        {
            var output = new OutputBuilder().Append( originalMessage );
            var outputBuilderType = output.GetType();
            var bufferPos = outputBuilderType.GetField( "bufferPos", BindingFlags.Instance | BindingFlags.NonPublic );
            
            Assert.AreEqual( originalMessage.Length, bufferPos.GetValue(output) );
            
            output.Clear();

            Assert.AreEqual( 0, bufferPos.GetValue(output) );
        }

        [Test]
        public void TestAppendLine()
        {
            var output = new OutputBuilder().AppendLine( originalMessage );
            var actualMesssage = output.Parse( terminalOptions );
            Assert.AreEqual( originalMessage + "\r\n", actualMesssage );
        }
        
        /// <summary>
        /// For future puprose
        /// </summary>
        // [Test]
        // public void TestAppend_FloatOverload()
        // {
        //     var testData = new List<Tuple<string, float>>()
        //     {
        //         new Tuple<string, float>( "Negative number: ", -100.3424f ),
        //         new Tuple<string, float>( "Test: ", 100.432f ),
        //         new Tuple<string, float>( "Pretty: ", 1.2388888f ),
        //     };
        //     var expectedResults = new List<string>()
        //     {
        //         "Negative number: -100.3424",
        //         "Test: 100.432",
        //         "Pretty: 10000.23",
        //     };
        //     var actualResults = new List<string>();
        //
        //     foreach ( var testCase in testData )
        //         actualResults.Add( new OutputBuilder().Append( testCase.Item1 ).Append( testCase.Item2 ).Parse( terminalOptions ) );
        //
        //     CollectionAssert.AreEqual( expectedResults, actualResults );
        // }

    }
}