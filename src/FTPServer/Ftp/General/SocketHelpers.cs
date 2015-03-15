//-----------------------------------------------------------------------------
// <copyright file="SocketHelpers.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: 2004 By David McClarnon (dmcclarnon@ntlworld.com)
//   Modified: June 16, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.General
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    public sealed class SocketHelpers
    {
        private SocketHelpers()
        { }

        public static bool Send(TcpClient socket, byte[] abMessage)
        {
            return Send(socket, abMessage, 0, abMessage.Length);
        }

        public static bool Send(TcpClient socket, byte[] abMessage, int nStart)
        {
            return Send(socket, abMessage, nStart, abMessage.Length - nStart);
        }

        public static bool Send(TcpClient socket, byte[] abMessage, int nStart, int nLength)
        {
            bool fReturn = true;

            try
            {
                var writer = new BinaryWriter(socket.GetStream());
                writer.Write(abMessage, nStart, nLength);
                writer.Flush();
            }
            catch (IOException)
            {
                fReturn = false;
            }
            catch (SocketException)
            {
                fReturn = false;
            }

            return fReturn;
        }

        public static bool Send(TcpClient socket, string sMessage)
        {
            byte[] abMessage = System.Text.Encoding.ASCII.GetBytes(sMessage);
            return Send(socket, abMessage);
        }

        public static void Close(TcpClient socket)
        {
            if (socket == null)
            {
                return;
            }

            try
            {
                if (socket.GetStream() != null)
                {
                    try
                    {
                        socket.GetStream().Flush();
                    }
                    catch (SocketException)
                    {
                    }

                    try
                    {
                        socket.GetStream().Close();
                    }
                    catch (SocketException)
                    {
                    }
                }
            }
            catch (SocketException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            try
            {
                socket.Close();
            }
            catch (SocketException)
            {
            }
        }

        public static TcpClient CreateTcpClient(string sAddress, int nPort)
        {
            TcpClient client;

            try
            {
                client = new TcpClient(sAddress, nPort);
            }
            catch (SocketException)
            {
                client = null;
            }

            return client;
        }

        public static TcpListener CreateTcpListener(int nPort)
        {
            TcpListener listener;

            try
            {
                listener = new TcpListener(IPAddress.Any, nPort);
            }
            catch (SocketException)
            {
                listener = null;
            }

            return listener;
        }

        public static IPAddress GetLocalAddress()
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());

            if (hostEntry.AddressList.Length == 0)
            {
                return null;
            }

            return hostEntry.AddressList[0];
        }
    }
}
