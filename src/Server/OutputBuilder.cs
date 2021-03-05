//-----------------------------------------------------------------------------
// <copyright file="OutputBuilder.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Utilities;

namespace WheelMUD.Server
{
    /// <summary>
    /// Mutable String class for Ansi based strings, optimized for speed and memory while retrieving the final result
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
        
        private readonly int wordWrapLength;
        
        private readonly bool useAnsi;

        /// <summary>
        /// Temporary string used for the replace method.
        /// </summary>
        private List<char> replacement;

        public OutputBuilder(TerminalOptions terminalOptions, int initialCapacity = 32)
        {
            buffer = new char[charsCapacity = initialCapacity];
            wordWrapLength = terminalOptions.Width;
            useAnsi = terminalOptions.UseANSI;
        }

        /// <summary>
        /// Returns the string.
        /// </summary>
        public override string ToString()
        {
            return Parse();
        }

        /// <summary>
        /// Handles a single line return
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string SingleLine(string input)
        {
            AppendLine(input);
            return Parse();
        }

        /// <summary>
        /// Returns a error message
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public string ErrorMessage(string error)
        {
            AppendSeparator('=', "red", true, wordWrapLength);
            AppendLine($"<%red%>{error}<%n%>");
            AppendSeparator('=', "red", true, wordWrapLength);
            return Parse();
        }
        
        /// <summary>
        /// Returns a warning message
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public string WarningMessage(string error)
        {
            AppendSeparator('=', "yellow", true, wordWrapLength);
            AppendLine($"<%yellow%>{error}<%n%>");
            AppendSeparator('=', "yellow", true, wordWrapLength);
            return Parse();
        }

        /// <summary>
        /// Reset the char array.
        /// </summary>
        public OutputBuilder Clear()
        {
            bufferPos = 0;
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
        public OutputBuilder AppendLine(string value)
        {
            value += "<%nl%>";
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
        public OutputBuilder AppendLine()
        {
            var value = "<%nl%>";
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
        public OutputBuilder Append(string value)
        {
            ReallocateIfn(value.Length);
            var n = value.Length;
            for (var i = 0; i < n; i++)
                buffer[bufferPos + i] = value[i];
            bufferPos += n;

            return this;
        }
        
        ///<summary>Append an int without memory allocation</summary>
        public OutputBuilder Append( int value )
        {
            // Allocate enough memory to handle any int number
            ReallocateIfn( 16 );

            // Handle the negative case
            if( value < 0 )
            {
                value = -value;
                buffer[ bufferPos++ ] = '-';
            }

            // Copy the digits in reverse order
            var nbChars = 0;
            do
            {
                buffer[ bufferPos++ ] = (char)('0' + value%10);
                value /= 10;
                nbChars++;
            } while( value != 0 );

            // Reverse the result
            for( var i=nbChars/2-1; i>=0; i-- )
            {
                var c = buffer[ bufferPos-i-1 ];
                buffer[ bufferPos-i-1 ] = buffer[ bufferPos-nbChars+i ];
                buffer[ bufferPos-nbChars+i ] = c;
            }
            return this;
        }

        ///<summary>Append a float without memory allocation.</summary>
        public OutputBuilder Append( float valueF )
        {
            double value = valueF;
            ReallocateIfn( 32 ); // Check we have enough buffer allocated to handle any float number

            // Handle the 0 case
            if( value == 0 )
            {
                buffer[ bufferPos++ ] = '0';
                return this;
            }

            // Handle the negative case
            if( value < 0 )
            {
                value = -value;
                buffer[ bufferPos++ ] = '-';
            }

            // Get the 7 meaningful digits as a long
            var nbDecimals = 0;
            while( value < 1000000 )
            {
                value *= 10;
                nbDecimals++;
            }
            var valueLong = (long)Math.Round( value );

            // Parse the number in reverse order
            var nbChars = 0;
            var isLeadingZero = true;
            while( valueLong != 0 || nbDecimals >= 0 )
            {
                // We stop removing leading 0 when non-0 or decimal digit
                if( valueLong%10 != 0 || nbDecimals <= 0 )
                    isLeadingZero = false;

                // Write the last digit (unless a leading zero)
                if( !isLeadingZero )
                    buffer[ bufferPos + nbChars++ ] = (char)('0' + valueLong%10);

                // Add the decimal point
                if( --nbDecimals == 0 && !isLeadingZero )
                    buffer[ bufferPos + nbChars++ ] = '.';

                valueLong /= 10;
            }
            bufferPos += nbChars;
            
            // Reverse the result
            for( var i=nbChars/2-1; i>=0; i-- )
            {
                var c = buffer[ bufferPos-i-1 ];
                buffer[ bufferPos-i-1 ] = buffer[ bufferPos-nbChars+i ];
                buffer[ bufferPos-nbChars+i ] = c;
            }

            return this;
        }

        /// <summary>
        /// Replace all occurrences of a string by another one.
        /// </summary>
        public OutputBuilder Replace(string oldStr, string newStr)
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

        private string Parse()
        {
            return useAnsi ? ParseAnsi() : ParseBase();
        }
        
        //TODO: Optimize more
        private string ParseBase()
        {
            var baseResult = "";
            var token = "";
            var charFromNewline = 0;

            var isToken = false;

            for (var i = 0; i < bufferPos; i++)
            {
                if (isToken && buffer[i] == '>')
                {
                    if (buffer[i - 1] == '%')
                    {
                        isToken = false;
                        token = token.Trim('%');
                        if (token == "nl")
                        {
                            charFromNewline = 0;
                        }

                        token = "";
                        continue;
                    }
                }
                
                if (!isToken && buffer[i] == '<')
                {
                    if (buffer[i + 1] == '%')
                    {
                        if (i == 0 || buffer[i - 1] != '\\')
                        {
                            isToken = true;
                            continue;
                        }
                        
                        baseResult = baseResult.Remove(baseResult.Length - 1);
                    }
                }

                if (!isToken)
                {
                    baseResult += buffer[i];
                    charFromNewline++;

                    if (charFromNewline <= wordWrapLength) continue;
                    
                    var lastSpaceIndex = baseResult.LastIndexOf(' ');
                        
                    if (lastSpaceIndex > 0)
                    {
                        baseResult = baseResult.Remove(lastSpaceIndex, 1);
                        charFromNewline = baseResult.Length - lastSpaceIndex;
                        baseResult = baseResult.Insert(lastSpaceIndex, AnsiSequences.NewLine);
                    }
                    else
                    {
                        baseResult += AnsiSequences.NewLine;
                        charFromNewline = 0;
                    }
                }
                else token += buffer[i];
            }

            return baseResult;
        }
        
        //TODO: Optimize more
        private string ParseAnsi()
        {
            var ansiResult = "";
            var token = "";
            var charFromNewline = 0;

            var isToken = false;

            for (var i = 0; i < bufferPos; i++)
            {
                if (isToken && buffer[i] == '>')
                {
                    if (buffer[i - 1] == '%')
                    {
                        isToken = false;
                        token = token.Trim('%');

                        if (token == "nl") charFromNewline = 0;
                        
                        AnsiSequences.TokenMap.TryGetValue(token, out var ansiCode);
                        ansiResult += ansiCode;
                        
                        token = "";
                        continue;
                    }
                }
                
                if (!isToken && buffer[i] == '<')
                {
                    if (buffer[i + 1] == '%')
                    {
                        if (i == 0 || buffer[i - 1] != '\\')
                        {
                            isToken = true;
                            continue;
                        }
                        
                        ansiResult = ansiResult.Remove(ansiResult.Length - 1);
                    }
                }

                if (!isToken)
                {
                    ansiResult += buffer[i];
                    charFromNewline++;

                    if (charFromNewline <= wordWrapLength) continue;
                    
                    var lastSpaceIndex = ansiResult.LastIndexOf(' ');
                        
                    if (lastSpaceIndex > 0)
                    {
                        ansiResult = ansiResult.Remove(lastSpaceIndex, 1);
                        charFromNewline = ansiResult.Length - lastSpaceIndex;
                        ansiResult = ansiResult.Insert(lastSpaceIndex, AnsiSequences.NewLine);
                    }
                    else
                    {
                        ansiResult += AnsiSequences.NewLine;
                        charFromNewline = 0;
                    }
                }
                else token += buffer[i];
            }

            return ansiResult;
        }
    }
}