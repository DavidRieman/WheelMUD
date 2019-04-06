//-----------------------------------------------------------------------------
// <copyright file="RemoveDirectoryCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    public class RemoveDirectoryCommandHandler : RemoveDirectoryCommandHandlerBase
    {
        public RemoveDirectoryCommandHandler(FtpConnectionObject connectionObject)
            : base("RMD", connectionObject)
        {
        }
    }
}