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
		private TcpListener _serverSocketListener;
		private Thread _serverThread;
		private int _id;
		private readonly ArrayList _connections;
		private int _port;
        private readonly IFileSystemClassFactory _fileSystemClassFactory;

        /// <summary>The host system that the GameEngine is subscribing to.</summary>
        private ISystemHost _host;

		public delegate void ConnectionHandler(int nId);
		public event ConnectionHandler ConnectionClosed;
		public event ConnectionHandler NewConnection;

		public FtpServer(IFileSystemClassFactory fileSystemClassFactory)
		{
            this._connections = new ArrayList();
            this._fileSystemClassFactory = fileSystemClassFactory;
        }

        public FtpServer() : this(null)
        {
        }
		
        ~FtpServer()
		{
            if (this._serverSocketListener != null)
			{
                this._serverSocketListener.Stop();
			}
		}

		public void Start(int port)
        {
            //this.host.UpdateSystemHost(this, new SystemUpdateArgs("Starting..."));

			this._port = port;
            this._serverThread = new Thread(ThreadRun);
            this._serverThread.Start();

            //this.host.UpdateSystemHost(this, new SystemUpdateArgs("Started"));
		}

        public void Start()
        {
            this._host.UpdateSystemHost(this, "Starting...");
            this.Start(21);
            this._host.UpdateSystemHost(this, "Started");
        }

        public void Stop()
        {
            this._host.UpdateSystemHost(this, "Stopping");

            foreach (object t in _connections)
            {
                var handler = t as FtpSocketHandler;

                if (handler != null)
                {
                    handler.Stop();
                }
            }

            this._serverSocketListener.Stop();
            this._serverThread.Join();

            this._host.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Allows the sub system host to receive an update when subscribed to this system.</summary>
        /// <param name="sender">The sender of the update.</param>
        /// <param name="msg">The message to be sent.</param>
        public void UpdateSubSystemHost(ISubSystem sender, string msg)
        {
            this._host.UpdateSystemHost(this, msg);
        }

        /// <summary>Subscribes this system to the specified system host, so that host can receive updates.</summary>
        /// <param name="sender">The system host to receive our updates.</param>
        public void SubscribeToSystemHost(ISystemHost sender)
        {
            this._host = sender;
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

        private void ThreadRun()
        {
            this._serverSocketListener = SocketHelpers.CreateTcpListener(_port);

            if (this._serverSocketListener != null)
            {
                this._serverSocketListener.Start();

                FtpServerMessageHandler.SendMessage(0, "FTP Server Started");
                this.UpdateSubSystemHost(this, "FTP Server Multi-Threaded Core Started");

                bool fContinue = true;

                while (fContinue)
                {
                    TcpClient socket = null;

                    try
                    {
                        socket = _serverSocketListener.AcceptTcpClient();
                    }
                    catch (SocketException)
                    {
                        fContinue = false;
                    }
                    finally
                    {
                        if (socket == null)
                        {
                            fContinue = false;
                        }
                        else
                        {
                            socket.NoDelay = false;

                            _id++;

                            FtpServerMessageHandler.SendMessage(_id, "New connection");
                            this.UpdateSubSystemHost(this, "New connection id = " + _id);

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

        private static void SendAcceptMessage(TcpClient socket)
        {
            SocketHelpers.Send(socket, System.Text.Encoding.ASCII.GetBytes("220 WheelMUD FTP Server Ready\r\n"));
        }

        private void InitialiseSocketHandler(TcpClient socket)
        {
            var handler = new FtpSocketHandler(_fileSystemClassFactory, _id);

            handler.Start(socket);

            this._connections.Add(handler);

            handler.Closed += handler_Closed;

            if (this.NewConnection != null)
            {
                this.NewConnection(_id);
            }
        }

		private void handler_Closed(FtpSocketHandler handler)
        {
            this.UpdateSubSystemHost(this, string.Format("Client #{0} has disconnected", handler.Id));

			this._connections.Remove(handler);

            if (this.ConnectionClosed != null)
            {
                this.ConnectionClosed(handler.Id);
			}
		}
	}
}
