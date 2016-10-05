//-----------------------------------------------------------------------------
// <copyright file="General.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: 2004 By David McClarnon (dmcclarnon@ntlworld.com)
//   Modified: June 16, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.General
{
    using System.Diagnostics;
    using System.Text;

    /// <summary>Helper functions relating to files and file names/paths.</summary>
    public class FileNameHelpers
    {
        public static bool IsValid(string fileName)
        {
            if (fileName.IndexOf("\\\\") >= 0)
            {
                return false;
            }

            if (fileName.IndexOf("...") >= 0)
            {
                return false;
            }

            return true;
        }
    }

    public class TextHelpers
    {
        public static string RightAlignString(string s, int width, char delimiter)
        {
            var stringBuilder = new StringBuilder();

            for (int chr = 0; chr < width - s.Length; chr++)
            {
                stringBuilder.Append(delimiter);
            }

            stringBuilder.Append(s);
            return stringBuilder.ToString();
        }

        public static string Month(int monthNumber)
        {
            switch (monthNumber)
            {
                case 1:
                    return "Jan";
                case 2:
                    return "Feb";
                case 3:
                    return "Mar";
                case 4:
                    return "Apr";
                case 5:
                    return "May";
                case 6:
                    return "Jun";
                case 7:
                    return "Jul";
                case 8:
                    return "Aug";
                case 9:
                    return "Sep";
                case 10:
                    return "Oct";
                case 11:
                    return "Nov";
                case 12:
                    return "Dec";
                default:
                    Debug.Assert(false, "Invalid month number");
                    return string.Empty;
            }
        }
    }
}