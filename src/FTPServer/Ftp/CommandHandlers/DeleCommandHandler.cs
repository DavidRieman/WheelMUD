//-----------------------------------------------------------------------------
// <copyright file="DeleCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
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
            var filePath = GetPath(message);
            if (!ConnectionObject.FileSystemObject.FileExists(filePath))
            {
                return GetMessage(550, "File does not exist.");
            }

            if (!ConnectionObject.FileSystemObject.Delete(filePath))
            {
                return GetMessage(550, "Couldn't delete file.");
            }

            return GetMessage(250, "File deleted successfully");
        }
    }
}