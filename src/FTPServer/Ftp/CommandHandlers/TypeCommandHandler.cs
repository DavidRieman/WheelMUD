//-----------------------------------------------------------------------------
// <copyright file="TypeCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
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
                ConnectionObject.BinaryMode = false;
                return GetMessage(200, "ASCII transfer mode active.");
            }

            if (message == "I")
            {
                ConnectionObject.BinaryMode = true;
                return GetMessage(200, "Binary transfer mode active.");
            }
            
            return GetMessage(550, $"Error - unknown binary mode \"{message}\"");
        }
    }
}