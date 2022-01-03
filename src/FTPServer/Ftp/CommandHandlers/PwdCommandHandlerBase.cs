//-----------------------------------------------------------------------------
// <copyright file="PwdCommandHandlerBase.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
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
            string dir = ConnectionObject.CurrentDirectory;
            dir = dir.Replace('\\', '/');
            return GetMessage(257, string.Format("\"{0}\" PWD Successful.", dir));
        }
    }
}