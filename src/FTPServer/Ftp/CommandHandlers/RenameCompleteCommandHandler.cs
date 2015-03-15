//-----------------------------------------------------------------------------
// <copyright file="RenameCompleteCommandHandler.cs" company="WheelMUD Development Team">
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
	public class RenameCompleteCommandHandler : FtpCommandHandler
	{
		public RenameCompleteCommandHandler(FtpConnectionObject connectionObject)
			: base("RNTO", connectionObject)
		{}

		protected override string OnProcess(string sMessage)
		{
            if (this.ConnectionObject.FileToRename.Length == 0)
			{
                return this.GetMessage(503, "RNTO must be preceded by a RNFR.");
			}

            string sNewFileName = this.GetPath(sMessage);
            string sOldFileName = this.ConnectionObject.FileToRename;

            this.ConnectionObject.FileToRename = "";

            if (this.ConnectionObject.FileSystemObject.FileExists(sNewFileName) ||
                this.ConnectionObject.FileSystemObject.DirectoryExists(sNewFileName))
			{
                return this.GetMessage(553, string.Format("File already exists ({0}).", sNewFileName));
			}

            if (!this.ConnectionObject.FileSystemObject.Move(sOldFileName, sNewFileName))
			{
                return this.GetMessage(553, "Move failed");
			}

            return this.GetMessage(250, "Renamed file successfully.");
		}
	}
}
