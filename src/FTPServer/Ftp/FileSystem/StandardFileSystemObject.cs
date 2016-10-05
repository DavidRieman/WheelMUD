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

        public StandardFileSystemObject(string startDirectory)
        {
            this.startDirectory = startDirectory;
        }

        public IFile OpenFile(string path, bool write)
        {
            var file = new StandardFileObject(this.GetPath(path), write);
            return file.Loaded ? file : null;
        }

        public IFileInfo GetFileInfo(string path)
        {
            var info = new StandardFileInfoObject(this.GetPath(path));
            return info.Loaded ? info : null;
        }

        public string[] GetFiles(string path)
        {
            string currentPath = this.GetPath(path);
            string[] files = Directory.GetFiles(currentPath);
            this.RemovePath(files, currentPath);
            return files;
        }

        public string[] GetFiles(string path, string wildcard)
        {
            string currentPath = this.GetPath(path);
            string[] asFiles = Directory.GetFiles(currentPath, wildcard);
            this.RemovePath(asFiles, currentPath);
            return asFiles;
        }

        public string[] GetDirectories(string path)
        {
            string currentPath = this.GetPath(path);
            string[] files = Directory.GetDirectories(currentPath);
            this.RemovePath(files, currentPath);
            return files;
        }

        public string[] GetDirectories(string path, string wildcard)
        {
            string currentPath = this.GetPath(path);
            string[] files = Directory.GetDirectories(currentPath, wildcard);
            this.RemovePath(files, currentPath);
            return files;
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(this.GetPath(path));
        }

        public bool FileExists(string path)
        {
            return File.Exists(this.GetPath(path));
        }

        public bool Move(string oldPath, string newPath)
        {
            string fullPathOld = this.GetPath(oldPath);
            string fullPathNew = this.GetPath(newPath);

            try
            {
                FileInfo info = new FileInfo(fullPathOld);
                if (info == null)
                {
                    return false;
                }

                if ((info.Attributes & FileAttributes.Directory) != 0)
                {
                    Directory.Move(fullPathOld, fullPathNew);
                }
                else
                {
                    File.Move(fullPathOld, fullPathNew);
                }
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool Delete(string path)
        {
            try
            {
                var fullPath = this.GetPath(path);
                var info = new FileInfo(fullPath);
                if (info == null)
                {
                    return false;
                }

                if ((info.Attributes & FileAttributes.Directory) != 0)
                {
                    Directory.Delete(fullPath);
                }
                else
                {
                    File.Delete(fullPath);
                }
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool CreateDirectory(string path)
        {
            string fullPath = this.GetPath(path);

            try
            {
                Directory.CreateDirectory(fullPath);
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        private string GetPath(string path)
        {
            if (path.Length == 0)
            {
                return startDirectory;
            }

            if (path[0] == '\\')
            {
                path = path.Substring(1);
            }

            return Path.Combine(startDirectory, path);
        }

        private void RemovePath(string[] files, string path)
        {
            int index = 0;
            string pathLowerCase = path.ToLower();

            foreach (string file in files)
            {
                if (file.Substring(0, path.Length).ToLower() == pathLowerCase)
                {
                    string fileName = file.Substring(path.Length);

                    if (fileName.Length > 0 && fileName[0] == '\\')
                    {
                        fileName = fileName.Substring(1);
                    }

                    files[index] = fileName;
                }

                index += 1;
            }
        }
    }
}