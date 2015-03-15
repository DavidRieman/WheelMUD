//-----------------------------------------------------------------------------
// <copyright file="FtpReplySocket.cs" company="WheelMUD Development Team">
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
    using System.Net.Sockets;
    using System.Text;
    using WheelMUD.Ftp.General;

    /// <summary>
    /// Encapsulates the functionality necessary to send data along the reply connection
    /// </summary>
    public class FtpReplySocket
    {
        private TcpClient socket;

        public FtpReplySocket(FtpConnectionObject connectionObject)
        {
            this.socket = OpenSocket(connectionObject);
        }

        public bool Loaded
        {
            get
            {
                return this.socket != null;
            }
        }

        public void Close()
        {
            SocketHelpers.Close(socket);
            this.socket = null;
        }

        public bool Send(byte[] abData, int nSize)
        {
            return SocketHelpers.Send(socket, abData, 0, nSize);
        }

        public bool Send(char[] abChars, int nSize)
        {
            return SocketHelpers.Send(socket, System.Text.Encoding.ASCII.GetBytes(abChars), 0, nSize);
        }

        public bool Send(string sMessage)
        {
            byte[] abData = Encoding.ASCII.GetBytes(sMessage);
            return this.Send(abData, abData.Length);
        }

        public int Receive(byte[] abData)
        {
            return this.socket.GetStream().Read(abData, 0, abData.Length);
        }

        static private TcpClient OpenSocket(FtpConnectionObject connectionObject)
        {
            TcpClient socketPasv = connectionObject.PasvSocket;

            if (socketPasv != null)
            {
                connectionObject.PasvSocket = null;
                return socketPasv;
            }

            return SocketHelpers.CreateTcpClient(connectionObject.PortCommandSocketAddress, connectionObject.PortCommandSocketPort);
        }
    }
}
