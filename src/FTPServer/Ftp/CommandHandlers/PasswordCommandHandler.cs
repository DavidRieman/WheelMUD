//-----------------------------------------------------------------------------
// <copyright file="PasswordCommandHandler.cs" company="WheelMUD Development Team">
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

            bool auth = PlayerRepository.Authenticate(this.ConnectionObject.User, message);

            if (auth)
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