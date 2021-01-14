//-----------------------------------------------------------------------------
// <copyright file="AnsiSequences.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{

    using System.Text;

    /// <summary>Common ANSI sequences to simplify construction of correct ANSI for adherance to the Telnet protocol.</summary>
    /// <remarks>
    /// Unchanging ANSI sequences should be const where possible to guarantee performance optimizations, as a MUD will be constantly
    /// doing a whole lot of ANSI parsing.
    /// If more direct/dynamic control of the cursor and other ANSI features are needed, check the AnsiHandler helper methods too.
    /// </remarks>
    public class AnsiSequences
    {
        /// <summary>Gets the ANSI sequence to move the cursor to a new line.</summary>
        /// <returns>The ANSI sequence to move the cursor to a new line.</returns>
        /// <remarks>Explicitly avoids Environment.NewLine, as a Telnet server is always supposed to send new lines as CR LF.</remarks>
        public const string NewLine = "\r\n";

        /// <summary>The ANSI 'escape sequence'.</summary>
        public const string Esc = "\x1B";

        /// <summary>The 'clear screen and home cursor' ANSI sequence.</summary>
        public const string ClearScreenAndHomeCursor = Esc + "[2J";

        /// <summary>The 'clear to end of line' ANSI sequence.</summary>
        public const string ClearToEOL = Esc + "K";

        /// <summary>The 'save cursor posision' ANSI sequence.</summary>
        public const string SaveCursorPosition = Esc + "[s";

        /// <summary>The 'load cursor position' ANSI sequence.</summary>
        public const string LoadCursorPosition = Esc + "[u";

        /// <summary>The ANSI sequence to move the cursor down one position.</summary>
        /// <remarks>To move more than one space, try MoveCursorDown() or MoveCursorTo() instead.</remarks>
        public const string MoveCursorDown1 = Esc + "[1B";

        /// <summary>The ANSI sequence to move the cursor left one position.</summary>
        /// <remarks>To move more than one space, try MoveCursorLeft() or MoveCursorTo() instead.</remarks>
        public const string MoveCursorLeft1 = Esc + "[1D";

        /// <summary>The ANSI sequence to move the cursor right one position.</summary>
        /// <remarks>To move more than one space, try MoveCursorRight() or MoveCursorTo() instead.</remarks>
        public const string MoveCursorRight1 = Esc + "[1C";

        /// <summary>The ANSI sequence to move the cursor up one position.</summary>
        /// <remarks>To move more than one space, try MoveCursorUp() or MoveCursorTo() instead.</remarks>
        public const string MoveCursorUp1 = Esc + "[1A";

        /// <summary>The extended ANSI-like sequence for 'MXP open line'.</summary>
        public const string MxpOpenLine = Esc + "[0z";

        /// <summary>The extended ANSI-like sequence for 'MXP secure line'.</summary>
        public const string MxpSecureLine = Esc + "[1z";

        /// <summary>The ANSI sequence for resetting all text to normal attributes.</summary>
        public const string TextNormal = Esc + "[0m";

        /// <summary>The ANSI sequence for setting text to bold.</summary>
        public const string TextBold = Esc + "[1m";

        /// <summary>The ANSI sequence for setting text to underlined.</summary>
        public const string TextUnderline = Esc + "[4m";

        /// <summary>The ANSI sequence for setting text to hidden.</summary>
        /// <remarks>
        /// This may be useful to set when prompting for passwords or other sensitive info. Beware the support for this
        /// might not be universal across all Telnet clients, but it shouldn't hurt to try to use it anyways.
        /// </remarks>
        public const string TextHidden = Esc + "[8m";

        /// <summary>The ANSI sequence for setting foreground (character) color to black.</summary>
        public const string ForegroundBlack = Esc + "[30m";

        /// <summary>The ANSI sequence for setting foreground (character) color to red.</summary>
        public const string ForegroundRed = Esc + "[31m";

        /// <summary>The ANSI sequence for setting foreground (character) color to green.</summary>
        public const string ForegroundGreen = Esc + "[32m";

        /// <summary>The ANSI sequence for setting foreground (character) color to yellow.</summary>
        public const string ForegroundYellow = Esc + "[33m";

        /// <summary>The ANSI sequence for setting foreground (character) color to blue.</summary>
        public const string ForegroundBlue = Esc + "[34m";

        /// <summary>The ANSI sequence for setting foreground (character) color to magenta.</summary>
        public const string ForegroundMagenta = Esc + "[35m";

        /// <summary>The ANSI sequence for setting foreground (character) color to cyan.</summary>
        public const string ForegroundCyan = Esc + "[36m";

        /// <summary>The ANSI sequence for setting foreground (character) color to white.</summary>
        public const string ForegroundWhite = Esc + "[37m";

        /// <summary>The ANSI sequence for setting character background color to black.</summary>
        public const string BackgroundBlack = Esc + "[40m";

        /// <summary>The ANSI sequence for setting character background color to red.</summary>
        public const string BackgroundRed = Esc + "[41m";

        /// <summary>The ANSI sequence for setting character background color to green.</summary>
        public const string BackgroundGreen = Esc + "[42m";

        /// <summary>The ANSI sequence for setting character background color to yellow.</summary>
        public const string BackgroundYellow = Esc + "[43m";

        /// <summary>The ANSI sequence for setting character background color to blue.</summary>
        public const string BackgroundBlue = Esc + "[44m";

        /// <summary>The ANSI sequence for setting character background color to magenta.</summary>
        public const string BackgroundMagenta = Esc + "[45m";

        /// <summary>The ANSI sequence for setting character background color to cyan.</summary>
        public const string BackgroundCyan = Esc + "[46m";

        /// <summary>The ANSI sequence for setting character background color to white.</summary>
        public const string BackgroundWhite = Esc + "[47m";
    }

    public static class StringBuilderExtensions
    {
        /// <summary>Extend StringBuilder class to provide an ANSI line ending version of AppendLine.</summary>
        public static StringBuilder AppendAnsiLine(this StringBuilder sb, string s)
        {
            sb.Append(s + AnsiSequences.NewLine);
            return sb;
        }
    }

}
