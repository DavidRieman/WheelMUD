//-----------------------------------------------------------------------------
// <copyright file="TypeCommandHandler.cs" company="WheelMUD Development Team">
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
    /// <summary>Implements the 'TYPE' command.</summary>
    public class TypeCommandHandler : FtpCommandHandler
    {
        public TypeCommandHandler(FtpConnectionObject connectionObject)
            : base("TYPE", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            message = message.ToUpper();
            if (message == "A")
            {
                this.ConnectionObject.BinaryMode = false;
                return this.GetMessage(200, "ASCII transfer mode active.");
            }
            else if (message == "I")
            {
                this.ConnectionObject.BinaryMode = true;
                return this.GetMessage(200, "Binary transfer mode active.");
            }
            else
            {
                return this.GetMessage(550, string.Format("Error - unknown binary mode \"{0}\"", message));
            }
        }
    }
}