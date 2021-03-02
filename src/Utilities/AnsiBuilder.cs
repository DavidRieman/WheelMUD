//-----------------------------------------------------------------------------
// <copyright file="AnsiBuilder.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace WheelMUD.Utilities
{
    /// <summary>
    /// Mutable String class for Ansi based strings, optimized for speed and memory while retrieving the final result
    /// as a string. Similar use to StringBuilder, but avoid a lot of allocations done by StringBuilder.
    /// </summary>
    public class AnsiBuilder
    {
        /// <summary>
        /// Working mutable string.
        /// </summary>
        private char[] buffer;

        private int bufferPos;
        private int charsCapacity;

        /// <summary>
        /// Temporary string used for the replace method.
        /// </summary>
        private List<char> replacement;

        /// <summary>
        /// Immutable string. Generated at last moment, only if needed.
        /// </summary>
        private string stringGenerated = "";

        /// <summary>
        /// Dirty flag for AnsiBuilder containing ansi that needs parsing.
        /// </summary>
        private bool needsParsing;

        public AnsiBuilder(int initialCapacity = 32)
        {
            buffer = new char[charsCapacity = initialCapacity];
        }

        public int Length => ToString().Length;

        /// <summary>
        /// Returns the string.
        /// </summary>
        public override string ToString()
        {
            if (needsParsing) Parse();
            stringGenerated = new string(buffer, 0, bufferPos);
            return stringGenerated;
        }

        /// <summary>
        /// Reset the char array.
        /// </summary>
        public AnsiBuilder Clear()
        {
            bufferPos = 0;
            needsParsing = false;
            return this;
        }

        /// <summary>
        /// Append a ANSI separator.
        /// </summary>
        /// <param name="design">repeating char of separator</param>
        /// <param name="color">Color of the separator</param>
        /// <param name="bold"></param>
        /// <param name="length">how long the separator is</param>
        /// <returns></returns>
        public AnsiBuilder AppendSeparator(char design = '-', string color = "white", bool bold = false, int length = 60)
        {
            AppendLine(bold
                ? $"<%b%><%{color}%>{new string(design, length)}<%n%>"
                : $"<%{color}%>{new string(design, length)}<%n%>");
            return this;
        }

        /// <summary>
        /// Append a string without memory allocation with ansi new line added.
        /// </summary>
        public AnsiBuilder AppendLine(string value)
        {
            needsParsing = value.Contains("<%");
            value += AnsiSequences.NewLine;
            ReallocateIfn(value.Length);
            var n = value.Length;
            for (var i = 0; i < n; i++)
                buffer[bufferPos + i] = value[i];
            bufferPos += n;
            return this;
        }
        
        /// <summary>
        /// Append a ansi new line.
        /// </summary>
        public AnsiBuilder AppendLine()
        {
            var value = AnsiSequences.NewLine;
            ReallocateIfn(value.Length);
            var n = value.Length;
            for (var i = 0; i < n; i++)
                buffer[bufferPos + i] = value[i];
            bufferPos += n;
            return this;
        }

        /// <summary>
        /// Append a string without memory allocation.
        /// </summary>
        public AnsiBuilder Append(string value)
        {
            needsParsing = value.Contains("<%");
            ReallocateIfn(value.Length);
            var n = value.Length;
            for (var i = 0; i < n; i++)
                buffer[bufferPos + i] = value[i];
            bufferPos += n;
            return this;
        }

        /// <summary>
        /// Append an int without memory allocation.
        /// </summary>
        public AnsiBuilder Append(int value)
        {
            // Allocate enough memory to handle any int number.
            ReallocateIfn(16);

            // Handle the negative case.
            if (value < 0)
            {
                value = -value;
                buffer[bufferPos++] = '-';
            }

            // Copy the digits in reverse order.
            var nbChars = 0;
            do
            {
                buffer[bufferPos++] = (char) ('0' + value % 10);
                value /= 10;
                nbChars++;
            } while (value != 0);

            // Reverse the result.
            for (var i = nbChars / 2 - 1; i >= 0; i--)
            {
                var c = buffer[bufferPos - i - 1];
                buffer[bufferPos - i - 1] = buffer[bufferPos - nbChars + i];
                buffer[bufferPos - nbChars + i] = c;
            }
            
            return this;
        }

        /// <summary>
        /// Append a float without memory allocation.
        /// </summary>
        public AnsiBuilder Append(float valueF)
        {
            double value = valueF;
            // Check we have enough buffer allocated to handle any float number.
            ReallocateIfn(32);

            // Handle the 0 case.
            if (value == 0)
            {
                buffer[bufferPos++] = '0';
                return this;
            }

            // Handle the negative case.
            if (value < 0)
            {
                value = -value;
                buffer[bufferPos++] = '-';
            }

            // Get the 7 meaningful digits as a long.
            var nbDecimals = 0;
            while (value < 1000000)
            {
                value *= 10;
                nbDecimals++;
            }

            var valueLong = (long) Math.Round(value);

            // Parse the number in reverse order.
            var nbChars = 0;
            var isLeadingZero = true;
            while (valueLong != 0 || nbDecimals >= 0)
            {
                // We stop removing leading 0 when non-0 or decimal digit.
                if (valueLong % 10 != 0 || nbDecimals <= 0)
                    isLeadingZero = false;

                // Write the last digit (unless a leading zero).
                if (!isLeadingZero)
                    buffer[bufferPos + nbChars++] = (char) ('0' + valueLong % 10);

                // Add the decimal point.
                if (--nbDecimals == 0 && !isLeadingZero)
                    buffer[bufferPos + nbChars++] = '.';

                valueLong /= 10;
            }

            bufferPos += nbChars;

            // Reverse the result.
            for (var i = nbChars / 2 - 1; i >= 0; i--)
            {
                var c = buffer[bufferPos - i - 1];
                buffer[bufferPos - i - 1] = buffer[bufferPos - nbChars + i];
                buffer[bufferPos - nbChars + i] = c;
            }

            return this;
        }

        /// <summary>
        /// Replace all occurrences of a string by another one.
        /// </summary>
        public AnsiBuilder Replace(string oldStr, string newStr)
        {
            if (bufferPos == 0)
                return this;

            replacement ??= new List<char>();

            // Create the new string into replacement.
            for (var i = 0; i < bufferPos; i++)
            {
                var isToReplace = false;
                // If first character found, check for the rest of the string to replace.
                if (buffer[i] == oldStr[0])
                {
                    var k = 1;
                    while (k < oldStr.Length && buffer[i + k] == oldStr[k])
                        k++;
                    isToReplace = k >= oldStr.Length;
                }

                if (isToReplace)
                {
                    i += oldStr.Length - 1;
                    if (newStr == null) continue;
                    foreach (var t in newStr)
                        replacement.Add(t);
                }
                else
                {
                    replacement.Add(buffer[i]);
                }
            }

            // Copy back the new string into chars.
            ReallocateIfn(replacement.Count - bufferPos);
            for (var k = 0; k < replacement.Count; k++)
                buffer[k] = replacement[k];
            bufferPos = replacement.Count;
            replacement.Clear();
            return this;
        }

        private void ReallocateIfn(int nbCharsToAdd)
        {
            if (bufferPos + nbCharsToAdd <= charsCapacity) return;
            charsCapacity = Math.Max(charsCapacity + nbCharsToAdd, charsCapacity * 2);
            var newChars = new char[charsCapacity];
            buffer.CopyTo(newChars, 0);
            buffer = newChars;
        }

        /// <summary>
        /// Parses a string for special tags and replaces them with ANSI codes.
        /// </summary>
        private void Parse()
        {
            // Seek out all tags and replace them with the equivalent ANSI codes.
            // Always scan for the most common attributes ('normal', 'new line' and foreground color
            // codes) for replacements, but after that we will be more selective about how we spend
            // our time with priority-ordered replacement batches while tokens are still present.
            Replace("<%n%>", AnsiSequences.TextNormal);
            Replace("<%nl%>", AnsiSequences.NewLine);
            Replace("<%black%>", AnsiSequences.ForegroundBlack);
            Replace("<%red%>", AnsiSequences.ForegroundRed);
            Replace("<%green%>", AnsiSequences.ForegroundGreen);
            Replace("<%yellow%>", AnsiSequences.ForegroundYellow);
            Replace("<%blue%>", AnsiSequences.ForegroundBlue);
            Replace("<%magenta%>", AnsiSequences.ForegroundMagenta);
            Replace("<%cyan%>", AnsiSequences.ForegroundCyan);
            Replace("<%white%>", AnsiSequences.ForegroundWhite);

            // Background Colors: only scan for these if we have any "<%b" left, which 
            // most likely indicates a background color setting.
            Replace("<%bblack%>", AnsiSequences.BackgroundBlack);
            Replace("<%bred%>", AnsiSequences.BackgroundRed);
            Replace("<%bgreen%>", AnsiSequences.BackgroundGreen);
            Replace("<%byellow%>", AnsiSequences.BackgroundYellow);
            Replace("<%bblue%>", AnsiSequences.BackgroundBlue);
            Replace("<%bmagenta%>", AnsiSequences.BackgroundMagenta);
            Replace("<%bcyan%>", AnsiSequences.BackgroundCyan);
            Replace("<%bwhite%>", AnsiSequences.BackgroundWhite);


            Replace("<%reset%>", AnsiSequences.TextNormal); // Avoid usage; favor the <%n%> form.
            Replace("<%b%>", AnsiSequences.TextBold);
            Replace("<%cls%>", AnsiSequences.ClearScreenAndHomeCursor);
            Replace("<%u%>", AnsiSequences.TextUnderline);
            Replace("<%underline%>", AnsiSequences.TextUnderline);
            Replace("<%hidden%>", AnsiSequences.TextHidden);
            
            // Cursor Movement
            Replace("<%up%>", AnsiSequences.MoveCursorUp1);
            Replace("<%down%>", AnsiSequences.MoveCursorDown1);
            Replace("<%left%>", AnsiSequences.MoveCursorLeft1);
            Replace("<%right%>", AnsiSequences.MoveCursorRight1);

            // MXP Bits
            // Regex to match <%mxp <!I can be an mxp tag>%> is <\%mxp (.|\n)+?%>
            Replace("<%mxpopenline%>", AnsiSequences.MxpOpenLine);
            Replace("<%mxpsecureline%>", AnsiSequences.MxpSecureLine);

            // Finally, if there are any escaped tags (E.G. as you might wish to use to discuss these tags in
            // game chat or boards or help files or whatnot), convert them to look like their <% %> form.
            Replace("\\<\\%", "<%").Replace("\\%\\>", "%>");
        }
    }
}