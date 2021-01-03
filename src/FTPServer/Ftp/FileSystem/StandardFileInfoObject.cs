//-----------------------------------------------------------------------------
// <copyright file="StandardFileInfoObject.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
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

        public StandardFileInfoObject(string path)
        {
            try
            {
                fileInfo = new FileInfo(path);
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
            bool isDir = (fileInfo.Attributes & FileAttributes.Directory) != 0;
            bool isReadOnly = (fileInfo.Attributes & FileAttributes.ReadOnly) != 0;

            var builder = new StringBuilder();

            if (isDir)
            {
                builder.Append("d");
            }
            else
            {
                builder.Append("-");
            }

            builder.Append("r");

            if (isReadOnly)
            {
                builder.Append("-");
            }
            else
            {
                builder.Append("w");
            }

            if (isDir)
            {
                builder.Append("x");
            }
            else
            {
                builder.Append("-");
            }

            if (isDir)
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