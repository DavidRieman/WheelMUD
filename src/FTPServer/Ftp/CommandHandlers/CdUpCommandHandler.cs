//-----------------------------------------------------------------------------
// <copyright file="CdUpCommandHandler.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: June 15, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FtpCommands
{
    using System.IO;

    public class CdUpCommandHandler : FtpCommandHandler
    {
        public CdUpCommandHandler(FtpConnectionObject connectionObject)
            : base("CDUP", connectionObject)
        {
        }

        protected override string OnProcess(string message)
        {
            string workingDirectory = this.GetPath(message);

            if (this.ConnectionObject.FileSystemObject.DirectoryExists(workingDirectory))
            {
                if (workingDirectory != "\\")
                {
                    string path = Directory.GetParent(workingDirectory).ToString();

                    if (path.Contains("C:"))
                    {
                        path = path.Replace("C:", string.Empty);
                        path = path.Replace('/', '\\');
                    }

                    this.ConnectionObject.CurrentDirectory = path;

                    return this.GetMessage(250, "CDUP command successful.");
                }
            }

            return this.GetMessage(550, "CDUP could not change directory.");
        }
    }
}