//-----------------------------------------------------------------------------
// <copyright file="FtpServer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections;
using System.ComponentModel.Composition;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WheelMUD.Ftp.FileSystem;
using WheelMUD.Ftp.General;
using WheelMUD.Interfaces;
using WheelMUD.Utilities;

namespace WheelMUD.Ftp
{
    /// <summary>Listens for incoming connections and accepts them.</summary>
    /// <remarks>Incoming socket connections are then passed to the socket handling class (FtpSocketHandler).</remarks>
    [Export(typeof(ISystemPlugIn))]
    public class FtpServer : ISystemPlugIn, ISubSystem
    {
        private readonly IFileSystemClassFactory fileSystemClassFactory;
        private readonly ArrayList connections;
        private TcpListener serverSocketListener;
        private Thread serverThread;
        private int id;
        private int port;
        private bool shuttingDown = false;

        /// <summary>The host system that the GameEngine is subscribing to.</summary>
        private ISystemHost host;

        public FtpServer(IFileSystemClassFactory fileSystemClassFactory)
        {
            connections = new ArrayList();
            this.fileSystemClassFactory = fileSystemClassFactory;
        }

        public FtpServer() : this(null)
        {
        }

        ~FtpServer()
        {
            serverSocketListener?.Stop();
        }

        public delegate void ConnectionHandler(int nId);

        public event ConnectionHandler ConnectionClosed;

        public event ConnectionHandler NewConnection;

        public void Start(int port)
        {
            this.port = port;
            serverThread = new Thread(ThreadRun);
            serverThread.Start();
        }

        public void Start()
        {
            int port = GameConfiguration.TryGetAppConfigInt("FtpPort", 0);
            if (port > 0)
            {
                host.UpdateSystemHost(this, "Starting...");
                Start();
                host.UpdateSystemHost(this, "Started");
            }
            else
            {
                host.UpdateSystemHost(this, "Omitting FTP support.");
            }
        }

        public void Stop()
        {
            host.UpdateSystemHost(this, "Stopping");
            shuttingDown = true;

            foreach (object t in connections)
            {
                var handler = t as FtpSocketHandler;
                handler?.Stop();
            }

            serverSocketListener?.Stop();

            serverThread?.Join();

            host.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Allows the sub system host to receive an update when subscribed to this system.</summary>
        /// <param name="sender">The sender of the update.</param>
        /// <param name="msg">The message to be sent.</param>
        public void UpdateSubSystemHost(ISubSystem sender, string msg)
        {
            host.UpdateSystemHost(this, msg);
        }

        /// <summary>Subscribes this system to the specified system host, so that host can receive updates.</summary>
        /// <param name="sender">The system host to receive our updates.</param>
        public void SubscribeToSystemHost(ISystemHost sender)
        {
            host = sender;
        }

        /// <summary>Subscribe to receive system updates from this system.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        public void SubscribeToSystem(ISubSystemHost sender)
        {
        }

        /// <summary>Inform subscribed system(s) of the specified update.</summary>
        /// <param name="msg">The message to be sent to subscribed system(s).</param>
        public void InformSubscribedSystem(string msg)
        {
        }

        private static void SendAcceptMessage(TcpClient socket)
        {
            SocketHelpers.Send(socket, Encoding.ASCII.GetBytes("220 WheelMUD FTP Server Ready\r\n"));
        }

        private void ThreadRun()
        {
            serverSocketListener = SocketHelpers.CreateTcpListener(port);

            if (serverSocketListener != null)
            {
                serverSocketListener.Start();

                FtpServerMessageHandler.SendMessage(0, "FTP Server Started");
                UpdateSubSystemHost(this, "FTP Server Multi-Threaded Core Started");

                while (!shuttingDown)
                {
                    TcpClient socket = null;
                    try
                    {
                        socket = serverSocketListener.AcceptTcpClient();
                    }
                    catch (SocketException)
                    {
                        shuttingDown = true;
                    }
                    finally
                    {
                        if (socket == null)
                        {
                            shuttingDown = true;
                        }
                        else
                        {
                            socket.NoDelay = false;
                            id++;

                            FtpServerMessageHandler.SendMessage(id, "New connection");
                            UpdateSubSystemHost(this, "New connection id = " + id);

                            SendAcceptMessage(socket);
                            InitialiseSocketHandler(socket);
                        }
                    }
                }
            }
            else
            {
                FtpServerMessageHandler.SendMessage(0, "Error in starting FTP server");
                UpdateSubSystemHost(this, "Error in starting FTP server");
            }
        }

        private void InitialiseSocketHandler(TcpClient socket)
        {
            var handler = new FtpSocketHandler(fileSystemClassFactory, id);
            handler.Start(socket);
            connections.Add(handler);
            handler.Closed += HandleClosed;
            NewConnection?.Invoke(id);
        }

        private void HandleClosed(FtpSocketHandler handler)
        {
            UpdateSubSystemHost(this, $"Client #{handler.Id} has disconnected");
            connections.Remove(handler);
            ConnectionClosed?.Invoke(handler.Id);
        }
    }
}