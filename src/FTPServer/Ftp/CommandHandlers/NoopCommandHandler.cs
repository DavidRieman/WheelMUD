//-----------------------------------------------------------------------------
// <copyright file="NoopCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    public class NoopCommandHandler : FtpCommandHandler
    {
        public NoopCommandHandler(FtpConnectionObject connectionObject)
            : base("NOOP", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            return GetMessage(200, string.Empty);
        }
    }
}