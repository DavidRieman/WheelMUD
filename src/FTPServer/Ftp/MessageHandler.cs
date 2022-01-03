//-----------------------------------------------------------------------------
// <copyright file="MessageHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp
{
    /// <summary>Gives a mechanism for classes to pass messages to the main form for display in the messages list box.</summary>
    public class FtpServerMessageHandler
    {
        protected FtpServerMessageHandler()
        {
        }

        public delegate void MessageEventHandler(int id, string message);

        public static event MessageEventHandler Message;

        public static void SendMessage(int id, string message)
        {
            if (Message != null)
            {
                Message(id, message);
            }
        }
    }
}