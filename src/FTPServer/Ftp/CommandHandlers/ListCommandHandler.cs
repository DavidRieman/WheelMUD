//-----------------------------------------------------------------------------
// <copyright file="ListCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    public class ListCommandHandler : ListCommandHandlerBase
    {
        public ListCommandHandler(FtpConnectionObject connectionObject)
            : base("LIST", connectionObject)
        {
        }

        protected override string BuildReply(string message, string[] files)
        {
            return BuildLongReply(files);
        }
    }
}