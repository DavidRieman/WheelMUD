//-----------------------------------------------------------------------------
// <copyright file="AppendCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Ftp.General;

namespace WheelMUD.Ftp.FtpCommands
{
    public class AppendCommandHandler : FtpCommandHandler
    {
        private const int BufferSize = 65536;

        public AppendCommandHandler(FtpConnectionObject connectionObject)
            : base("APPE", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string filePath = GetPath(message);

            var file = ConnectionObject.FileSystemObject.OpenFile(filePath, true);
            if (file == null)
            {
                return GetMessage(425, "Couldn't open file");
            }

            var socketReply = new FtpReplySocket(ConnectionObject);

            if (!socketReply.Loaded)
            {
                return GetMessage(425, "Error in establishing data connection.");
            }

            var data = new byte[BufferSize];

            SocketHelpers.Send(ConnectionObject.Socket, GetMessage(150, "Opening connection for data transfer."));

            int received = socketReply.Receive(data);

            while (received > 0)
            {
                received = socketReply.Receive(data);
                file.Write(data, received);
            }

            file.Close();
            socketReply.Close();

            return GetMessage(226, string.Format("Appended file successfully. ({0})", filePath));
        }
    }
}