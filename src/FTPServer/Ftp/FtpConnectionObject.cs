//-----------------------------------------------------------------------------
// <copyright file="FtpConnectionObject.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: 2004 By David McClarnon (dmcclarnon@ntlworld.com)
//   Modified: June 16, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp
{
    using System.Collections;
    using System.Net.Sockets;
    using System.Text;
    using WheelMUD.Ftp.FileSystem;
    using WheelMUD.Ftp.FtpCommands;
    using WheelMUD.Ftp.General;

	/// <summary>Processes incoming messages and passes the data on to the relevant handler class.</summary>
	public class FtpConnectionObject : FtpConnectionData
	{
		private readonly Hashtable _commandHashTable;
		private readonly IFileSystemClassFactory _fileSystemClassFactory;
		
		public FtpConnectionObject(IFileSystemClassFactory fileSystemClassFactory, int nId, TcpClient socket)
			: base(nId, socket)
		{
            this._commandHashTable = new Hashtable();
            this._fileSystemClassFactory = fileSystemClassFactory;
			
			this.LoadCommands();
		}

        public bool Login(string sPassword)
		{
			IFileSystem fileSystem = _fileSystemClassFactory.Create(User, sPassword);

			if (fileSystem == null)
			{
				return false;
			}

			SetFileSystemObject(fileSystem);
			return true;
        }

        public bool CreateFileSystem()
        {
            IFileSystem fileSystem = _fileSystemClassFactory.Create(User, "");

            if (fileSystem == null)
            {
                return false;
            }

            this.SetFileSystemObject(fileSystem);
            return true;
        }

        public void Process(byte[] abData)
        {
            string message = Encoding.ASCII.GetString(abData);
            message = message.Substring(0, message.IndexOf('\r'));

            FtpServerMessageHandler.SendMessage(Id, message);

            string command;
            string value;

            int spaceIndex = message.IndexOf(' ');

            if (spaceIndex < 0)
            {
                command = message.ToUpper();
                value = string.Empty;
            }
            else
            {
                command = message.Substring(0, spaceIndex).ToUpper();
                value = message.Substring(command.Length + 1);
            }

            var handler = _commandHashTable[command] as FtpCommandHandler;

            if (handler == null)
            {
                FtpServerMessageHandler.SendMessage(Id, string.Format("\"{0}\" : Unknown command", command));
                SocketHelpers.Send(Socket, "550 Unknown command\r\n");
            }
            else
            {
                handler.Process(value);
            }
        }

		private void LoadCommands()
		{
			this.AddCommand(new UserCommandHandler(this));
            this.AddCommand(new PasswordCommandHandler(this));
		    this.AddCommand(new QuitCommandHandler(this));
			this.AddCommand(new CwdCommandHandler(this));
			this.AddCommand(new PortCommandHandler(this));
			this.AddCommand(new PasvCommandHandler(this));
			this.AddCommand(new ListCommandHandler(this));
			this.AddCommand(new NlstCommandHandler(this));
			this.AddCommand(new PwdCommandHandler(this));
			this.AddCommand(new XPwdCommandHandler(this));
			this.AddCommand(new TypeCommandHandler(this));
			this.AddCommand(new RetrCommandHandler(this));
			this.AddCommand(new NoopCommandHandler(this));
			this.AddCommand(new SizeCommandHandler(this));
			this.AddCommand(new DeleCommandHandler(this));
			this.AddCommand(new AlloCommandHandler(this));
			this.AddCommand(new StoreCommandHandler(this));
			this.AddCommand(new MakeDirectoryCommandHandler(this));
			this.AddCommand(new RemoveDirectoryCommandHandler(this));
			this.AddCommand(new AppendCommandHandler(this));
			this.AddCommand(new RenameStartCommandHandler(this));
			this.AddCommand(new RenameCompleteCommandHandler(this));
			this.AddCommand(new XMkdCommandHandler(this));
            this.AddCommand(new XRmdCommandHandler(this));
            this.AddCommand(new CdUpCommandHandler(this));
		}

		private void AddCommand(FtpCommandHandler handler)
		{
			this._commandHashTable.Add(handler.Command, handler);
		}
	}
}
