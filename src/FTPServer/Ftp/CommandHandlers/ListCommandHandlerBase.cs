//-----------------------------------------------------------------------------
// <copyright file="ListCommandHandlerBase.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.IO;
using System.Text;
using WheelMUD.Ftp.General;
using WheelMUD.Utilities;

namespace WheelMUD.Ftp.FtpCommands
{
    /// <summary>Base class for list commands</summary>
    public abstract class ListCommandHandlerBase : FtpCommandHandler
    {
        public ListCommandHandlerBase(string command, FtpConnectionObject connectionObject)
            : base(command, connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            SocketHelpers.Send(ConnectionObject.Socket, "150 Opening data connection for LIST\r\n");

            string[] asFiles = null;
            string[] asDirectories = null;

            message = message.Trim();

            var path = GetPath(string.Empty);

            if (message.Length == 0 || message[0] == '-')
            {
                asFiles = ConnectionObject.FileSystemObject.GetFiles(path);
                asDirectories = ConnectionObject.FileSystemObject.GetDirectories(path);
            }
            else
            {
                asFiles = ConnectionObject.FileSystemObject.GetFiles(path, message);
                asDirectories = ConnectionObject.FileSystemObject.GetDirectories(path, message);
            }

            var asAll = ArrayHelpers.Add(asDirectories, asFiles) as string[];
            var fileList = BuildReply(message, asAll);

            var socketReply = new FtpReplySocket(ConnectionObject);

            if (!socketReply.Loaded)
            {
                return GetMessage(550, "LIST unable to establish return connection.");
            }

            socketReply.Send(fileList);
            socketReply.Close();

            return GetMessage(226, "LIST successful.");
        }

        protected abstract string BuildReply(string message, string[] asFiles);

        protected string BuildShortReply(string[] asFiles)
        {
            var fileList = string.Join(AnsiSequences.NewLine, asFiles);
            fileList += AnsiSequences.NewLine;
            return fileList;
        }

        protected string BuildLongReply(string[] asFiles)
        {
            var dir = GetPath(string.Empty);

            var sb = new StringBuilder();

            foreach (var t in asFiles)
            {
                var file = t;
                file = Path.Combine(dir, file);

                var info = ConnectionObject.FileSystemObject.GetFileInfo(file);
                if (info == null) continue;

                sb.Append($"{info.GetAttributeString()} 1 owner group ");

                if (info.IsDirectory())
                {
                    sb.Append("            1 ");
                }
                else
                {
                    var fileSize = info.GetSize().ToString();
                    sb.Append($"{TextHelpers.RightAlignString(fileSize, 13, ' ')} ");
                }

                var fileDate = info.GetModifiedTime();

                var day = fileDate.Day.ToString();

                sb.Append($"{TextHelpers.Month(fileDate.Month)} ");

                if (day.Length == 1)
                {
                    sb.Append(" ");
                }

                sb.AppendLine($"{day} {fileDate:hh} : {fileDate:mm} {t}");
            }

            return sb.ToString();
        }
    }
}