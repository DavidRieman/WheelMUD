//-----------------------------------------------------------------------------
// <copyright file="StandardFileObject.cs" company="WheelMUD Development Team">
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
    using System.IO;
    using WheelMUD.Ftp.General;

    public class StandardFileObject : LoadedClass, IFile
    {
        private FileStream fileStream;

        public StandardFileObject(string path, bool write)
        {
            try
            {
                var mode = write ? FileMode.OpenOrCreate : FileMode.Open;
                var access = write ? FileAccess.Write : FileAccess.Read;
                this.fileStream = new FileStream(path, mode, access);

                if (write)
                {
                    this.fileStream.Seek(0, System.IO.SeekOrigin.End);
                }

                this.isLoaded = true;
            }
            catch (IOException)
            {
                this.fileStream = null;
            }
        }

        public int Read(byte[] data, int dataSize)
        {
            if (this.fileStream == null)
            {
                return 0;
            }

            try
            {
                return this.fileStream.Read(data, 0, dataSize);
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public int Write(byte[] data, int dataSize)
        {
            if (this.fileStream == null)
            {
                return 0;
            }

            try
            {
                this.fileStream.Write(data, 0, dataSize);
            }
            catch (IOException)
            {
                return 0;
            }

            return dataSize;
        }

        public void Close()
        {
            if (this.fileStream != null)
            {
                try
                {
                    this.fileStream.Close();
                }
                catch (IOException)
                {
                }

                this.fileStream = null;
            }
        }
    }
}