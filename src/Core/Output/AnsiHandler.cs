//-----------------------------------------------------------------------------
// <copyright file="AnsiHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Purpose: ANSI/VT100 Handlers
//   Use Telnet Negoations to Determin TermType and 
//   Use The Proper Class to handle that Terms TermType
//   *Edited by themime (Will) December 5th, 2010:
//              -Renamed COLORS to AnsiForegroundColor changed values to Capital from UPPERCASE
//              -Renamed ATTR to Attribute, changed values to Capital from UPPERCASE
//              -Moved ansi background colors into their own enum. Changed to Capital from UPPERCASE and removed BG prefix.
//              -Made appropriate refactoring changes   
//              
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Output
{
    using System;

    /// <summary>ANSI color codes.</summary>
    public enum AnsiForegroundColor
    {
        // DEFAULT FG COLORS

        /// <summary>ANSI color code for black.</summary>
        Black = 30,

        /// <summary>ANSI color code for red.</summary>
        Red = 31,

        /// <summary>ANSI color code for green.</summary>
        Green = 32,

        /// <summary>ANSI color code for yellow.</summary>
        Yellow = 33,

        /// <summary>ANSI color code for blue.</summary>
        Blue = 34,

        /// <summary>ANSI color code for magenta.</summary>
        Magenta = 35,

        /// <summary>ANSI color code for cyan.</summary>
        Cyan = 36,

        /// <summary>ANSI color code for white.</summary>
        White = 37
    }

    // Background Colors              
    public enum AnsiBackgroundColor
    {
        /// <summary>ANSI color code for black background.</summary>
        Black = 40,

        /// <summary>ANSI color code for red background.</summary>
        Red = 41,

        /// <summary>ANSI color code for green background.</summary>
        Green = 42,

        /// <summary>ANSI color code for yellow background.</summary>
        Yellow = 43,

        /// <summary>ANSI color code for blue background.</summary>
        Blue = 44,

        /// <summary>ANSI color code for magenta background.</summary>
        Magenta = 45,

        /// <summary>ANSI color code for cyan background.</summary>
        Cyan = 46,

        /// <summary>ANSI color code for white background.</summary>
        White = 47
    }

    /// <summary>ANSI attributes.</summary>
    public enum AnsiAttribute
    {
        /// <summary>Normal attribute.</summary>
        Normal = 0,

        /// <summary>Bold attribute.</summary>
        Bold = 1,

        /// <summary>Underline attribute.</summary>
        Underline = 4
    }

    /// <summary>Class For parsing mud ansi Tags into ansi.</summary>
    public static class AnsiHandler
    {
        /// <summary>The ANSI 'escape sequence'.</summary>
        private const string Esc = "\x1B";

        /// <summary>Gets the 'save cursor posision' ANSI sequence.</summary>
        public static string SaveCursorPosition
        {
            get { return Esc + "[s"; }
        }

        /// <summary>Gets the 'load cursor posision' ANSI sequence.</summary>
        public static string LoadCursorPosition
        {
            get { return Esc + "[u"; }
        }

        /// <summary>Gets the 'clear screen and home cursor' ANSI sequence.</summary>
        public static string ClearScreenAndHomeCursor
        {
            get { return Esc + "[2J"; }
        }

        /// <summary>Gets the 'clear to end of line' ANSI sequence.</summary>
        public static string ClearToEOL
        {
            get { return Esc + "K"; }
        }

        /// <summary>Gets the ANSI sequence to move the cursor up the specified number of lines.</summary>
        /// <param name="numLines">The number of lines to move the cursor up.</param>
        /// <returns>The ANSI sequence to move the cursor up the specified number of lines.</returns>
        public static string MoveCursorUp(int numLines)
        {
            return Esc + string.Format("[{0}A", numLines);
        }

        /// <summary>Gets the ANSI sequence to move the cursor down the specified number of lines.</summary>
        /// <param name="numberLines">The number of lines to move the cursor down.</param>
        /// <returns>The ANSI sequence to move the cursor down the specified number of lines.</returns>
        public static string MoveCursorDown(int numberLines)
        {
            return Esc + string.Format("[{0}B", numberLines);
        }

        /// <summary>Gets the ANSI sequence to move the cursor right the specified number of columns.</summary>
        /// <param name="numberColumns">The number of columns to move the cursor right.</param>
        /// <returns>The ANSI sequence to move the cursor right the specified number of columns.</returns>
        public static string MoveCursorRight(int numberColumns)
        {
            return Esc + string.Format("[{0}C", numberColumns);
        }

        /// <summary>Gets the ANSI sequence to move the cursor left the specified number of columns.</summary>
        /// <param name="numberColumns">The number of columns to move the cursor left.</param>
        /// <returns>The ANSI sequence to move the cursor left the specified number of columns.</returns>
        public static string MoveCursorLeft(int numberColumns)
        {
            return Esc + string.Format("[{0}D", numberColumns);
        }

        /// <summary>Gets the ANSI sequence to move the cursor to a new line.</summary>
        /// <returns>The ANSI sequence to move the cursor to a new line.</returns>
        public static string MoveCursorToNewLine()
        {
            return Environment.NewLine;
        }

        /// <summary>Gets the ANSI sequence to set the foreground to the specified color.</summary>
        /// <param name="foregroundColor">Which foreground color to set.</param>
        /// <returns>The ANSI sequence to set the foreground to the specified color.</returns>
        public static string SetForegroundColor(AnsiForegroundColor foregroundColor)
        {
            return Esc + string.Format("[{0}m", (int)foregroundColor);
        }

        /// <summary>Gets the ANSI sequence to set the background to the specified color.</summary>
        /// <param name="backgroundColor">Which background color to set.</param>
        /// <returns>The ANSI sequence to set the background to the specified color.</returns>
        public static string SetBackgroundColor(AnsiBackgroundColor backgroundColor)
        {
            return Esc + string.Format("[{0}m", (int)backgroundColor);
        }

        /// <summary>Gets the ANSI sequence to set all of the attribute, forground, and background colors.</summary>
        /// <param name="attribute">Which attribute to set.</param>
        /// <param name="foregroundColor">Which foreground color to set.</param>
        /// <param name="backgroundColor">Which background color to set.</param>
        /// <returns>The ANSI sequence to set all of the attribute, forground, and background colors.</returns>
        public static string SetTextAttributes(AnsiAttribute attribute, AnsiForegroundColor foregroundColor, AnsiBackgroundColor backgroundColor)
        {
            return Esc + string.Format(
                "[{0};{1};{2}m",
                (int)attribute,
                (int)foregroundColor,
                (int)backgroundColor);
        }

        /// <summary>Gets the ANSI sequence to set the text attribute.</summary>
        /// <param name="attribute">Which attribute to set.</param>
        /// <returns>The ANSI sequence to set the text attribute.</returns>
        public static string SetTextAttributes(AnsiAttribute attribute)
        {
            return Esc + string.Format("[{0}m", (int)attribute);
        }

        /// <summary>Gets the ANSI sequence to set the text foreground and background colors.</summary>
        /// <param name="foregroundColor">Which foreground color to set.</param>
        /// <param name="backgroundColor">Which background color to set.</param>
        /// <returns>The ANSI sequence to set the text foreground and background colors.</returns>
        public static string SetTextAttributes(AnsiForegroundColor foregroundColor, AnsiBackgroundColor backgroundColor)
        {
            return Esc + string.Format("[{0};{1}m", (int)foregroundColor, (int)backgroundColor);
        }

        /// <summary>Gets the ANSI sequence to set the cursor to the specified coordinates.</summary>
        /// <param name="line">Which line to set the cursor on.</param>
        /// <param name="column">Which column to set the cursor on.</param>
        /// <returns>The ANSI sequence to set the cursor to the specified coordinates.</returns>
        public static string MoveCursorTo(int line, int column)
        {
            return Esc + string.Format("[{0};{1}H", line, column);
        }

        /// <summary>Gets the ANSI sequence for 'MXP open line'.</summary>
        /// <returns>The ANSI sequence for 'MXP open line'.</returns>
        public static string SetMXPOpenLine()
        {
            return Esc + "[0z";
        }

        /// <summary>Gets the ANSI sequence for 'MXP secure line'.</summary>
        /// <returns>The ANSI sequence for 'MXP secure line'.</returns>
        public static string SetMXPSecureLine()
        {
            return Esc + "[1z";
        }

        /// <summary>Parses a string for special tags and replaces them with ANSI codes.</summary>
        /// <param name="parseString">The string to scan for special tags.</param>
        /// <returns>A string with all special tags replaced with matching ANSI codes.</returns>
        public static string Parse(string parseString)
        {
            //@@@ TODO: Replace with regex and separate ANSI from MXP.
            string newString = parseString;

            if (newString != null)
            {
                // Seek out all tags and replace them with the equivalent ANSI codes.
                // The most common attributes (being 'normal', 'new line' and foreground
                // color codes) will always be scanned for, but after that, we will be a 
                // little more careful about how we spend our time.
                newString = newString.Replace("<%n%>", SetTextAttributes(AnsiAttribute.Normal));
                newString = newString.Replace("<%nl%>", MoveCursorToNewLine());

                // Forground colors
                newString = newString.Replace("<%black%>", SetForegroundColor(AnsiForegroundColor.Black));
                newString = newString.Replace("<%red%>", SetForegroundColor(AnsiForegroundColor.Red));
                newString = newString.Replace("<%green%>", SetForegroundColor(AnsiForegroundColor.Green));
                newString = newString.Replace("<%yellow%>", SetForegroundColor(AnsiForegroundColor.Yellow));
                newString = newString.Replace("<%blue%>", SetForegroundColor(AnsiForegroundColor.Blue));
                newString = newString.Replace("<%magenta%>", SetForegroundColor(AnsiForegroundColor.Magenta));
                newString = newString.Replace("<%cyan%>", SetForegroundColor(AnsiForegroundColor.Cyan));
                newString = newString.Replace("<%white%>", SetForegroundColor(AnsiForegroundColor.White));

                // Background Colors: only scan for these if we have any "<%b" left, which 
                // most likely indicates a background color setting.
                if (newString.Contains("<%b"))
                {
                    newString = newString.Replace("<%bblack%>", SetBackgroundColor(AnsiBackgroundColor.Black));
                    newString = newString.Replace("<%bred%>", SetBackgroundColor(AnsiBackgroundColor.Red));
                    newString = newString.Replace("<%bgreen%>", SetBackgroundColor(AnsiBackgroundColor.Green));
                    newString = newString.Replace("<%byellow%>", SetBackgroundColor(AnsiBackgroundColor.Yellow));
                    newString = newString.Replace("<%bblue%>", SetBackgroundColor(AnsiBackgroundColor.Blue));
                    newString = newString.Replace("<%bmagenta%>", SetBackgroundColor(AnsiBackgroundColor.Magenta));
                    newString = newString.Replace("<%bcyan%>", SetBackgroundColor(AnsiBackgroundColor.Cyan));
                    newString = newString.Replace("<%bwhite%>", SetBackgroundColor(AnsiBackgroundColor.White));
                }

                // Tags for clearing and setting attributes.
                if (newString.Contains("<%"))
                {
                    newString = newString.Replace("<%reset%>", SetTextAttributes(AnsiAttribute.Normal));
                    newString = newString.Replace("<%b%>", SetTextAttributes(AnsiAttribute.Bold));
                    newString = newString.Replace("<%cls%>", ClearScreenAndHomeCursor);
                    newString = newString.Replace("<%u%>", SetTextAttributes(AnsiAttribute.Underline));
                    newString = newString.Replace("<%underline%>", SetTextAttributes(AnsiAttribute.Underline));
                }

                // Finally, tackle the more awkward and less common tags, if any tags remain.
                if (newString.Contains("<%"))
                {
                    // Cursor Movement
                    newString = newString.Replace("<%up%>", MoveCursorUp(1));
                    newString = newString.Replace("<%down%>", MoveCursorDown(1));
                    newString = newString.Replace("<%left%>", MoveCursorLeft(1));
                    newString = newString.Replace("<%right%>", MoveCursorRight(1));

                    // MXP Bits
                    // Regex to match <%mxp <!I can be an mxp tag>%> is <\%mxp (.|\n)+?%>
                    newString = newString.Replace("<%mxpopenline%>", SetMXPOpenLine());
                    newString = newString.Replace("<%mxpsecureline%>", SetMXPSecureLine());
                }

                return newString;
            }

            return string.Empty;
        }
    }
}