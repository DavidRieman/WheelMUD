/* ANSI/VT100 Handlers
 * 
 * Use Telnet Negoations to Determin TermType and 
 * Use The Proper Class to handle that Terms TermType
 */


using System;

namespace WheelMUD.Server.TermHandler
{
    public enum COLORS
    {
        // DEFAULT FG COLORS
        BLACK = 30,
        RED = 31,
        GREEN = 32,
        YELLOW = 33,
        BLUE = 34,
        MAGENTA = 35,
        CYAN = 36,
        WHITE = 37,

        // Background Colors              
        BGBLACK = 40,
        BGRED = 41,
        BGGREEN = 42,
        BGYELLOW = 43,
        BGBLUE = 44,
        BGMAGENTA = 45,
        BGCYAN = 46,
        BGWHITE = 47
    }

    public enum ATTRS
    {
        NORMAL = 0,
        BOLD = 1,
        UNDERLINE = 4
    }

    /// <summary>
    /// Class For parsing mud ansi Tags into ansi
    /// </summary>
    public static class AnsiHandler
    {
        private static string _esc = "\x1B";

        public static string ESC
        {
            get { return _esc; }
            set { _esc = value; }
        }


        public static string MoveCursorUp(int numLines)
        {
            return ESC + String.Format("[{0}A", numLines);
        }

        public static string MoveCursorDown(int numLines)
        {
            return ESC + String.Format("[{0}B", numLines);
        }

        public static string MoveCursorRight(int numCols)
        {
            return ESC + String.Format("[{0}C", numCols);
        }

        public static string MoveCursorLeft(int numCols)
        {
            return ESC + String.Format("[{0}D", numCols);
        }

        public static string SaveCursorPosition
        {
            get { return ESC + "[s"; }
        }

        public static string LoadCursorPosition
        {
            get { return ESC + "[u"; }
        }

        public static string ClearScreenAndHomeCursor
        {
            get { return ESC + "[2J"; }
        }

        public static string ClearToEOL
        {
            get { return ESC + "K"; }
        }

        public static string SetForegroundColor(COLORS foregroundColor)
        {
            return ESC + String.Format("[{0}m",
                                       (int) foregroundColor);
        }

        public static string SetBackgroundColor(COLORS backgroundColor)
        {
            return ESC + String.Format("[{0}m",
                                       (int) backgroundColor);
        }

        public static string SetTextAttributes(ATTRS attribute, COLORS foregroundColor, COLORS backgroundColor)
        {
            return ESC + String.Format("[{0};{1};{2}m",
                                       (int) attribute,
                                       (int) foregroundColor,
                                       (int) backgroundColor);
        }

        public static string SetTextAttributes(ATTRS attribute)
        {
            return ESC + String.Format("[{0}m",
                                       (int) attribute);
        }

        public static string SetTextAttributes(COLORS foregroundColor, COLORS backgroundColor)
        {
            return ESC + String.Format("[{0};{1}m",
                                       (int) foregroundColor,
                                       (int) backgroundColor);
        }

        public static string MoveCursorTo(int line, int col)
        {
            return ESC + String.Format("[{0};{1}H", line, col);
        }

        public static string SetMXPOpenLine()
        {
            return ESC + "[0z";
        }

        public static string SetMXPSecureLine()
        {
            return ESC + "[1z";
        }
        
        public static string Parse(string pString)
        {
            string cString = pString;

            // Seek and Destroy Tags

            // Basic Clear and Set Attributes 
            cString = cString.Replace("<%n%>", SetTextAttributes(ATTRS.NORMAL));
            cString = cString.Replace("<%reset%>", SetTextAttributes(ATTRS.NORMAL));
            cString = cString.Replace("<%b%>", SetTextAttributes(ATTRS.BOLD));
            cString = cString.Replace("<%cls%>", ClearScreenAndHomeCursor);
            cString = cString.Replace("<%u%>", SetTextAttributes(ATTRS.UNDERLINE));
            cString = cString.Replace("<%underline%>", SetTextAttributes(ATTRS.UNDERLINE));

            // Forground colors
            cString = cString.Replace("<%black%>", SetForegroundColor(COLORS.BLACK));
            cString = cString.Replace("<%red%>", SetForegroundColor(COLORS.RED));
            cString = cString.Replace("<%green%>", SetForegroundColor(COLORS.GREEN));
            cString = cString.Replace("<%yellow%>", SetForegroundColor(COLORS.YELLOW));
            cString = cString.Replace("<%blue%>", SetForegroundColor(COLORS.BLUE));
            cString = cString.Replace("<%magenta%>", SetForegroundColor(COLORS.MAGENTA));
            cString = cString.Replace("<%cyan%>", SetForegroundColor(COLORS.CYAN));
            cString = cString.Replace("<%white%>", SetForegroundColor(COLORS.WHITE));

            // Background Colors
            cString = cString.Replace("<%bblack%>", SetBackgroundColor(COLORS.BGBLACK));
            cString = cString.Replace("<%bred%>", SetBackgroundColor(COLORS.BGRED));
            cString = cString.Replace("<%bgreen%>", SetBackgroundColor(COLORS.BGGREEN));
            cString = cString.Replace("<%byellow%>", SetBackgroundColor(COLORS.BGYELLOW));
            cString = cString.Replace("<%bblue%>", SetBackgroundColor(COLORS.BGBLUE));
            cString = cString.Replace("<%bmagenta%>", SetBackgroundColor(COLORS.BGMAGENTA));
            cString = cString.Replace("<%bcyan%>", SetBackgroundColor(COLORS.BGCYAN));
            cString = cString.Replace("<%bwhite%>", SetBackgroundColor(COLORS.BGWHITE));

            // Cursor Movement
            cString = cString.Replace("<%up%>", MoveCursorUp(1));
            cString = cString.Replace("<%down%>", MoveCursorDown(1));

            //MXP Bits
            //Regex to match <%mxp <!I can be an mxp tag>%> is <\%mxp (.|\n)+?%>
            cString = cString.Replace("<%mxpopenline%>", SetMXPOpenLine());
            cString = cString.Replace("<%mxpsecureline%>", SetMXPSecureLine());

            return cString;
        }
    }
}