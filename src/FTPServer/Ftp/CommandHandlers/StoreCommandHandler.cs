//-----------------------------------------------------------------------------
// <copyright file="StoreCommandHandler.cs" company="WheelMUD Development Team">
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
    using WheelMUD.Ftp.FileSystem;
    using WheelMUD.Ftp.General;
	
    public class StoreCommandHandler : FtpCommandHandler
	{
		private const int BufferSize = 65536;

		public StoreCommandHandler(FtpConnectionObject connectionObject)
			: base("STOR", connectionObject)
		{}

		protected override string OnProcess(string sMessage)
		{
            string sFile = this.GetPath(sMessage);

            if (this.ConnectionObject.FileSystemObject.FileExists(sFile))
			{
                return this.GetMessage(553, "File already exists.");
			}

            IFile file = this.ConnectionObject.FileSystemObject.OpenFile(sFile, true);

            var socketReply = new FtpReplySocket(this.ConnectionObject);

			if (!socketReply.Loaded)
			{
                return this.GetMessage(425, "Error in establishing data connection.");
			}

			byte [] abData = new byte[BufferSize];

            SocketHelpers.Send(this.ConnectionObject.Socket, this.GetMessage(150, "Opening connection for data transfer."));

			int nReceived = socketReply.Receive(abData);

			while (nReceived > 0)
			{
				file.Write(abData, nReceived);
				nReceived = socketReply.Receive(abData);
			}

			file.Close();
			socketReply.Close();

            return this.GetMessage(226, "Uploaded file successfully.");
		}
	}
}
