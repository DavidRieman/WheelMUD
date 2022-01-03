//-----------------------------------------------------------------------------
// <copyright file="SizeCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    public class SizeCommandHandler : FtpCommandHandler
    {
        public SizeCommandHandler(FtpConnectionObject connectionObject)
            : base("SIZE", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string path = GetPath(message);
            if (!ConnectionObject.FileSystemObject.FileExists(path))
            {
                return GetMessage(550, string.Format("File doesn't exist ({0})", path));
            }

            var info = ConnectionObject.FileSystemObject.GetFileInfo(path);
            if (info == null)
            {
                return GetMessage(550, "Error in getting file information");
            }

            return GetMessage(220, info.GetSize().ToString());
        }
    }
}