//-----------------------------------------------------------------------------
// <copyright file="OutputBuilder.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;

namespace WheelMUD.Server
{
    /// <summary>
    /// Custom mutable String class, optimized for speed and memory while retrieving the final result
    /// as a string. Similar use to StringBuilder, but avoid a lot of allocations done by StringBuilder.
    /// </summary>
    public class OutputBuilder
    {
        /// <summary>
        /// Working mutable string.
        /// </summary>
        private char[] buffer;

        private int bufferPos;
        private int charsCapacity;
        
        //Static Char Numbers
        private static readonly char[] CharNumbers = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

        private static readonly string[] CharBool = {"False", "True"};

        /// <summary>
        /// Temporary string used for the replace method.
        /// </summary>
        private List<char> replacement;
        
        /// <summary>
        /// Quickly gets buffer length of int
        /// </summary>
        /// <param name="n"></param>
        /// <returns>buffer length</returns>
        private static int GetIntLength(long n)
        {
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

        /// <summary>
        /// Returns a error message
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public void ErrorMessage(string error)
        {
            AppendLine($"<%error%>{error}");
        }
        
        /// <summary>
        /// Returns a warning message
        /// </summary>
        /// <param name="warning"></param>
        /// <returns></returns>
        public void WarningMessage(string warning)
        {
            AppendLine($"<%warning%>{warning}");
        }

        /// <summary>
        /// Reset the char array.
        /// </summary>
        public void Clear()
        {
            bufferPos = 0;
        }

        /// <summary>
        /// Append a ANSI separator.
        /// </summary>
        /// <param name="design">repeating char of separator</param>
        /// <param name="color">Color of the separator</param>
        /// <param name="bold"></param>
        /// <param name="length">how long the separator is</param>
        /// <returns></returns>
        public OutputBuilder AppendSeparator(char design = '-', string color = "white", bool bold = false,
            int length = 60)
        {
            AppendLine(bold
                ? $"<%b%><%{color}%>{new string(design, length)}<%n%>"
                : $"<%{color}%>{new string(design, length)}<%n%>");
            return this;
        }

        /// <summary>
        /// Append a string without memory allocation with ansi new line added.
        /// </summary>
        public OutputBuilder AppendLine(string value = null)
        {
            value += "<%nl%>";
            ReallocateIfn(value.Length);
            var length = value.Length;
            for (var i = 0; i < length; i++)
                buffer[bufferPos + i] = value[i];
            bufferPos += length;
            
            return this;
        }

        /// <summary>
        /// Append a string without memory allocation.
        /// </summary>
        public OutputBuilder Append(string value)
        {
            ReallocateIfn(value.Length);
            var length = value.Length;
            for (var i = 0; i < length; i++)
                buffer[bufferPos + i] = value[i];
            bufferPos += length;

            return this;
        }

        ///<summary>Append bool without memory allocation</summary>
        public OutputBuilder Append(bool value)
        {
            return Append(value ? CharBool[1] : CharBool[0]);
        }

        ///<summary>Append an int without memory allocation</summary>
        public OutputBuilder Append(int value)
        {
            return Append((long) value);
        }
        
        ///<summary>Converts int to long to handle int.MinValue</summary>
        private OutputBuilder Append(long value)
        {
            var isNegative = false;

            if (value < 0)
            {
                value = -value;
                isNegative = true;
            }
            
            var length = GetIntLength(value);
            
            // Allocate enough memory to handle this int
            ReallocateIfn(length);

            // Handle the negative case
            if(isNegative)
                buffer[ bufferPos++ ] = '-';

            if (value <= 9)
            {
                //between 0-9
                buffer[bufferPos++] = CharNumbers[value];
                return this;
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
        

        public OutputBuilder Append( float value )
        {
            Append(value.ToString(CultureInfo.InvariantCulture));
            return this;
        }

        /// <summary>
        /// Replace all occurrences of a string by another one.
        /// </summary>
        public OutputBuilder Replace(string oldStr, string newStr)
        {
            if (bufferPos == 0 || oldStr == null)
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

        private void ReallocateIfn(int charsToAdd)
        {
            if (bufferPos + charsToAdd <= charsCapacity) return;
            charsCapacity = Math.Max(charsCapacity + charsToAdd, charsCapacity * 2);
            var newChars = new char[charsCapacity];
            buffer.CopyTo(newChars, 0);
            buffer = newChars;
        }

        public string Parse(TerminalOptions terminalOptions)
        {
            return OutputParser.Parse(buffer, bufferPos, terminalOptions);
        }
    }
}