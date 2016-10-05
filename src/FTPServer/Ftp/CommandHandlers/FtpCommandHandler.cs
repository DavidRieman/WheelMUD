//-----------------------------------------------------------------------------
// <copyright file="FtpCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: 2004 By David McClarnon (dmcclarnon@ntlworld.com)
//   Modified: June 16, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    using System.Diagnostics;
    using System.IO;
    using WheelMUD.Ftp.General;

    /// <summary>Base class for all ftp command handlers.</summary>
    public class FtpCommandHandler
    {
        public FtpCommandHandler(string command, FtpConnectionObject connectionObject)
        {
            this.Command = command;
            this.ConnectionObject = connectionObject;
        }

        public string Command { get; private set; }

        public FtpConnectionObject ConnectionObject { get; private set; }

        public void Process(string message)
        {
            this.SendMessage(OnProcess(message));
        }

        protected virtual string OnProcess(string message)
        {
            Debug.Assert(false, "FtpCommandHandler::OnProcess base called");
            return string.Empty;
        }

        protected string GetMessage(int returnCode, string message)
        {
            return string.Format("{0} {1}\r\n", returnCode, message);
        }

        protected string GetPath(string path)
        {
            if (path.Length == 0)
            {
                return this.ConnectionObject.CurrentDirectory;
            }

            path = path.Replace('/', '\\');
            return Path.Combine(this.ConnectionObject.CurrentDirectory, path);
        }

        private void SendMessage(string message)
        {
            if (message.Length == 0)
            {
                return;
            }

            int endIndex = message.IndexOf('\r');
            if (endIndex < 0)
            {
                FtpServerMessageHandler.SendMessage(this.ConnectionObject.Id, message);
            }
            else
            {
                FtpServerMessageHandler.SendMessage(this.ConnectionObject.Id, message.Substring(0, endIndex));
            }

            SocketHelpers.Send(this.ConnectionObject.Socket, message);
        }
    }
}