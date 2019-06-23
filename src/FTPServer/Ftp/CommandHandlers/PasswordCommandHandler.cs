//-----------------------------------------------------------------------------
// <copyright file="PasswordCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    using WheelMUD.Data.Entities;
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

            var user = PlayerRepositoryExtensions.Authenticate(this.ConnectionObject.User, message);
            if (user != null)
            {
                this.ConnectionObject.CreateFileSystem();
                retval = this.GetMessage(220, "Password ok, FTP server ready");
            }
            else
            {
                retval = this.GetMessage(530, "Username or password incorrect");
            }

            ////if (this.ConnectionObject.Login(sMessage))
            ////{
            ////    return this.GetMessage(220, "Password ok, FTP server ready");
            ////}
            ////
            ////return this.GetMessage(530, "Username or password incorrect");

            return retval;
        }
    }
}