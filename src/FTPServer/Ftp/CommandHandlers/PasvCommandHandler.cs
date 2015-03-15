//-----------------------------------------------------------------------------
// <copyright file="PasvCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: 2004 By David McClarnon (dmcclarnon@ntlworld.com)
//   Modified: June 16, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    using System.Net.Sockets;
    using WheelMUD.Ftp.General;

	public class PasvCommandHandler : FtpCommandHandler
	{
		const int m_nPort = 20;

		public PasvCommandHandler(FtpConnectionObject connectionObject)
			: base("PASV", connectionObject)
		{}

		protected override string OnProcess(string sMessage)
		{
			if (this.ConnectionObject.PasvSocket == null)
			{			
				TcpListener listener = SocketHelpers.CreateTcpListener(m_nPort);

				if (listener == null)
				{
                    return this.GetMessage(550, string.Format("Couldn't start listener on port {0}", m_nPort));
				}

				this.SendPasvReply();

				listener.Start();

				this.ConnectionObject.PasvSocket = listener.AcceptTcpClient();
				
				listener.Stop();
				return string.Empty;
			}

		    this.SendPasvReply();
		    return string.Empty;
		}

		private void SendPasvReply()
		{
			string sIpAddress = SocketHelpers.GetLocalAddress().ToString();
			sIpAddress = sIpAddress.Replace('.', ',');
			sIpAddress += ',';
			sIpAddress += "0";
			sIpAddress += ',';
			sIpAddress += m_nPort.ToString();
            SocketHelpers.Send(this.ConnectionObject.Socket, string.Format("227 ={0}\r\n", sIpAddress));
		}
	}
}
