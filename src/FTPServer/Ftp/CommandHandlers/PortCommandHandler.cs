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
        {
        }

        protected override string OnProcess(string message)
        {
            string[] data = message.Split(new char[] { ',' });
            if (data.Length != 6)
            {
                return this.GetMessage(550, "Error in setting up data connection");
            }

            int socketPort = (int.Parse(data[4]) * 256) + int.Parse(data[5]);

            this.ConnectionObject.PortCommandSocketPort = socketPort;
            this.ConnectionObject.PortCommandSocketAddress = string.Join(".", data, 0, 4);

            return this.GetMessage(200, string.Format("{0} command succeeded", this.Command));
        }
    }
}