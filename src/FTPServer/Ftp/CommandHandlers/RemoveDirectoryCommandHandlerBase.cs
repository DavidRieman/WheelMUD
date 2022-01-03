//-----------------------------------------------------------------------------
// <copyright file="RemoveDirectoryCommandHandlerBase.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
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
            string file = GetPath(message);
            if (!ConnectionObject.FileSystemObject.DirectoryExists(file))
            {
                return GetMessage(550, string.Format("Directory does not exist"));
            }

            if (ConnectionObject.FileSystemObject.Delete(file))
            {
                return GetMessage(250, "Directory removed.");
            }

            return GetMessage(550, string.Format("Couldn't remove directory ({0}).", file));
        }
    }
}