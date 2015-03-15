//-----------------------------------------------------------------------------
// <copyright file="NsltCommandHandler.cs" company="WheelMUD Development Team">
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
		{}

		protected override string BuildReply(string sMessage, string[] asFiles)
		{
		    if (sMessage == "-L" || sMessage == "-l")
			{
				return BuildLongReply(asFiles);
			}
		    
            return BuildShortReply(asFiles);
		}
	}
}
