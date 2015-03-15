//-----------------------------------------------------------------------------
// <copyright file="ListCommandHandlerBase.cs" company="WheelMUD Development Team">
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
    using System;
    using System.IO;
    using System.Text;
    using WheelMUD.Ftp.FileSystem;
    using WheelMUD.Ftp.General;

	/// <summary>
	/// Base class for list commands
	/// </summary>
	public abstract class ListCommandHandlerBase : FtpCommandHandler
	{
		public ListCommandHandlerBase(string sCommand, FtpConnectionObject connectionObject)
			: base(sCommand, connectionObject)
		{}

		protected override string OnProcess(string message)
		{
			SocketHelpers.Send(this.ConnectionObject.Socket, "150 Opening data connection for LIST\r\n");

			string [] asFiles = null;
			string [] asDirectories = null;

			message = message.Trim();

			string path = this.GetPath("");
			
			if (message.Length == 0 || message[0] == '-')
			{
				asFiles = this.ConnectionObject.FileSystemObject.GetFiles(path);
				asDirectories = this.ConnectionObject.FileSystemObject.GetDirectories(path);
			}
			else 
			{
				asFiles = this.ConnectionObject.FileSystemObject.GetFiles(path, message);
				asDirectories = this.ConnectionObject.FileSystemObject.GetDirectories(path, message);
			}

			var asAll = ArrayHelpers.Add(asDirectories, asFiles) as string[];
			string sFileList = this.BuildReply(message, asAll);
			
			var socketReply = new FtpReplySocket(this.ConnectionObject);

			if (!socketReply.Loaded)
			{
				return this.GetMessage(550, "LIST unable to establish return connection.");
			}
			
			socketReply.Send(sFileList);
			socketReply.Close();

			return this.GetMessage(226, "LIST successful.");
		}

		protected abstract string BuildReply(string sMessage, string [] asFiles);

		protected string BuildShortReply(string [] asFiles)
		{
			string sFileList = string.Join("\r\n", asFiles);
			sFileList += "\r\n";
			return sFileList;
		}

		protected string BuildLongReply(string [] asFiles)
		{
            string sDirectory = this.GetPath(string.Empty);

			var stringBuilder = new StringBuilder();

			for (int nIndex = 0 ; nIndex < asFiles.Length; nIndex++)
			{
				string sFile = asFiles[nIndex];
				sFile = Path.Combine(sDirectory, sFile);

				IFileInfo info = this.ConnectionObject.FileSystemObject.GetFileInfo(sFile);

				if (info != null)
				{
					string sAttributes = info.GetAttributeString();
					stringBuilder.Append(sAttributes);
					stringBuilder.Append(" 1 owner group");

					if (info.IsDirectory())
					{
						stringBuilder.Append("            1 ");
					}
					else
					{
						string sFileSize = info.GetSize().ToString();
						stringBuilder.Append(General.TextHelpers.RightAlignString(sFileSize, 13, ' '));
						stringBuilder.Append(" ");
					}

					DateTime fileDate = info.GetModifiedTime();

					string sDay = fileDate.Day.ToString();

					stringBuilder.Append(TextHelpers.Month(fileDate.Month));
					stringBuilder.Append(" ");

					if (sDay.Length == 1)
					{
						stringBuilder.Append(" ");
					}

					stringBuilder.Append(sDay);
					stringBuilder.Append(" ");
					stringBuilder.Append(string.Format("{0:hh}", fileDate));
					stringBuilder.Append(":");
					stringBuilder.Append(string.Format("{0:mm}", fileDate));
					stringBuilder.Append(" ");

					stringBuilder.Append(asFiles[nIndex]);
					stringBuilder.Append("\r\n");
				}
			}

			return stringBuilder.ToString();
		}
	}
}
