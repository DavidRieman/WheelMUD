//-----------------------------------------------------------------------------
// <copyright file="DeleCommandHandler.cs" company="WheelMUD Development Team">
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
    /// <summary>Delete command handler.</summary>
    public class DeleCommandHandler : FtpCommandHandler
    {
        public DeleCommandHandler(FtpConnectionObject connectionObject)
            : base("DELE", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            var filePath = this.GetPath(message);
            if (!this.ConnectionObject.FileSystemObject.FileExists(filePath))
            {
                return this.GetMessage(550, "File does not exist.");
            }

            if (!this.ConnectionObject.FileSystemObject.Delete(filePath))
            {
                return this.GetMessage(550, "Couldn't delete file.");
            }

            return this.GetMessage(250, "File deleted successfully");
        }
    }
}