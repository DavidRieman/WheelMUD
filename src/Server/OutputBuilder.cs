//-----------------------------------------------------------------------------
// <copyright file="OutputBuilder.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WheelMUD.Server
{
    /// <summary>
    /// Custom mutable String class, optimized for speed and memory while retrieving the final result
    /// as a string. Similar use to StringBuilder, but avoid a lot of allocations done by StringBuilder.
    /// </summary>
    public class OutputBuilder
    {
        /// <summary>The mutable output string we are building.</summary>
        private char[] buffer;

        private int bufferPos;
        private int charsCapacity;
        private static readonly char[] CharNumbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        /// <summary>Temporary string used for the replace method.</summary>
        private List<char> replacement;

        /// <summary>Quickly get the buffer length required for an int.</summary>
        /// <param name="n">The source int.</param>
        /// <returns>The required buffer length to fit this int as a string.</returns>
        private static int GetIntLength(int n)
        {
            if (n < 0) throw new NotImplementedException("GetIntLength does not support negative numbers yet.");
            if (n < 10) return 1;
            if (n < 100) return 2;
            if (n < 1000) return 3;
            if (n < 10000) return 4;
            if (n < 100000) return 5;
            if (n < 1000000) return 6;
            if (n < 10000000) return 7;
            if (n < 100000000) return 8;
            if (n < 1000000000) return 9;
            return 10;
        }

        public OutputBuilder(int initialCapacity = 16)
        {
            buffer = new char[charsCapacity = initialCapacity];
        }

        /// <summary>Treat the buffer as empty.</summary>
        /// <remarks>Moves the buffer cursor but does not reduce the buffer memory. (Buffer memory will not be reclaimed until the OutputBuilder itself is GC'd.)</remarks>
        /// <returns>This OutputBuilder, for fluent syntax options.</returns>
        public OutputBuilder Clear()
        {
            bufferPos = 0;
            return this;
        }

        /// <summary>Append an ANSI separator.</summary>
        /// <param name="design">Repeating char of separator.</param>
        /// <param name="color">Color of the separator.</param>
        /// <param name="bold">If true, uses bold color mode.</param>
        /// <param name="length">How long the separator is.</param>
        /// <returns>This OutputBuilder, for fluent syntax options.</returns>
        public OutputBuilder AppendSeparator(char design = '-', string color = "white", bool bold = false,
            int length = 60)
        {
            AppendLine(bold
                ? $"<%b%><%{color}%>{new string(design, length)}<%n%>"
                : $"<%{color}%>{new string(design, length)}<%n%>");
            return this;
        }

        /// <summary>Append a string with ANSI New Line added, without memory allocation (if possible).</summary>
        public OutputBuilder AppendLine(string value = null)
        {
            value += "<%nl%>";
            ReallocateIfn(value.Length);
            var length = value.Length;
            for (var i = 0; i < length; i++)
            {
                // OutputBuffer / OutputParser should be the only place writing ANSI NewLine character sequences, to help guarantee correct Telnet protocol handling.
                // (Telnet protocol says new lines sent should be CR LF regardless of what OS the server is running on and what OS the client is running on.)
                Debug.Assert(value[i] != '\r' && value[i] != '\n', "Output should not include explicit newline characters. Use <%nl%> or OutputBuffer WriteLine functions instead.");
                buffer[bufferPos + i] = value[i];
            }
            bufferPos += length;

            return this;
        }

        /// <summary>Append a string to the output (without memory allocation, if possible).</summary>
        public OutputBuilder Append(string value)
        {
            ReallocateIfn(value.Length);
            var length = value.Length;
            for (var i = 0; i < length; i++)
            {
                buffer[bufferPos + i] = value[i];
            }
            bufferPos += length;

            return this;
        }

        /// <summary>Append bool to the output (without memory allocation, if possible).</summary>
        public OutputBuilder Append(bool value)
        {
            return Append(value ? "true" : "false");
        }

        /// <summary>Append an int to the output (without memory allocation, if possible).</summary>
        /// <remarks>TO DO: An Append(long value) could follow the same pattern, but with a more robuse "GetLongLength".</remarks>
        public OutputBuilder Append(int value)
        {
            var isNegative = false;

            if (value < 0)
            {
                // Special rare edge case, since absolute min value cannot just swap sign due to different magnitudes:
                // Handle through slower but definitely-correct string append instead.
                if (value == int.MinValue)
                {
                    return Append(int.MinValue.ToString());
                }

                value = -value;
                isNegative = true;
            }

            // Get the length of what is now the absolute value.
            var length = GetIntLength(value);

            // Allocate enough memory to handle this int, plus a negative dash, when needed.
            ReallocateIfn(length + (isNegative ? 1 : 0));

            // Append the negative.
            if (isNegative)
            {
                buffer[bufferPos++] = '-';
            }

            bufferPos += length;
            var nbChars = bufferPos - 1;

            do
            {
                buffer[nbChars--] = CharNumbers[value % 10];
                value /= 10;
            } while (value != 0);

            return this;
        }

        /// <summary>For other types not specifically covered, simply convert the type to string and append it.</summary>
        /// <remarks>Primarily this is meant to cover "float", "long", unsigned variants, and so on. These will not benefit from the speed boosts of custom implementations.</remarks>
        public OutputBuilder Append<T>(T value)
        {
            return Append(value.ToString());
        }

        /// <summary>Replace all occurrences of a string with another one.</summary>
        public OutputBuilder Replace(string oldStr, string newStr)
        {
            if (bufferPos == 0 || oldStr == null)
            {
                return this;
            }

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
                    {
                        k++;
                    }
                    isToReplace = k >= oldStr.Length;
                }

                if (isToReplace)
                {
                    i += oldStr.Length - 1;
                    if (newStr == null) continue;
                    foreach (var t in newStr)
                    {
                        replacement.Add(t);
                    }
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

        public string Parse(TerminalOptions terminalOptions)
        {
            return OutputParser.Parse(buffer, bufferPos, terminalOptions);
        }

        private void ReallocateIfn(int charsToAdd)
        {
            if (bufferPos + charsToAdd <= charsCapacity) return;
            charsCapacity = Math.Max(charsCapacity + charsToAdd, charsCapacity * 2);
            var newChars = new char[charsCapacity];
            buffer.CopyTo(newChars, 0);
            buffer = newChars;
        }
    }
}