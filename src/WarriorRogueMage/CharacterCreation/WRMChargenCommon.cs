//-----------------------------------------------------------------------------
// <copyright file="WrmChargenCommon.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Core;
using WheelMUD.Server;
using WheelMUD.Utilities.Interfaces;

namespace WarriorRogueMage.CharacterCreation
{
    /// <summary>Common methods for Warrior, Rogue, and Mage character creation.</summary>
    public class WrmChargenCommon
    {
        /// <summary>Sends the error message to the player.</summary>
        /// <param name="session">The session.</param>
        /// <param name="message">The message.</param>
        public static void SendErrorMessage(Session session, string message)
        {
            var wrappedMessage = new OutputBuilder();

            wrappedMessage.AppendSeparator('=', "red", true, message.Length);
            wrappedMessage.AppendLine(message);
            wrappedMessage.AppendSeparator('=', "red", true, message.Length);

            session.Write(wrappedMessage);
        }

        /// <summary>Formats the string to fit the column width. It will be padded with spaces on the right side.</summary>
        /// <param name="columnWidth">Width of the column.</param>
        /// <param name="stringToFormat">The string to format.</param>
        /// <returns>Formatted row to match the column width.</returns>
        public static string FormatToColumn(int columnWidth, string stringToFormat)
        {
            return stringToFormat.Length < columnWidth ? stringToFormat.PadRight(columnWidth, ' ') : stringToFormat;
        }

        public static T GetFirstPriorityMatch<T>(string userQuery, IEnumerable<T> collection) where T : INamed
        {
            return (from r in collection
                    where r.Name.StartsWith(userQuery, StringComparison.OrdinalIgnoreCase)
                    select r).FirstOrDefault() ??
                   (from r in collection
                    where r.Name.Contains(userQuery, StringComparison.OrdinalIgnoreCase)
                    select r).FirstOrDefault();
        }
    }
}