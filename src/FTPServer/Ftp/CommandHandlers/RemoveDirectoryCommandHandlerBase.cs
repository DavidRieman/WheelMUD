//-----------------------------------------------------------------------------
// <copyright file="RemoveDirectoryCommandHandlerBase.cs" company="WheelMUD Development Team">
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
    public class RemoveDirectoryCommandHandlerBase : FtpCommandHandler
    {
        protected RemoveDirectoryCommandHandlerBase(string command, FtpConnectionObject connectionObject)
            : base(command, connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string file = this.GetPath(message);
            if (!this.ConnectionObject.FileSystemObject.DirectoryExists(file))
            {
                return this.GetMessage(550, string.Format("Directory does not exist"));
            }

            if (this.ConnectionObject.FileSystemObject.Delete(file))
            {
                return this.GetMessage(250, "Directory removed.");
            }

            return this.GetMessage(550, string.Format("Couldn't remove directory ({0}).", file));
        }
    }
}