//-----------------------------------------------------------------------------
// <copyright file="RetrCommandHandler.cs" company="WheelMUD Development Team">
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
    using WheelMUD.Ftp.General;

    /// <summary>Implements the RETR command.</summary>
    public class RetrCommandHandler : FtpCommandHandler
    {
        public RetrCommandHandler(FtpConnectionObject connectionObject)
            : base("RETR", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string filePath = this.GetPath(message);
            if (!this.ConnectionObject.FileSystemObject.FileExists(filePath))
            {
                return this.GetMessage(550, "File doesn't exist");
            }

            var replySocket = new FtpReplySocket(this.ConnectionObject);
            if (!replySocket.Loaded)
            {
                return this.GetMessage(550, "Unable to establish data connection");
            }

            SocketHelpers.Send(this.ConnectionObject.Socket, "150 Starting data transfer, please wait...\r\n");

            var file = this.ConnectionObject.FileSystemObject.OpenFile(filePath, false);
            if (file == null)
            {
                return this.GetMessage(550, "Couldn't open file");
            }

            const int BufferSize = 65536;
            var buffer = new byte[BufferSize];
            int read = file.Read(buffer, BufferSize);
            while (read > 0 && replySocket.Send(buffer, read))
            {
                read = file.Read(buffer, BufferSize);
            }

            file.Close();
            replySocket.Close();

            return this.GetMessage(226, "File download succeeded.");
        }
    }
}