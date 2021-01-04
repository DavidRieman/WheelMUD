//-----------------------------------------------------------------------------
// <copyright file="AnsiHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>Class For parsing MUD ANSI tags into ANSI.</summary>
    /// <remarks>
    /// ANSI sequences and ANSI building methods should generally be avoided for normal, language-based MUDs. However, they are marked public
    /// here for special use cases: For example, they may be useful for interactive editor modes (long description or mail editing) or for
    /// rogue-like semi-graphical game interfaces that want constant direct cursor placement, or things like that.
    /// </remarks>
    public static class AnsiHandler
    {
        /// <summary>Gets the ANSI sequence to move the cursor up the specified number of lines.</summary>
        /// <param name="numLines">The number of lines to move the cursor up.</param>
        /// <returns>The ANSI sequence to move the cursor up the specified number of lines.</returns>
        public static string MoveCursorUp(int numLines)
        {
            return AnsiSequences.Esc + string.Format("[{0}A", numLines);
        }

        /// <summary>Gets the ANSI sequence to move the cursor down the specified number of lines.</summary>
        /// <param name="numberLines">The number of lines to move the cursor down.</param>
        /// <returns>The ANSI sequence to move the cursor down the specified number of lines.</returns>
        public static string MoveCursorDown(int numberLines)
        {
            return AnsiSequences.Esc + string.Format("[{0}B", numberLines);
        }

        /// <summary>Gets the ANSI sequence to move the cursor right the specified number of columns.</summary>
        /// <param name="numberColumns">The number of columns to move the cursor right.</param>
        /// <returns>The ANSI sequence to move the cursor right the specified number of columns.</returns>
        public static string MoveCursorRight(int numberColumns)
        {
            return AnsiSequences.Esc + string.Format("[{0}C", numberColumns);
        }

        /// <summary>Gets the ANSI sequence to move the cursor left the specified number of columns.</summary>
        /// <param name="numberColumns">The number of columns to move the cursor left.</param>
        /// <returns>The ANSI sequence to move the cursor left the specified number of columns.</returns>
        public static string MoveCursorLeft(int numberColumns)
        {
            return AnsiSequences.Esc + string.Format("[{0}D", numberColumns);
        }

        /// <summary>Gets the ANSI sequence to set the foreground to the specified color.</summary>
        /// <param name="foregroundColor">Which foreground color to set.</param>
        /// <returns>The ANSI sequence to set the foreground to the specified color.</returns>
        public static string SetForegroundColor(AnsiForegroundColor foregroundColor)
        {
            return AnsiSequences.Esc + string.Format("[{0}m", (int)foregroundColor);
        }

        /// <summary>Gets the ANSI sequence to set the background to the specified color.</summary>
        /// <param name="backgroundColor">Which background color to set.</param>
        /// <returns>The ANSI sequence to set the background to the specified color.</returns>
        public static string SetBackgroundColor(AnsiBackgroundColor backgroundColor)
        {
            return AnsiSequences.Esc + string.Format("[{0}m", (int)backgroundColor);
        }

        /// <summary>Gets the ANSI sequence to set all of the attribute, foreground, and background colors.</summary>
        /// <param name="attribute">Which attribute to set.</param>
        /// <param name="foregroundColor">Which foreground color to set.</param>
        /// <param name="backgroundColor">Which background color to set.</param>
        /// <returns>The ANSI sequence to set all of the attribute, foreground, and background colors.</returns>
        public static string SetTextAttributes(AnsiTextAttribute attribute, AnsiForegroundColor foregroundColor, AnsiBackgroundColor backgroundColor)
        {
            return AnsiSequences.Esc + string.Format("[{0};{1};{2}m", (int)attribute, (int)foregroundColor, (int)backgroundColor);
        }

        /// <summary>Gets the ANSI sequence to set the text attribute.</summary>
        /// <param name="attribute">Which attribute to set.</param>
        /// <returns>The ANSI sequence to set the text attribute.</returns>
        public static string SetTextAttributes(AnsiTextAttribute attribute)
        {
            return AnsiSequences.Esc + string.Format("[{0}m", (int)attribute);
        }

        /// <summary>Gets the ANSI sequence to set the text foreground and background colors.</summary>
        /// <param name="foregroundColor">Which foreground color to set.</param>
        /// <param name="backgroundColor">Which background color to set.</param>
        /// <returns>The ANSI sequence to set the text foreground and background colors.</returns>
        public static string SetTextAttributes(AnsiForegroundColor foregroundColor, AnsiBackgroundColor backgroundColor)
        {
            return AnsiSequences.Esc + string.Format("[{0};{1}m", (int)foregroundColor, (int)backgroundColor);
        }

        /// <summary>Gets the ANSI sequence to set the cursor to the specified coordinates.</summary>
        /// <param name="line">Which line to set the cursor on.</param>
        /// <param name="column">Which column to set the cursor on.</param>
        /// <returns>The ANSI sequence to set the cursor to the specified coordinates.</returns>
        public static string MoveCursorTo(int line, int column)
        {
            return AnsiSequences.Esc + string.Format("[{0};{1}H", line, column);
        }

        /// <summary>Parses a string for special tags and replaces them with ANSI codes.</summary>
        /// <param name="parseString">The string to scan for special tags.</param>
        /// <returns>A string with all special tags replaced with matching ANSI codes.</returns>
        public static string Parse(string parseString)
        {
            // TODO: Consider replacing with regex (if fast enough), and separating ANSI from MXP handling.
            // TODO: Another approach to consider and compare with benchmarking may be to split the
            //   string into tokens (where "<%" and "%>" become named tokens) and replace any sequence
            //   of "open-tag, string, close-tag" tokens with the appropriate ANSI sequence, and then sew
            //   all the strings back together (if the token sequence was deemed valid).
            string newString = parseString;
            if (newString != null)
            {
                // Seek out all tags and replace them with the equivalent ANSI codes.
                // Always scan for the most common attributes ('normal', 'new line' and foreground color
                // codes) for replacements, but after that we will be more selective about how we spend
                // our time with priority-ordered replacement batches while tokens are still present.
                newString = newString.Replace("<%n%>", AnsiSequences.TextNormal)
                    .Replace("<%nl%>", AnsiSequences.NewLine)
                    .Replace("<%black%>", AnsiSequences.ForegroundBlack)
                    .Replace("<%red%>", AnsiSequences.ForegroundRed)
                    .Replace("<%green%>", AnsiSequences.ForegroundGreen)
                    .Replace("<%yellow%>", AnsiSequences.ForegroundYellow)
                    .Replace("<%blue%>", AnsiSequences.ForegroundBlue)
                    .Replace("<%magenta%>", AnsiSequences.ForegroundMagenta)
                    .Replace("<%cyan%>", AnsiSequences.ForegroundCyan)
                    .Replace("<%white%>", AnsiSequences.ForegroundWhite);

                // Background Colors: only scan for these if we have any "<%b" left, which 
                // most likely indicates a background color setting.
                if (newString.Contains("<%b"))
                {
                    newString = newString.Replace("<%bblack%>", AnsiSequences.BackgroundBlack)
                        .Replace("<%bred%>", AnsiSequences.BackgroundRed)
                        .Replace("<%bgreen%>", AnsiSequences.BackgroundGreen)
                        .Replace("<%byellow%>", AnsiSequences.BackgroundYellow)
                        .Replace("<%bblue%>", AnsiSequences.BackgroundBlue)
                        .Replace("<%bmagenta%>", AnsiSequences.BackgroundMagenta)
                        .Replace("<%bcyan%>", AnsiSequences.BackgroundCyan)
                        .Replace("<%bwhite%>", AnsiSequences.BackgroundWhite);
                }

                // Tags for clearing and setting attributes.
                if (newString.Contains("<%"))
                {
                    newString = newString.Replace("<%reset%>", AnsiSequences.TextNormal) // Avoid usage; favor the <%n%> form.
                        .Replace("<%b%>", AnsiSequences.TextBold)
                        .Replace("<%cls%>", AnsiSequences.ClearScreenAndHomeCursor)
                        .Replace("<%u%>", AnsiSequences.TextUnderline)
                        .Replace("<%underline%>", AnsiSequences.TextUnderline);
                }

                // Finally, tackle the more awkward and less common tags, if any tags remain.
                if (newString.Contains("<%"))
                {
                    // Cursor Movement
                    newString = newString.Replace("<%up%>", AnsiSequences.MoveCursorUp1)
                        .Replace("<%down%>", AnsiSequences.MoveCursorDown1)
                        .Replace("<%left%>", AnsiSequences.MoveCursorLeft1)
                        .Replace("<%right%>", AnsiSequences.MoveCursorRight1);

                    // MXP Bits
                    // Regex to match <%mxp <!I can be an mxp tag>%> is <\%mxp (.|\n)+?%>
                    newString = newString.Replace("<%mxpopenline%>", AnsiSequences.MxpOpenLine)
                        .Replace("<%mxpsecureline%>", AnsiSequences.MxpSecureLine);
                }

                return newString;
            }

            return string.Empty;
        }
    }
}