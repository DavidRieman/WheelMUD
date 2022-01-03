//-----------------------------------------------------------------------------
// <copyright file="FtpSocketHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Net.Sockets;
using System.Threading;
using WheelMUD.Ftp.FileSystem;
using WheelMUD.Ftp.General;

namespace WheelMUD.Ftp
{
    /// <summary>Contains the socket read functionality. Works on its own thread since all socket operation is blocking.</summary>
    public class FtpSocketHandler
    {
        private const int InternalBufferSize = 65536;
        private readonly IFileSystemClassFactory fileSystemClassFactory;
        private TcpClient clientSocket;
        private Thread mainThread;
        private FtpConnectionObject connectionCommands;

        public FtpSocketHandler(IFileSystemClassFactory fileSystemClassFactory, int nId)
        {
            Id = nId;
            this.fileSystemClassFactory = fileSystemClassFactory;
        }

        public delegate void CloseHandler(FtpSocketHandler handler);

        public event CloseHandler Closed;

        public int Id { get; private set; }

        public void Start(TcpClient socket)
        {
            clientSocket = socket;

            connectionCommands = new FtpConnectionObject(fileSystemClassFactory, Id, socket);

            mainThread = new Thread(ThreadRun)
            {
                Name = "ThreadRun:" + Id
            };
            mainThread.Start();
        }

        public void Stop()
        {
            SocketHelpers.Close(clientSocket);
            ////_mainThread.Join();
        }

        private void ThreadRun()
        {
            var data = new byte[InternalBufferSize];
            try
            {
                //while (_clientSocket.Connected)
                //{
                int received = clientSocket.GetStream().Read(data, 0, InternalBufferSize);
                while (received > 0)
                {
                    connectionCommands.Process(data);

                    received = clientSocket.GetStream().Read(data, 0, InternalBufferSize);
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

                CloseSocket();
            }

            CloseSocket();
        }

        private void CloseSocket()
        {
            FtpServerMessageHandler.SendMessage(Id, "Connection closed");

            if (clientSocket.Connected)
            {
                clientSocket.Close();
            }

            if (Closed != null)
            {
                Closed(this);
            }
        }
    }
}