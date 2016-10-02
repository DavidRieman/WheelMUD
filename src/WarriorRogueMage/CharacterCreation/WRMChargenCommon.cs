//-----------------------------------------------------------------------------
// <copyright file="WrmChargenCommon.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Common methods for Warrior, Rogue, and Mage character creation
//   Author: Fastalanasa
//   Date: May 14, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.CharacterCreation
{
    using System;
    using System.Text;
    using WheelMUD.Core;

    /// <summary>Common methods for Warrior, Rogue, and Mage character creation.</summary>
    public class WrmChargenCommon
    {
        /// <summary>Sends the error message to the player.</summary>
        /// <param name="session">The session.</param>
        /// <param name="message">The message.</param>
        public static void SendErrorMessage(Session session, string message)
        {
            var divider = new StringBuilder();
            var wrappedMessage = new StringBuilder();

            divider.Append('=', message.Length);

            wrappedMessage.Append("<%red%>" + divider + Environment.NewLine);
            wrappedMessage.Append(message + Environment.NewLine);
            wrappedMessage.Append(divider + "<%n%>");

            session.Write(wrappedMessage.ToString());
        }

        /// <summary>Formats the string to fit the column width. It will be padded with spaces on the right side.</summary>
        /// <param name="columnWidth">Width of the column.</param>
        /// <param name="stringToFormat">The string to format.</param>
        /// <returns>Formatted row to match the column width.</returns>
        public static string FormatToColumn(int columnWidth, string stringToFormat)
        {
            string retval = string.Empty;

            if (stringToFormat.Length < columnWidth)
            {
                retval = stringToFormat.PadRight(columnWidth, ' ');
            }
            else
            {
                retval = stringToFormat;
            }

            return retval;
        }
    }
}