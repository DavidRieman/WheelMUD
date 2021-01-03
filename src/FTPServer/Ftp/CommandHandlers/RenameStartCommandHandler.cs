//-----------------------------------------------------------------------------
// <copyright file="RenameStartCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    /// <summary>Starts a rename file operation.</summary>
    public class RenameStartCommandHandler : FtpCommandHandler
    {
        public RenameStartCommandHandler(FtpConnectionObject connectionObject)
            : base("RNFR", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string file = GetPath(message);
            ConnectionObject.FileToRename = file;

            var info = ConnectionObject.FileSystemObject.GetFileInfo(file);
            if (info == null)
            {
                return GetMessage(550, string.Format("File does not exist ({0}).", file));
            }

            ConnectionObject.RenameDirectory = info.IsDirectory();
            return GetMessage(350, string.Format("Rename file started ({0}).", file));
        }
    }
}