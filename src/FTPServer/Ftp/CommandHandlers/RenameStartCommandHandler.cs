//-----------------------------------------------------------------------------
// <copyright file="RenameStartCommandHandler.cs" company="WheelMUD Development Team">
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
    using WheelMUD.Ftp.FileSystem;

    /// <summary>Starts a rename file operation.</summary>
    public class RenameStartCommandHandler : FtpCommandHandler
    {
        public RenameStartCommandHandler(FtpConnectionObject connectionObject)
            : base("RNFR", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string file = this.GetPath(message);
            this.ConnectionObject.FileToRename = file;

            var info = this.ConnectionObject.FileSystemObject.GetFileInfo(file);
            if (info == null)
            {
                return this.GetMessage(550, string.Format("File does not exist ({0}).", file));
            }

            this.ConnectionObject.RenameDirectory = info.IsDirectory();
            return this.GetMessage(350, string.Format("Rename file started ({0}).", file));
        }
    }
}