//-----------------------------------------------------------------------------
// <copyright file="PwdCommandHandler.cs" company="WheelMUD Development Team">
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
    /// <summary>Present working directory command handler</summary>
    public class PwdCommandHandler : PwdCommandHandlerBase
    {
        public PwdCommandHandler(FtpConnectionObject connectionObject)
            : base("PWD", connectionObject)
        {
        }
    }
}