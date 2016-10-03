//-----------------------------------------------------------------------------
// <copyright file="BufferHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   This class provides a server side output buffer, IE it wraps the text to
//   a predefined number of lines.
//   Created: March 2007 by Foxedup
//   MOdified: November 2009 by BenGecko for extended more capabilities.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Output
{
    using System;
    using System.Text;

    /// <summary>Buffer handler.</summary>
    public class BufferHandler
    {
        /// <summary>The single-page overflow indication.</summary>
        private const string OverflowIndicator = @"<%mxpsecureline%>Paging {0}%: [M]ore, [P]revious, [R]epeat, [A]ll, Enter to quit.";

        /// <summary>
        /// Parses the text and checks to see if the number of lines is over the threshold
        /// if it is then it splits the text and returns the first portion. The rest of the
        /// text is passed back to the calling code in the out parameter BufferedText
        /// </summary>
        /// <param name="originalText">The text to parse.</param>
        /// <returns>Text that conforms to the buffer specified.</returns>
        public static string[] Parse(string originalText)
        {
            string[] lines = originalText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            return lines;
        }

        /// <summary>
        /// Parses the text and checks to see if the number of lines is over the threshold
        /// if it is then it splits the text and returns the first portion. The rest of the
        /// text is passed back to the calling code in the out parameter BufferedText
        /// </summary>
        /// <param name="bufferLines">The text to parse.</param>
        /// <param name="appendLastLineFeed">Indicates if the last line should be appended with crlf.</param>
        /// <param name="appendOverflowIndicator">Indicates if the last line should be appended with the more indicator.</param>
        /// <param name="currentPosition">Current position in the list.</param>
        /// <param name="totalRows">Total rows in the output buffer.</param>
        /// <returns>Returns the text to return to the user.</returns>
        public static string Format(
            string[] bufferLines,
            bool appendLastLineFeed,
            bool appendOverflowIndicator,
            int currentPosition,
            int totalRows)
        {
            if (bufferLines.Length > 0)
            {
                StringBuilder output = new StringBuilder();

                for (int i = 0; i < bufferLines.Length - 1; i++)
                {
                    output.AppendLine(bufferLines[i]);
                }

                if (appendLastLineFeed || appendOverflowIndicator)
                {
                    output.AppendLine(bufferLines[bufferLines.Length - 1]);
                }
                else
                {
                    output.Append(bufferLines[bufferLines.Length - 1]);
                }

                if (appendOverflowIndicator)
                {
                    if (totalRows != 0)
                    {
                        int percent = Convert.ToInt32(currentPosition * 100 / totalRows);
                        output.AppendLine(string.Format(OverflowIndicator, percent));
                    }
                    else
                    {
                        output.AppendLine(string.Format(OverflowIndicator, string.Empty));
                    }
                }

                return output.ToString();
            }

            return null;
        }
    }
}