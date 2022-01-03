//-----------------------------------------------------------------------------
// <copyright file="UserCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    public class UserCommandHandler : FtpCommandHandler
    {
        public UserCommandHandler(FtpConnectionObject connectionObject)
            : base("USER", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            ConnectionObject.User = message;
            return GetMessage(331, string.Format("User {0} logged in, needs password", message));
        }
    }
}