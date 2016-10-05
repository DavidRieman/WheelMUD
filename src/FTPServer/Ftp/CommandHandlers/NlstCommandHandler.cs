//-----------------------------------------------------------------------------
// <copyright file="NlstCommandHandler.cs" company="WheelMUD Development Team">
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
    public class NlstCommandHandler : ListCommandHandlerBase
    {
        public NlstCommandHandler(FtpConnectionObject connectionObject)
            : base("NLST", connectionObject)
        {
        }

        protected override string BuildReply(string message, string[] asFiles)
        {
            if (message == "-L" || message == "-l")
            {
                return this.BuildLongReply(asFiles);
            }

            return this.BuildShortReply(asFiles);
        }
    }
}