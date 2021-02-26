//-----------------------------------------------------------------------------
// <copyright file="FtpCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;

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
            Command = command;
            ConnectionObject = connectionObject;
        }

        public string Command { get; private set; }

        public FtpConnectionObject ConnectionObject { get; private set; }

        public void Process(string message)
        {
            SendMessage(OnProcess(message));
        }

        protected virtual string OnProcess(string message)
        {
            Debug.Assert(false, "FtpCommandHandler::OnProcess base called");
            return string.Empty;
        }

        protected string GetMessage(int returnCode, string message)
        {
            return $"{returnCode} {message}{AnsiSequences.NewLine}";
        }

        protected string GetPath(string path)
        {
            if (path.Length == 0)
            {
                return ConnectionObject.CurrentDirectory;
            }

            path = path.Replace('/', '\\');
            return Path.Combine(ConnectionObject.CurrentDirectory, path);
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
                FtpServerMessageHandler.SendMessage(ConnectionObject.Id, message);
            }
            else
            {
                FtpServerMessageHandler.SendMessage(ConnectionObject.Id, message.Substring(0, endIndex));
            }

            SocketHelpers.Send(ConnectionObject.Socket, message);
        }
    }
}