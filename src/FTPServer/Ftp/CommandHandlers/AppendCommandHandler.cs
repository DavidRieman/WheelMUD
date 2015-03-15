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
    using WheelMUD.Ftp.FileSystem;
    using WheelMUD.Ftp.General;
    
    public class AppendCommandHandler : FtpCommandHandler
	{
		private const int m_nBufferSize = 65536;

		public AppendCommandHandler(FtpConnectionObject connectionObject)
			: base("APPE", connectionObject)
		{}

		protected override string OnProcess(string sMessage)
		{
            string sFile = this.GetPath(sMessage);

			IFile file = this.ConnectionObject.FileSystemObject.OpenFile(sFile, true);

			if (file == null)
			{
				return this.GetMessage(425, "Couldn't open file");
			}

            var socketReply = new FtpReplySocket(this.ConnectionObject);

			if (!socketReply.Loaded)
			{
                return this.GetMessage(425, "Error in establishing data connection.");
			}

			var abData = new byte[m_nBufferSize];

            SocketHelpers.Send(this.ConnectionObject.Socket, this.GetMessage(150, "Opening connection for data transfer."));

			int nReceived = socketReply.Receive(abData);

			while (nReceived > 0)
			{
				nReceived = socketReply.Receive(abData);
				file.Write(abData, nReceived);
			}

			file.Close();
			socketReply.Close();

            return this.GetMessage(226, string.Format("Appended file successfully. ({0})", sFile));
		}
	}
}
