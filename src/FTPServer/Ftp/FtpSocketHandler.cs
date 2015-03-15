//-----------------------------------------------------------------------------
// <copyright file="FtpSocketHandler.cs" company="WheelMUD Development Team">
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
    using System;
    using System.Net.Sockets;
    using System.Threading;
    using WheelMUD.Ftp.FileSystem;
    using WheelMUD.Ftp.General;

    /// <summary>
    /// Contains the socket read functionality. Works on its own thread since all socket operation is blocking.
    /// </summary>
    public class FtpSocketHandler
    {
        private TcpClient _clientSocket;
        private Thread _mainThread;
        private const int InternalBufferSize = 65536;
        private FtpConnectionObject _connectionCommands;
        private readonly IFileSystemClassFactory _fileSystemClassFactory;

        public delegate void CloseHandler(FtpSocketHandler handler);
        public event CloseHandler Closed;

        public FtpSocketHandler(IFileSystemClassFactory fileSystemClassFactory, int nId)
        {
            this.Id = nId;
            this._fileSystemClassFactory = fileSystemClassFactory;
        }

        public int Id { get; private set; }

        public void Start(TcpClient socket)
        {
            this._clientSocket = socket;

            this._connectionCommands = new FtpConnectionObject(_fileSystemClassFactory, Id, socket);

            this._mainThread = new Thread(ThreadRun);
            this._mainThread.Name = "ThreadRun:" + Id;
            this._mainThread.Start();
        }

        public void Stop()
        {
            SocketHelpers.Close(this._clientSocket);
            //_mainThread.Join();
        }

        private void ThreadRun()
        {
            var abData = new byte[InternalBufferSize];

            try
            {
                //while (_clientSocket.Connected)
                //{
                int nReceived = this._clientSocket.GetStream().Read(abData, 0, InternalBufferSize);

                while (nReceived > 0)
                {
                    this._connectionCommands.Process(abData);

                    nReceived = this._clientSocket.GetStream().Read(abData, 0, InternalBufferSize);
                }
                //}
            }
            //catch (SocketException socketException)
            //{
            //    string msg = socketException.Message;
            //    Console.WriteLine("FtpSocketHandler.ThreadRun()" + Environment.NewLine + msg);
            //}
            //catch (IOException ioException)
            //{
            //    string msg = ioException.Message;
            //    Console.WriteLine("FtpSocketHandler.ThreadRun()" + Environment.NewLine + msg);

            //    CloseSocket();
            //}
            //catch(InvalidOperationException invalidOperationException)
            //{
            //    string msg = invalidOperationException.Message;
            //    Console.WriteLine("FtpSocketHandler.ThreadRun()" + Environment.NewLine + msg);

            //    CloseSocket();
            //}
            catch (Exception e)
            {
                string msg = e.Message;
                Console.WriteLine("FtpSocketHandler.ThreadRun() ::" + Environment.NewLine + msg);

                this.CloseSocket();
            }

            this.CloseSocket();
        }

        private void CloseSocket()
        {
            FtpServerMessageHandler.SendMessage(Id, "Connection closed");

            if (this._clientSocket.Connected)
            {
                this._clientSocket.Close();
            }

            if (this.Closed != null)
            {
                this.Closed(this);
            }
        }
    }
}
