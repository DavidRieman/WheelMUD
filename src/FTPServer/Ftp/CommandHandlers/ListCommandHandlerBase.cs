//-----------------------------------------------------------------------------
// <copyright file="ListCommandHandlerBase.cs" company="WheelMUD Development Team">
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
    using System;
    using System.IO;
    using System.Text;
    using WheelMUD.Ftp.General;

    /// <summary>Base class for list commands</summary>
    public abstract class ListCommandHandlerBase : FtpCommandHandler
    {
        public ListCommandHandlerBase(string command, FtpConnectionObject connectionObject)
            : base(command, connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            SocketHelpers.Send(this.ConnectionObject.Socket, "150 Opening data connection for LIST\r\n");

            string[] asFiles = null;
            string[] asDirectories = null;

            message = message.Trim();

            string path = this.GetPath(string.Empty);

            if (message.Length == 0 || message[0] == '-')
            {
                asFiles = this.ConnectionObject.FileSystemObject.GetFiles(path);
                asDirectories = this.ConnectionObject.FileSystemObject.GetDirectories(path);
            }
            else
            {
                asFiles = this.ConnectionObject.FileSystemObject.GetFiles(path, message);
                asDirectories = this.ConnectionObject.FileSystemObject.GetDirectories(path, message);
            }

            var asAll = ArrayHelpers.Add(asDirectories, asFiles) as string[];
            string fileList = this.BuildReply(message, asAll);

            var socketReply = new FtpReplySocket(this.ConnectionObject);

            if (!socketReply.Loaded)
            {
                return this.GetMessage(550, "LIST unable to establish return connection.");
            }

            socketReply.Send(fileList);
            socketReply.Close();

            return this.GetMessage(226, "LIST successful.");
        }

        protected abstract string BuildReply(string message, string[] asFiles);

        protected string BuildShortReply(string[] asFiles)
        {
            string fileList = string.Join("\r\n", asFiles);
            fileList += "\r\n";
            return fileList;
        }

        protected string BuildLongReply(string[] asFiles)
        {
            string dir = this.GetPath(string.Empty);

            var stringBuilder = new StringBuilder();

            for (int index = 0; index < asFiles.Length; index++)
            {
                string file = asFiles[index];
                file = Path.Combine(dir, file);

                var info = this.ConnectionObject.FileSystemObject.GetFileInfo(file);
                if (info != null)
                {
                    string attributes = info.GetAttributeString();
                    stringBuilder.Append(attributes);
                    stringBuilder.Append(" 1 owner group");

                    if (info.IsDirectory())
                    {
                        stringBuilder.Append("            1 ");
                    }
                    else
                    {
                        string fileSize = info.GetSize().ToString();
                        stringBuilder.Append(General.TextHelpers.RightAlignString(fileSize, 13, ' '));
                        stringBuilder.Append(" ");
                    }

                    DateTime fileDate = info.GetModifiedTime();

                    string day = fileDate.Day.ToString();

                    stringBuilder.Append(TextHelpers.Month(fileDate.Month));
                    stringBuilder.Append(" ");

                    if (day.Length == 1)
                    {
                        stringBuilder.Append(" ");
                    }

                    stringBuilder.Append(day);
                    stringBuilder.Append(" ");
                    stringBuilder.Append(string.Format("{0:hh}", fileDate));
                    stringBuilder.Append(":");
                    stringBuilder.Append(string.Format("{0:mm}", fileDate));
                    stringBuilder.Append(" ");

                    stringBuilder.Append(asFiles[index]);
                    stringBuilder.Append("\r\n");
                }
            }

            return stringBuilder.ToString();
        }
    }
}