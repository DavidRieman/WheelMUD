//-----------------------------------------------------------------------------
// <copyright file="CwdCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.IO;
using WheelMUD.Ftp.General;

namespace WheelMUD.Ftp.FtpCommands
{
    public class CwdCommandHandler : FtpCommandHandler
    {
        public CwdCommandHandler(FtpConnectionObject connectionObject)
            : base("CWD", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            message = message.Replace('/', '\\');

            if (!FileNameHelpers.IsValid(message))
            {
                return GetMessage(550, "Not a valid directory string.");
            }

            var dir = GetPath(message);
            if (!ConnectionObject.FileSystemObject.DirectoryExists(dir))
            {
                return GetMessage(550, "Not a valid directory.");
            }

            ConnectionObject.CurrentDirectory = Path.Combine(ConnectionObject.CurrentDirectory, message);
            return GetMessage(250, string.Format("CWD Successful ({0})", ConnectionObject.CurrentDirectory.Replace("\\", "/")));
        }
    }
}