//-----------------------------------------------------------------------------
// <copyright file="AnsiHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace WheelMUD.Utilities
{
    /// <summary>Class For parsing MUD ANSI tags into ANSI.</summary>
    /// <remarks>
    /// ANSI sequences and ANSI building methods should generally be avoided for normal, language-based MUDs. However, they are marked public
    /// here for special use cases: For example, they may be useful for interactive editor modes (long description or mail editing) or for
    /// rogue-like semi-graphical game interfaces that want constant direct cursor placement, or things like that.
    /// </remarks>
    public static class AnsiHandler
    {
        // TODO: Consider separating ANSI from MXP handling.
        // NOTE: AnsiSequences.MxpSecureLine MUST NOT be honored from automatic conversion; e.g. cannot support "<%mxpsecureline%>" conversion.
        private static readonly Dictionary<string, string> CodeSequenceMap = new Dictionary<string, string>()
        {
            { "n", AnsiSequences.TextNormal },
            { "reset", AnsiSequences.TextNormal }, // Avoid usage; favor the <%n%> form.
            { "nl", AnsiSequences.NewLine },
            { "black", AnsiSequences.ForegroundBlack },
            { "red", AnsiSequences.ForegroundRed },
            { "green", AnsiSequences.ForegroundGreen },
            { "yellow", AnsiSequences.ForegroundYellow },
            { "blue", AnsiSequences.ForegroundBlue },
            { "magenta", AnsiSequences.ForegroundMagenta },
            { "cyan", AnsiSequences.ForegroundCyan },
            { "white", AnsiSequences.ForegroundWhite },
            { "bblack", AnsiSequences.BackgroundBlack },
            { "bred", AnsiSequences.BackgroundRed },
            { "bgreen", AnsiSequences.BackgroundGreen },
            { "byellow", AnsiSequences.BackgroundYellow },
            { "bblue", AnsiSequences.BackgroundBlue },
            { "bmagenta", AnsiSequences.BackgroundMagenta },
            { "bcyan", AnsiSequences.BackgroundCyan },
            { "bwhite", AnsiSequences.BackgroundWhite },
            { "b", AnsiSequences.TextBold },
            { "cls", AnsiSequences.ClearScreenAndHomeCursor },
            { "u", AnsiSequences.TextUnderline },
            { "underline", AnsiSequences.TextUnderline },
            { "hidden", AnsiSequences.TextHidden },
            { "up", AnsiSequences.MoveCursorUp1 },
            { "down", AnsiSequences.MoveCursorDown1 },
            { "left", AnsiSequences.MoveCursorLeft1 },
            { "right", AnsiSequences.MoveCursorRight1 },
        };

        /// <summary>Gets the ANSI sequence to move the cursor up the specified number of lines.</summary>
        /// <param name="numLines">The number of lines to move the cursor up.</param>
        /// <returns>The ANSI sequence to move the cursor up the specified number of lines.</returns>
        public static string MoveCursorUp(int numLines)
        {
            return $"{AnsiSequences.Esc}[{numLines}A";
        }

        /// <summary>Gets the ANSI sequence to move the cursor down the specified number of lines.</summary>
        /// <param name="numberLines">The number of lines to move the cursor down.</param>
        /// <returns>The ANSI sequence to move the cursor down the specified number of lines.</returns>
        public static string MoveCursorDown(int numberLines)
        {
            return $"{AnsiSequences.Esc}[{numberLines}B";
        }

        /// <summary>Gets the ANSI sequence to move the cursor right the specified number of columns.</summary>
        /// <param name="numberColumns">The number of columns to move the cursor right.</param>
        /// <returns>The ANSI sequence to move the cursor right the specified number of columns.</returns>
        public static string MoveCursorRight(int numberColumns)
        {
            return $"{AnsiSequences.Esc}[{numberColumns}C";
        }

        /// <summary>Gets the ANSI sequence to move the cursor left the specified number of columns.</summary>
        /// <param name="numberColumns">The number of columns to move the cursor left.</param>
        /// <returns>The ANSI sequence to move the cursor left the specified number of columns.</returns>
        public static string MoveCursorLeft(int numberColumns)
        {
            return $"{AnsiSequences.Esc}[{numberColumns}D";
        }

        /// <summary>Gets the ANSI sequence to set the foreground to the specified color.</summary>
        /// <param name="foregroundColor">Which foreground color to set.</param>
        /// <returns>The ANSI sequence to set the foreground to the specified color.</returns>
        public static string SetForegroundColor(AnsiForegroundColor foregroundColor)
        {
            return $"{AnsiSequences.Esc}[{(int)foregroundColor}m";
        }

        /// <summary>Gets the ANSI sequence to set the background to the specified color.</summary>
        /// <param name="backgroundColor">Which background color to set.</param>
        /// <returns>The ANSI sequence to set the background to the specified color.</returns>
        public static string SetBackgroundColor(AnsiBackgroundColor backgroundColor)
        {
            return $"{AnsiSequences.Esc}[{(int)backgroundColor}m";
        }

        /// <summary>Gets the ANSI sequence to set all of the attribute, foreground, and background colors.</summary>
        /// <param name="attribute">Which attribute to set.</param>
        /// <param name="foregroundColor">Which foreground color to set.</param>
        /// <param name="backgroundColor">Which background color to set.</param>
        /// <returns>The ANSI sequence to set all of the attribute, foreground, and background colors.</returns>
        public static string SetTextAttributes(AnsiTextAttribute attribute, AnsiForegroundColor foregroundColor, AnsiBackgroundColor backgroundColor)
        {
            return $"{AnsiSequences.Esc}[{(int)attribute};{(int)foregroundColor};{(int)backgroundColor}m";
        }

        /// <summary>Gets the ANSI sequence to set the text attribute.</summary>
        /// <param name="attribute">Which attribute to set.</param>
        /// <returns>The ANSI sequence to set the text attribute.</returns>
        public static string SetTextAttributes(AnsiTextAttribute attribute)
        {
            return $"{AnsiSequences.Esc}[{(int)attribute}m";
        }

        /// <summary>Gets the ANSI sequence to set the text foreground and background colors.</summary>
        /// <param name="foregroundColor">Which foreground color to set.</param>
        /// <param name="backgroundColor">Which background color to set.</param>
        /// <returns>The ANSI sequence to set the text foreground and background colors.</returns>
        public static string SetTextAttributes(AnsiForegroundColor foregroundColor, AnsiBackgroundColor backgroundColor)
        {
            return $"{AnsiSequences.Esc}[{(int)foregroundColor};{(int)backgroundColor}m";
        }

        /// <summary>Gets the ANSI sequence to set the cursor to the specified coordinates.</summary>
        /// <param name="line">Which line to set the cursor on.</param>
        /// <param name="column">Which column to set the cursor on.</param>
        /// <returns>The ANSI sequence to set the cursor to the specified coordinates.</returns>
        public static string MoveCursorTo(int line, int column)
        {
            return $"{AnsiSequences.Esc}[{line};{column}H";
        }

        /// <summary>Converts an ANSI code name to the ANSI code sequence suitable for sending to the Telnet client.</summary>
        /// <param name="code">The code name, such as "red" or "underline".</param>
        /// <returns>The ANSI code sequence, or null when the code is unrecognized.</returns>
        public static string ConvertCode(string code)
        {
            return CodeSequenceMap.ContainsKey(code) ? CodeSequenceMap[code] : null;
        }
    }
}