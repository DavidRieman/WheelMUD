//-----------------------------------------------------------------------------
// <copyright file="BufferHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Diagnostics;
using System.Text;
using WheelMUD.Utilities;

namespace WheelMUD.Server
{
    /// <summary>Buffer handler.</summary>
    /// <remarks>This class provides a server-side output buffer. It provides paging of large text outputs.</remarks>
    public class BufferHandler
    {
        /// <summary>The single-page overflow indication.</summary>
        /// <remarks>TODO: Add OverflowPromptFormatMXP, with clickable links.</remarks>
        private const string OverflowPromptFormat = @"Paging {0}%: [M]ore, [P]revious, [R]epeat, [A]ll, Enter to quit. > ";

        public static string FormatOverflowPrompt(int currentPosition, int totalRows)
        {
            int percent = currentPosition * 100 / totalRows;
            return string.Format(OverflowPromptFormat, percent);
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
            bool appendOverflowIndicator,
            int currentPosition,
            int totalRows)
        {
            Debug.Assert(totalRows > 0);
            Debug.Assert(currentPosition >= 0);
            Debug.Assert(bufferLines.Length > 0);

            StringBuilder output = new StringBuilder();
            for (int i = 0; i < bufferLines.Length - 1; i++)
            {
                output.AppendAnsiLine(bufferLines[i]);
            }
            // Last line without NewLine; should always be a prompt (for output modes that would support this style of buffering)
            // and the prompt can have the cursor beside it like most regular, user-comfortable shell-like experiences.
            output.Append(bufferLines[bufferLines.Length - 1]);

            if (appendOverflowIndicator)
            {
                output.AppendAnsiLine();
                output.AppendAnsiLine(FormatOverflowPrompt(currentPosition, totalRows));
            }

            return output.ToString();
        }
    }
}