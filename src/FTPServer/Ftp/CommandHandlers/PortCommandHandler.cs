//-----------------------------------------------------------------------------
// <copyright file="PortCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
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
                return GetMessage(550, "Error in setting up data connection");
            }

            int socketPort = (int.Parse(data[4]) * 256) + int.Parse(data[5]);

            ConnectionObject.PortCommandSocketPort = socketPort;
            ConnectionObject.PortCommandSocketAddress = string.Join(".", data, 0, 4);

            return GetMessage(200, string.Format("{0} command succeeded", Command));
        }
    }
}