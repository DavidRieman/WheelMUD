//-----------------------------------------------------------------------------
// <copyright file="PasvCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    using WheelMUD.Ftp.General;

    public class PasvCommandHandler : FtpCommandHandler
    {
        private const int Port = 20;

        public PasvCommandHandler(FtpConnectionObject connectionObject)
            : base("PASV", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            if (ConnectionObject.PasvSocket == null)
            {
                var listener = SocketHelpers.CreateTcpListener(Port);
                if (listener == null)
                {
                    return GetMessage(550, string.Format("Couldn't start listener on port {0}", Port));
                }

                SendPasvReply();

                listener.Start();

                ConnectionObject.PasvSocket = listener.AcceptTcpClient();

                listener.Stop();
                return string.Empty;
            }

            SendPasvReply();
            return string.Empty;
        }

        private void SendPasvReply()
        {
            string addr = SocketHelpers.GetLocalAddress().ToString();
            addr = addr.Replace('.', ',');
            addr += ',';
            addr += "0";
            addr += ',';
            addr += Port.ToString();
            SocketHelpers.Send(ConnectionObject.Socket, string.Format("227 ={0}\r\n", addr));
        }
    }
}