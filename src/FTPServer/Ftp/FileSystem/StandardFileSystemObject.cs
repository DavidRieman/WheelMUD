//-----------------------------------------------------------------------------
// <copyright file="StandardFileSystemObject.cs" company="WheelMUD Development Team">
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

    public class StandardFileSystemObject : IFileSystem
    {
        private string startDirectory = string.Empty;

        public StandardFileSystemObject(string sStartDirectory)
        {
            startDirectory = sStartDirectory;
        }

        private string GetPath(string sPath)
        {
            if (sPath.Length == 0)
            {
                return startDirectory;
            }

            if (sPath[0] == '\\')
            {
                sPath = sPath.Substring(1);
            }

            return Path.Combine(startDirectory, sPath);
        }

        public IFile OpenFile(string sPath, bool fWrite)
        {
            var file = new StandardFileObject(this.GetPath(sPath), fWrite);

            if (file.Loaded)
            {
                return file;
            }

            return null;
        }

        public IFileInfo GetFileInfo(string sPath)
        {
            var info = new StandardFileInfoObject(this.GetPath(sPath));
            if (info.Loaded)
            {
                return info;
            }

            return null;
        }

        public string[] GetFiles(string sPath)
        {
            string sCurrentPath = this.GetPath(sPath);
            string[] asFiles = Directory.GetFiles(sCurrentPath);
            this.RemovePath(asFiles, sCurrentPath);
            return asFiles;
        }

        public string[] GetFiles(string sPath, string sWildcard)
        {
            string sCurrentPath = this.GetPath(sPath);
            string[] asFiles = Directory.GetFiles(sCurrentPath, sWildcard);
            this.RemovePath(asFiles, sCurrentPath);
            return asFiles;
        }

        public string[] GetDirectories(string sPath)
        {
            string sCurrentPath = this.GetPath(sPath);
            string[] asFiles = Directory.GetDirectories(sCurrentPath);
            this.RemovePath(asFiles, sCurrentPath);
            return asFiles;
        }

        public string[] GetDirectories(string sPath, string sWildcard)
        {
            string sCurrentPath = this.GetPath(sPath);
            string[] asFiles = Directory.GetDirectories(sCurrentPath, sWildcard);
            this.RemovePath(asFiles, sCurrentPath);
            return asFiles;
        }

        public bool DirectoryExists(string sPath)
        {
            return Directory.Exists(this.GetPath(sPath));
        }

        public bool FileExists(string sPath)
        {
            return File.Exists(this.GetPath(sPath));
        }

        public bool Move(string sOldPath, string sNewPath)
        {
            string sFullPathOld = this.GetPath(sOldPath);
            string sFullPathNew = this.GetPath(sNewPath);

            try
            {
                FileInfo info = new FileInfo(sFullPathOld);
                if (info == null)
                {
                    return false;
                }

                if ((info.Attributes & FileAttributes.Directory) != 0)
                {
                    Directory.Move(sFullPathOld, sFullPathNew);
                }
                else
                {
                    File.Move(sFullPathOld, sFullPathNew);
                }
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool Delete(string sPath)
        {
            try
            {
                string sFullPath = this.GetPath(sPath);

                var info = new FileInfo(sFullPath);
                if (info == null)
                {
                    return false;
                }

                if ((info.Attributes & FileAttributes.Directory) != 0)
                {
                    Directory.Delete(sFullPath);
                }
                else
                {
                    File.Delete(sFullPath);
                }
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool CreateDirectory(string sPath)
        {
            string sFullPath = this.GetPath(sPath);

            try
            {
                Directory.CreateDirectory(sFullPath);
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        private void RemovePath(string[] asFiles, string sPath)
        {
            int nIndex = 0;

            string sPathLowerCase = sPath.ToLower();

            foreach (string file in asFiles)
            {
                if (file.Substring(0, sPath.Length).ToLower() == sPathLowerCase)
                {
                    string fileName = file.Substring(sPath.Length);

                    if (fileName.Length > 0 && fileName[0] == '\\')
                    {
                        fileName = fileName.Substring(1);
                    }

                    asFiles[nIndex] = fileName;
                }

                nIndex += 1;
            }
        }
    }
}
