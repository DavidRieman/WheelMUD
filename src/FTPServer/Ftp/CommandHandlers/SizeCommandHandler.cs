//-----------------------------------------------------------------------------
// <copyright file="SizeCommandHandler.cs" company="WheelMUD Development Team">
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
    public class SizeCommandHandler : FtpCommandHandler
    {
        public SizeCommandHandler(FtpConnectionObject connectionObject)
            : base("SIZE", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string path = this.GetPath(message);
            if (!this.ConnectionObject.FileSystemObject.FileExists(path))
            {
                return this.GetMessage(550, string.Format("File doesn't exist ({0})", path));
            }

            var info = this.ConnectionObject.FileSystemObject.GetFileInfo(path);
            if (info == null)
            {
                return this.GetMessage(550, "Error in getting file information");
            }

            return this.GetMessage(220, info.GetSize().ToString());
        }
    }
}