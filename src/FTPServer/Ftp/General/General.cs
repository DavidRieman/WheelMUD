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

    /// <summary>
    /// Helper functions relating to files and file names/paths
    /// </summary>
    public class FileNameHelpers
    {
        static public bool IsValid(string sFileName)
        {
            if (sFileName.IndexOf("\\\\") >= 0)
            {
                return false;
            }

            if (sFileName.IndexOf("...") >= 0)
            {
                return false;
            }

            return true;
        }
    }

    public class TextHelpers
    {
        static public string RightAlignString(string sString, int nWidth, char cDelimiter)
        {
            var stringBuilder = new StringBuilder();

            for (int nCharacter = 0; nCharacter < nWidth - sString.Length; nCharacter++)
            {
                stringBuilder.Append(cDelimiter);
            }

            stringBuilder.Append(sString);
            return stringBuilder.ToString();
        }

        static public string Month(int nMonth)
        {
            switch (nMonth)
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
                    Debug.Assert(false);
                    return string.Empty;
            }
        }
    }
}
