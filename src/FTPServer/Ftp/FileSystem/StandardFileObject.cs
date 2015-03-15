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

        public StandardFileObject(string sPath, bool fWrite)
        {
            try
            {
                fileStream = new FileStream(sPath,
                    (fWrite) ? FileMode.OpenOrCreate : FileMode.Open,
                    (fWrite) ? FileAccess.Write : FileAccess.Read);

                if (fWrite)
                {
                    fileStream.Seek(0, System.IO.SeekOrigin.End);
                }

                isLoaded = true;
            }
            catch (IOException)
            {
                fileStream = null;
            }
        }

        public int Read(byte[] abData, int nDataSize)
        {
            if (fileStream == null)
            {
                return 0;
            }

            try
            {
                return fileStream.Read(abData, 0, nDataSize);
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public int Write(byte[] abData, int nDataSize)
        {
            if (fileStream == null)
            {
                return 0;
            }

            try
            {
                fileStream.Write(abData, 0, nDataSize);
            }
            catch (IOException)
            {
                return 0;
            }

            return nDataSize;
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
