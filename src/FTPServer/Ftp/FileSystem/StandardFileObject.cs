//-----------------------------------------------------------------------------
// <copyright file="StandardFileObject.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.IO;
using WheelMUD.Ftp.General;

namespace WheelMUD.Ftp.FileSystem
{
    public class StandardFileObject : LoadedClass, IFile
    {
        private FileStream fileStream;

        public StandardFileObject(string path, bool write)
        {
            try
            {
                var mode = write ? FileMode.OpenOrCreate : FileMode.Open;
                var access = write ? FileAccess.Write : FileAccess.Read;
                fileStream = new FileStream(path, mode, access);

                if (write)
                {
                    fileStream.Seek(0, SeekOrigin.End);
                }

                isLoaded = true;
            }
            catch (IOException)
            {
                fileStream = null;
            }
        }

        public int Read(byte[] data, int dataSize)
        {
            if (fileStream == null)
            {
                return 0;
            }

            try
            {
                return fileStream.Read(data, 0, dataSize);
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public int Write(byte[] data, int dataSize)
        {
            if (fileStream == null)
            {
                return 0;
            }

            try
            {
                fileStream.Write(data, 0, dataSize);
            }
            catch (IOException)
            {
                return 0;
            }

            return dataSize;
        }

        public void Close()
        {
            if (fileStream != null)
            {
                try
                {
                    fileStream.Close();
                }
                catch (IOException)
                {
                }

                fileStream = null;
            }
        }
    }
}