//-----------------------------------------------------------------------------
// <copyright file="FtpServer.cs" company="WheelMUD Development Team">
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
    using System.ComponentModel.Composition;
    using System.Net.Sockets;
    using System.Threading;
    using WheelMUD.Ftp.FileSystem;
    using WheelMUD.Ftp.General;
    using WheelMUD.Interfaces;

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

        /// <summary>The host system that the GameEngine is subscribing to.</summary>
        private ISystemHost host;

        public FtpServer(IFileSystemClassFactory fileSystemClassFactory)
        {
            this.connections = new ArrayList();
            this.fileSystemClassFactory = fileSystemClassFactory;
        }

        public FtpServer() : this(null)
        {
        }

        ~FtpServer()
        {
            if (this.serverSocketListener != null)
            {
                this.serverSocketListener.Stop();
            }
        }

        public delegate void ConnectionHandler(int nId);

        public event ConnectionHandler ConnectionClosed;

        public event ConnectionHandler NewConnection;

        public void Start(int port)
        {
            ////this.host.UpdateSystemHost(this, new SystemUpdateArgs("Starting..."));

            this.port = port;
            this.serverThread = new Thread(ThreadRun);
            this.serverThread.Start();

            ////this.host.UpdateSystemHost(this, new SystemUpdateArgs("Started"));
        }

        public void Start()
        {
            this.host.UpdateSystemHost(this, "Starting...");
            this.Start(21);
            this.host.UpdateSystemHost(this, "Started");
        }

        public void Stop()
        {
            this.host.UpdateSystemHost(this, "Stopping");

            foreach (object t in connections)
            {
                var handler = t as FtpSocketHandler;

                if (handler != null)
                {
                    handler.Stop();
                }
            }

            this.serverSocketListener.Stop();
            this.serverThread.Join();

            this.host.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Allows the sub system host to receive an update when subscribed to this system.</summary>
        /// <param name="sender">The sender of the update.</param>
        /// <param name="msg">The message to be sent.</param>
        public void UpdateSubSystemHost(ISubSystem sender, string msg)
        {
            this.host.UpdateSystemHost(this, msg);
        }

        /// <summary>Subscribes this system to the specified system host, so that host can receive updates.</summary>
        /// <param name="sender">The system host to receive our updates.</param>
        public void SubscribeToSystemHost(ISystemHost sender)
        {
            this.host = sender;
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
            SocketHelpers.Send(socket, System.Text.Encoding.ASCII.GetBytes("220 WheelMUD FTP Server Ready\r\n"));
        }

        private void ThreadRun()
        {
            this.serverSocketListener = SocketHelpers.CreateTcpListener(port);

            if (this.serverSocketListener != null)
            {
                this.serverSocketListener.Start();

                FtpServerMessageHandler.SendMessage(0, "FTP Server Started");
                this.UpdateSubSystemHost(this, "FTP Server Multi-Threaded Core Started");

                bool shouldContinue = true;

                while (shouldContinue)
                {
                    TcpClient socket = null;

                    try
                    {
                        socket = serverSocketListener.AcceptTcpClient();
                    }
                    catch (SocketException)
                    {
                        shouldContinue = false;
                    }
                    finally
                    {
                        if (socket == null)
                        {
                            shouldContinue = false;
                        }
                        else
                        {
                            socket.NoDelay = false;

                            id++;

                            FtpServerMessageHandler.SendMessage(id, "New connection");
                            this.UpdateSubSystemHost(this, "New connection id = " + id);

                            SendAcceptMessage(socket);
                            this.InitialiseSocketHandler(socket);
                        }
                    }
                }
            }
            else
            {
                FtpServerMessageHandler.SendMessage(0, "Error in starting FTP server");
                this.UpdateSubSystemHost(this, "Error in starting FTP server");
            }
        }

        private void InitialiseSocketHandler(TcpClient socket)
        {
            var handler = new FtpSocketHandler(fileSystemClassFactory, id);
            handler.Start(socket);
            this.connections.Add(handler);
            handler.Closed += HandleClosed;
            if (this.NewConnection != null)
            {
                this.NewConnection(id);
            }
        }

        private void HandleClosed(FtpSocketHandler handler)
        {
            this.UpdateSubSystemHost(this, string.Format("Client #{0} has disconnected", handler.Id));
            this.connections.Remove(handler);
            if (this.ConnectionClosed != null)
            {
                this.ConnectionClosed(handler.Id);
            }
        }
    }
}