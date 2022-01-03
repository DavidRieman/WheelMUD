//-----------------------------------------------------------------------------
// <copyright file="PasswordCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    using WheelMUD.Data.Repositories;

    public class PasswordCommandHandler : FtpCommandHandler
    {
        public PasswordCommandHandler(FtpConnectionObject connectionObject)
            : base("PASS", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string retval;

            var user = PlayerRepositoryExtensions.Authenticate(ConnectionObject.User, message);
            if (user != null)
            {
                ConnectionObject.CreateFileSystem();
                retval = GetMessage(220, "Password ok, FTP server ready");
            }
            else
            {
                retval = GetMessage(530, "Username or password incorrect");
            }

            ////if (ConnectionObject.Login(sMessage))
            ////{
            ////    return GetMessage(220, "Password ok, FTP server ready");
            ////}
            ////
            ////return GetMessage(530, "Username or password incorrect");

            return retval;
        }
    }
}