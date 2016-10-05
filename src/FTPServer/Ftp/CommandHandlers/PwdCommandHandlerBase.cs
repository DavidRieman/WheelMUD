//-----------------------------------------------------------------------------
// <copyright file="PwdCommandHandlerBase.cs" company="WheelMUD Development Team">
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
    /// <summary>Base class for present current directory commands</summary>
    public class PwdCommandHandlerBase : FtpCommandHandler
    {
        public PwdCommandHandlerBase(string command, FtpConnectionObject connectionObject)
            : base(command, connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string dir = this.ConnectionObject.CurrentDirectory;
            dir = dir.Replace('\\', '/');
            return this.GetMessage(257, string.Format("\"{0}\" PWD Successful.", dir));
        }
    }
}