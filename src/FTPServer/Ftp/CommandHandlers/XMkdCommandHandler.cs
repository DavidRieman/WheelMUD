//-----------------------------------------------------------------------------
// <copyright file="XMkdCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    public class XMkdCommandHandler : MakeDirectoryCommandHandlerBase
    {
        public XMkdCommandHandler(FtpConnectionObject connectionObject)
            : base("XMKD", connectionObject)
        {
        }
    }
}