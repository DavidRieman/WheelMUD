//-----------------------------------------------------------------------------
// <copyright file="RenameCompleteCommandHandler.cs" company="WheelMUD Development Team">
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
    public class RenameCompleteCommandHandler : FtpCommandHandler
    {
        public RenameCompleteCommandHandler(FtpConnectionObject connectionObject)
            : base("RNTO", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            if (this.ConnectionObject.FileToRename.Length == 0)
            {
                return this.GetMessage(503, "RNTO must be preceded by a RNFR.");
            }

            string newFileName = this.GetPath(message);
            string oldFileName = this.ConnectionObject.FileToRename;

            this.ConnectionObject.FileToRename = string.Empty;

            if (this.ConnectionObject.FileSystemObject.FileExists(newFileName) ||
                this.ConnectionObject.FileSystemObject.DirectoryExists(newFileName))
            {
                return this.GetMessage(553, string.Format("File already exists ({0}).", newFileName));
            }

            if (!this.ConnectionObject.FileSystemObject.Move(oldFileName, newFileName))
            {
                return this.GetMessage(553, "Move failed");
            }

            return this.GetMessage(250, "Renamed file successfully.");
        }
    }
}