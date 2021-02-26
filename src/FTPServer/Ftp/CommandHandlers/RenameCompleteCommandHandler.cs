//-----------------------------------------------------------------------------
// <copyright file="RenameCompleteCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    public class RenameCompleteCommandHandler : FtpCommandHandler
    {
        public RenameCompleteCommandHandler(FtpConnectionObject connectionObject)
            : base("RNTO", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            if (ConnectionObject.FileToRename.Length == 0)
            {
                return GetMessage(503, "RNTO must be preceded by a RNFR.");
            }

            string newFileName = GetPath(message);
            string oldFileName = ConnectionObject.FileToRename;

            ConnectionObject.FileToRename = string.Empty;

            if (ConnectionObject.FileSystemObject.FileExists(newFileName) ||
                ConnectionObject.FileSystemObject.DirectoryExists(newFileName))
            {
                return GetMessage(553, $"File already exists ({newFileName}).");
            }

            if (!ConnectionObject.FileSystemObject.Move(oldFileName, newFileName))
            {
                return GetMessage(553, "Move failed");
            }

            return GetMessage(250, "Renamed file successfully.");
        }
    }
}