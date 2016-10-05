//-----------------------------------------------------------------------------
// <copyright file="AppendCommandHandler.cs" company="WheelMUD Development Team">
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

    public class AppendCommandHandler : FtpCommandHandler
    {
        private const int BufferSize = 65536;

        public AppendCommandHandler(FtpConnectionObject connectionObject)
            : base("APPE", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string filePath = this.GetPath(message);

            var file = this.ConnectionObject.FileSystemObject.OpenFile(filePath, true);
            if (file == null)
            {
                return this.GetMessage(425, "Couldn't open file");
            }

            var socketReply = new FtpReplySocket(this.ConnectionObject);

            if (!socketReply.Loaded)
            {
                return this.GetMessage(425, "Error in establishing data connection.");
            }

            var data = new byte[BufferSize];

            SocketHelpers.Send(this.ConnectionObject.Socket, this.GetMessage(150, "Opening connection for data transfer."));

            int received = socketReply.Receive(data);

            while (received > 0)
            {
                received = socketReply.Receive(data);
                file.Write(data, received);
            }

            file.Close();
            socketReply.Close();

            return this.GetMessage(226, string.Format("Appended file successfully. ({0})", filePath));
        }
    }
}