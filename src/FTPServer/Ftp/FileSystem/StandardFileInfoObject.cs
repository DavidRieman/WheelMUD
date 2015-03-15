//-----------------------------------------------------------------------------
// <copyright file="StandardFileInfoObject.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: 2004 By David McClarnon (dmcclarnon@ntlworld.com)
//   Modified: June 16, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp.FileSystem
{
    using System;
    using System.IO;
    using System.Text;
    using WheelMUD.Ftp.General;

    public class StandardFileInfoObject : LoadedClass, IFileInfo
    {
        private FileInfo fileInfo = null;

        public StandardFileInfoObject(string sPath)
        {
            try
            {
                fileInfo = new FileInfo(sPath);
                isLoaded = true;
            }
            catch (IOException)
            {
                fileInfo = null;
            }
        }

        public bool IsDirectory()
        {
            return (fileInfo.Attributes & FileAttributes.Directory) != 0;
        }

        public DateTime GetModifiedTime()
        {
            return fileInfo.LastWriteTime;
        }

        public long GetSize()
        {
            return fileInfo.Length;
        }

        public string GetAttributeString()
        {
            bool fDirectory = (fileInfo.Attributes & FileAttributes.Directory) != 0;
            bool fReadOnly = (fileInfo.Attributes & FileAttributes.ReadOnly) != 0;

            var builder = new StringBuilder();

            if (fDirectory)
            {
                builder.Append("d");
            }
            else
            {
                builder.Append("-");
            }

            builder.Append("r");

            if (fReadOnly)
            {
                builder.Append("-");
            }
            else
            {
                builder.Append("w");
            }

            if (fDirectory)
            {
                builder.Append("x");
            }
            else
            {
                builder.Append("-");
            }

            if (fDirectory)
            {
                builder.Append("r-xr-x");
            }
            else
            {
                builder.Append("r--r--");
            }

            return builder.ToString();
        }
    }
}
