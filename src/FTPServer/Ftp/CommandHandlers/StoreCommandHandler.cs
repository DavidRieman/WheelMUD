//-----------------------------------------------------------------------------
// <copyright file="StoreCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    using WheelMUD.Ftp.General;

    public class StoreCommandHandler : FtpCommandHandler
    {
        private const int BufferSize = 65536;

        public StoreCommandHandler(FtpConnectionObject connectionObject)
            : base("STOR", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            var filePath = GetPath(message);
            if (ConnectionObject.FileSystemObject.FileExists(filePath))
            {
                return GetMessage(553, "File already exists.");
            }

            var file = ConnectionObject.FileSystemObject.OpenFile(filePath, true);
            var socketReply = new FtpReplySocket(ConnectionObject);

            if (!socketReply.Loaded)
            {
                return GetMessage(425, "Error in establishing data connection.");
            }

            SocketHelpers.Send(ConnectionObject.Socket, GetMessage(150, "Opening connection for data transfer."));

            byte[] data = new byte[BufferSize];
            int received = socketReply.Receive(data);
            while (received > 0)
            {
                file.Write(data, received);
                received = socketReply.Receive(data);
            }

            file.Close();
            socketReply.Close();

            return GetMessage(226, "Uploaded file successfully.");
        }
    }
}