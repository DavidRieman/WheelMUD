//-----------------------------------------------------------------------------
// <copyright file="NlstCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
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
                return BuildLongReply(asFiles);
            }

            return BuildShortReply(asFiles);
        }
    }
}