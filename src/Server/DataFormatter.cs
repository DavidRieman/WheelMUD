//-----------------------------------------------------------------------------
// <copyright file="DataFormatter.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using WheelMUD.Core;

namespace WheelMUD.Server
{
    /// <summary>The data formatter. Runs data through our series of data filters for correct presentation on the client.</summary>
    public static class DataFormatter
    {
        private static readonly string LexerRegex = $"(<%|%>|\\<\\%|\\%\\>|{AnsiSequences.NewLineRegexPartEscaped})";

        /// <summary>Runs the data through our presentation handlers.</summary>
        /// <param name="data">The data to format; may include colorization codes, explicit newlines, etc.</param>
        /// <param name="wordWrapWidth">If positive, the number of characters to perform word wrapping against. Else, skips word wrapping.</param>
        /// <param name="useANSI">If true, prepare the output with ANSI codes, else just stript ANSI tokens out.</param>
        /// <returns>The formatted lines of text, suitable to send to a Telnet client.</returns>
        public static List<string> FormatData(string data, int wordWrapWidth, bool useANSI)
        {
            // We'll use a lightweight lexer to split out data like "<%red%>Foo\r\nBar" into parts like "<%", "red", "%>", "Foo", "\r\n", "Bar",
            // and then handle each section. Typically such a process would abstract the sections as "tokens" with an enumeration, but given our
            // limited set of recognized token patterns and the high call frequency for this method, we'll forego such overhead if we can.
            var parts = Regex.Split(data, LexerRegex);
            var length = parts.Length;

            // When using server-side word-wrapping, we should reserve enough space to be prepared for strings of size up to the word wrap width.
            var lineBuilder = wordWrapWidth > 0 ? new StringBuilder(wordWrapWidth) : new StringBuilder();
            var outputLines = new List<string>();
            int wordWrapPosition = 0;

            // Track whether the last append was a word, to avoid adding a space between an ANSI code and text.
            // For example, "Roses are <%red%>red<%n%>," should print text and word wrap as length "Roses are red," without injecting any extra
            // spaces before "red" or between "red" and the comma, etc.
            bool lastAppendWasWord = false;

            // TODO: https://github.com/DavidRieman/WheelMUD/issues/106 - Need unit tests, and need to solve extra spaces between text and color codes, etc.
            for (int i = 0; i < length; i++)
            {
                var part = parts[i];
                if (part == string.Empty)
                {
                    // The Regex.Split can generate these between tokens / around new lines, but there is no further work we should do for them.
                }
                else if (part == "<%" && (i + 2) < length && parts[i + 2] == "%>")
                {
                    // If we started with <% but didn't meet the other conditions, we'll just end up treating each part as regular text and sending it to
                    // the client. This won't look great to a user (when unintended), but will help identify broken ANSI code markers in an obvious way.
                    // If this part is <% and we have %> two parts up, treat this as an ANSI code marker.
                    if (useANSI)
                    {
                        lineBuilder.Append(AnsiHandler.ConvertCode(parts[i + 1]));
                        lastAppendWasWord = false;
                    }
                    i += 2; // We've handled 3 parts instead of 1 on this pass (even if we aren't printing anything when useANSI is false).
                }
                else if (part == AnsiSequences.NewLine)
                {
                    outputLines.Add(lineBuilder.ToString());
                    lineBuilder.Clear();
                    wordWrapPosition = 0;
                }
                else // Anything that isn't recognized above will be treated as text to render to the client.
                {
                    if (part == "\\<\\%" && (i + 2) < length && parts[i + 2] == "%>")
                    {
                        // If we are using escaping to output text like "<%red%>" to be actually displayed to the user instead of processed as an ANSI
                        // sequence, then build this word from these parts and use the regular word-wrapping output algorithm to render it.
                        part = $"<%{parts[i + 1]}%>";
                        i += 2;
                    }

                    var words = part.Split(' ');
                    foreach (string word in words)
                    {
                        // If we are appending a word to an existing output line (rather than starting a fresh line after a word-wrap), and the last append
                        // was also a word, inject a space between the words.
                        if (wordWrapPosition > 0 && lastAppendWasWord && wordWrapPosition + 1 < wordWrapWidth)
                        {
                            lineBuilder.Append(' ');
                            lastAppendWasWord = false;
                            wordWrapPosition++;
                        }

                        // Each empty section actually represents an extra space mid-string, or on the outer edges of the string.
                        // For example, if we want output to begin with and include extra spaces like: "  Routes:  east, west" then the split values will be:
                        // ["", "", "Routes", "", "east,", "west"].
                        if (word == string.Empty)
                        {
                            // Just append the extra space, and ensure we don't add yet another one when we do get to printing another real word.
                            // Avoid the spaces though if we're at the end of the line (the client's terminal printing width), so we don't accidentally force a
                            // client to wrap the extra spaces onto the following one. We want the following line to start with the next freshly-wrapped word.
                            if (wordWrapPosition + 1 < wordWrapWidth)
                            {
                                lineBuilder.Append(' ');
                            }
                            lastAppendWasWord = false;
                        }
                        else
                        {
                            // If adding this word would flow to a new line, and isn't a single massive block spanning a whole line on its own, move to a new line.
                            var wordLength = word.Length;
                            if (wordWrapPosition > 0 && wordWrapPosition + wordLength + 1 > wordWrapWidth)
                            {
                                outputLines.Add(lineBuilder.ToString());
                                lineBuilder.Clear();
                                wordWrapPosition = 0;
                            }

                            lineBuilder.Append(word);
                            lastAppendWasWord = true;
                            wordWrapPosition += wordLength;
                        }
                    }
                }
            }

            // Add the final line, if applicable. There are a couple cases where this will typically occur:
            // 1) The data ended with a display string that was not ended with a NewLine Token. In this scenario, it may be a Prompt or something
            //    that we actually do want to print, even without an explicit NewLine. This will allow the user-typed text to potentially echo next
            //    to the relevant prompt instead of on a new line. We can detect this scenario with wordWrapPosition > 0.
            // 2) The data ended with an explicit NewLine, and we just finished processing a "" section put in by the Regex.Split. In this scenario,
            //    even though wordWrapPosition is 0, we do want to add the blank line to our output lines, to signal where we want the cursor to go.
            //    We can detect this scenario by inspecting the second-last section for the NewLine.
            // 3) The data moved to a new line, followed by just an ANSI code, but useANSI is off so we didn't render anything (so missed case 1).
            //    We can detect this scenario by inspecting the second last section for the ANSI closing tag. Treating this case correctly should
            //    help ANSI and non-ANSI clients both have the same NewLine pacing for a more consistent experience (though one is single-color).
            // The main case where we need to be most careful _not_ to add an extra NewLine is at the end of a Prompt, which may often be a data 
            // string like "> <%n%>" to end with an ANSI reset right at the end. This example should have built up "> " and an ANSI escape sequence
            // into the lineBuilder, so it should trigger case 1 to finish the active line without adding any _extra_ NewLine after that.
            if (wordWrapPosition > 0 || (length >= 2 && (parts[length - 2] == AnsiSequences.NewLine || parts[length - 2] == "%>")))
            {
                outputLines.Add(lineBuilder.ToString());
            }

            return outputLines;
        }
    }
}