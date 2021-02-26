//-----------------------------------------------------------------------------
// <copyright file="FtpConnectionObject.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
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
        private readonly Hashtable commandHashTable;
        private readonly IFileSystemClassFactory fileSystemClassFactory;

        public FtpConnectionObject(IFileSystemClassFactory fileSystemClassFactory, int nId, TcpClient socket)
            : base(nId, socket)
        {
            commandHashTable = new Hashtable();
            this.fileSystemClassFactory = fileSystemClassFactory;

            LoadCommands();
        }

        public bool Login(string password)
        {
            var fileSystem = fileSystemClassFactory.Create(User, password);
            if (fileSystem == null)
            {
                return false;
            }

            SetFileSystemObject(fileSystem);
            return true;
        }

        public bool CreateFileSystem()
        {
            var fileSystem = fileSystemClassFactory.Create(User, string.Empty);
            if (fileSystem == null)
            {
                return false;
            }

            SetFileSystemObject(fileSystem);
            return true;
        }

        public void Process(byte[] data)
        {
            var message = Encoding.ASCII.GetString(data);
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

            var handler = commandHashTable[command] as FtpCommandHandler;
            if (handler == null)
            {
                FtpServerMessageHandler.SendMessage(Id, $"\"{command}\" : Unknown command");
                SocketHelpers.Send(Socket, "550 Unknown command\r\n");
            }
            else
            {
                handler.Process(value);
            }
        }

        private void LoadCommands()
        {
            AddCommand(new UserCommandHandler(this));
            AddCommand(new PasswordCommandHandler(this));
            AddCommand(new QuitCommandHandler(this));
            AddCommand(new CwdCommandHandler(this));
            AddCommand(new PortCommandHandler(this));
            AddCommand(new PasvCommandHandler(this));
            AddCommand(new ListCommandHandler(this));
            AddCommand(new NlstCommandHandler(this));
            AddCommand(new PwdCommandHandler(this));
            AddCommand(new XPwdCommandHandler(this));
            AddCommand(new TypeCommandHandler(this));
            AddCommand(new RetrCommandHandler(this));
            AddCommand(new NoopCommandHandler(this));
            AddCommand(new SizeCommandHandler(this));
            AddCommand(new DeleCommandHandler(this));
            AddCommand(new AlloCommandHandler(this));
            AddCommand(new StoreCommandHandler(this));
            AddCommand(new MakeDirectoryCommandHandler(this));
            AddCommand(new RemoveDirectoryCommandHandler(this));
            AddCommand(new AppendCommandHandler(this));
            AddCommand(new RenameStartCommandHandler(this));
            AddCommand(new RenameCompleteCommandHandler(this));
            AddCommand(new XMkdCommandHandler(this));
            AddCommand(new XRmdCommandHandler(this));
            AddCommand(new CdUpCommandHandler(this));
        }

        private void AddCommand(FtpCommandHandler handler)
        {
            commandHashTable.Add(handler.Command, handler);
        }
    }
}