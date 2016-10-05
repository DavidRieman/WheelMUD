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
        {
        }

        public static bool Send(TcpClient socket, byte[] messageData)
        {
            return Send(socket, messageData, 0, messageData.Length);
        }

        public static bool Send(TcpClient socket, byte[] messageData, int start)
        {
            return Send(socket, messageData, start, messageData.Length - start);
        }

        public static bool Send(TcpClient socket, byte[] messageData, int start, int length)
        {
            try
            {
                var writer = new BinaryWriter(socket.GetStream());
                writer.Write(messageData, start, length);
                writer.Flush();
            }
            catch (IOException)
            {
                return false;
            }
            catch (SocketException)
            {
                return false;
            }

            return true;
        }

        public static bool Send(TcpClient socket, string message)
        {
            byte[] messageBuffer = System.Text.Encoding.ASCII.GetBytes(message);
            return Send(socket, messageBuffer);
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

        public static TcpClient CreateTcpClient(string address, int port)
        {
            TcpClient client;
            try
            {
                client = new TcpClient(address, port);
            }
            catch (SocketException)
            {
                client = null;
            }

            return client;
        }

        public static TcpListener CreateTcpListener(int port)
        {
            TcpListener listener;
            try
            {
                listener = new TcpListener(IPAddress.Any, port);
            }
            catch (SocketException)
            {
                listener = null;
            }

            return listener;
        }

        public static IPAddress GetLocalAddress()
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            if (hostEntry.AddressList.Length == 0)
            {
                return null;
            }

            return hostEntry.AddressList[0];
        }
    }
}