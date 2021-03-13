//-----------------------------------------------------------------------------
// <copyright file="OutputBuilder.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Temporary string used for the replace method.
        /// </summary>
        private List<char> replacement;

        public OutputBuilder(int initialCapacity = 32)
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
            AppendSeparator('=', "red", true, error.Length);
            AppendLine($"<%red%>{error}<%n%>");
            AppendSeparator('=', "red", true, error.Length);
        }
        
        /// <summary>
        /// Returns a warning message
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public void WarningMessage(string error)
        {
            AppendSeparator('=', "yellow", true, error.Length);
            AppendLine($"<%yellow%>{error}<%n%>");
            AppendSeparator('=', "yellow", true, error.Length);
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
        
        ///<summary>Append an int without memory allocation</summary>
        public OutputBuilder Append( int value )
        {
            // Allocate enough memory to handle any int number
            ReallocateIfn( 16 );

            var minVal = false;
            
            // Handle the negative case
            if( value < 0 )
            {
                if (value == int.MinValue)
                {
                    minVal = true;
                    value += 1;
                }
                
                value = -value;
                buffer[bufferPos++] = '-';
            }

            // Copy the digits in reverse order
            var chars = 0;
            do
            {
                buffer[ bufferPos++ ] = (char)('0' + value%10);
                value /= 10;
                chars++;
            } while( value != 0 );

            // Reverse the result
            for( var i=chars/2-1; i>=0; i-- )
            {
                var c = buffer[ bufferPos-i-1 ];
                buffer[ bufferPos-i-1 ] = buffer[ bufferPos-chars+i ];
                buffer[ bufferPos-chars+i ] = c;
                
                if(i == 0 && minVal)
                    buffer[ bufferPos-i-1 ] = '8';
            }

            return this;
        }

        //TODO: Fix.
        ///<summary>Append a float without memory allocation.</summary>
        // public OutputBuilder Append( float valueF )
        // {
        //     double value = valueF;
        //     ReallocateIfn( 32 ); // Check we have enough buffer allocated to handle any float number
        //
        //     // Handle the 0 case
        //     if( value == 0 )
        //     {
        //         buffer[ bufferPos++ ] = '0';
        //         return this;
        //     }
        //
        //     // Handle the negative case
        //     if( value < 0 )
        //     {
        //         value = -value;
        //         buffer[ bufferPos++ ] = '-';
        //     }
        //
        //     // Get the 7 meaningful digits as a long
        //     var decimals = 0;
        //     while( value < 1000000 )
        //     {
        //         value *= 10;
        //         decimals++;
        //     }
        //     var valueLong = (long)Math.Round( value );
        //
        //     // Parse the number in reverse order
        //     var chars = 0;
        //     var isLeadingZero = true;
        //     while( valueLong != 0 || decimals >= 0 )
        //     {
        //         // We stop removing leading 0 when non-0 or decimal digit
        //         if( valueLong%10 != 0 || decimals <= 0 )
        //             isLeadingZero = false;
        //
        //         // Write the last digit (unless a leading zero)
        //         if( !isLeadingZero )
        //             buffer[ bufferPos + chars++ ] = (char)('0' + valueLong%10);
        //
        //         // Add the decimal point
        //         if( --decimals == 0 && !isLeadingZero )
        //             buffer[ bufferPos + chars++ ] = '.';
        //
        //         valueLong /= 10;
        //     }
        //     bufferPos += chars;
        //     
        //     // Reverse the result
        //     for( var i=chars/2-1; i>=0; i-- )
        //     {
        //         var c = buffer[ bufferPos-i-1 ];
        //         buffer[ bufferPos-i-1 ] = buffer[ bufferPos-chars+i ];
        //         buffer[ bufferPos-chars+i ] = c;
        //     }
        //
        //     return this;
        // }

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