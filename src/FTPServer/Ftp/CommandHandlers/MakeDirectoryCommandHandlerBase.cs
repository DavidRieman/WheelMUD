//-----------------------------------------------------------------------------
// <copyright file="MakeDirectoryCommandHandlerBase.cs" company="WheelMUD Development Team">
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
    public class MakeDirectoryCommandHandlerBase : FtpCommandHandler
    {
        protected MakeDirectoryCommandHandlerBase(string command, FtpConnectionObject connectionObject)
            : base(command, connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string filePath = this.GetPath(message);
            if (!this.ConnectionObject.FileSystemObject.CreateDirectory(filePath))
            {
                return this.GetMessage(550, string.Format("Couldn't create directory. ({0})", filePath));
            }

            return this.GetMessage(257, filePath);
        }
    }
}