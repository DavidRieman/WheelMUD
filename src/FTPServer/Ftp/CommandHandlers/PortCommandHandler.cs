//-----------------------------------------------------------------------------
// <copyright file="PortCommandHandler.cs" company="WheelMUD Development Team">
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
    public class PortCommandHandler : FtpCommandHandler
	{
		public PortCommandHandler(FtpConnectionObject connectionObject)
			: base("PORT", connectionObject)
		{}

		protected override string OnProcess(string sMessage)
		{
			string [] asData = sMessage.Split(new char [] { ',' });

			if (asData.Length != 6)
			{
				return this.GetMessage(550, "Error in setting up data connection");
			}

			int nSocketPort = int.Parse(asData[4]) * 256 + int.Parse(asData[5]);

			this.ConnectionObject.PortCommandSocketPort = nSocketPort;
			this.ConnectionObject.PortCommandSocketAddress = string.Join(".", asData, 0, 4);
			
			return this.GetMessage(200, string.Format("{0} command succeeded", Command));
		}
	}
}
