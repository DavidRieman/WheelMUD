//-----------------------------------------------------------------------------
// <copyright file="Interfaces.cs" company="WheelMUD Development Team">
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

    public interface IFile
    {
        int Read(byte[] data, int dataSize);

        int Write(byte[] data, int dataSize);

        void Close();
    }

    public interface IFileInfo
    {
        DateTime GetModifiedTime();

        long GetSize();

        string GetAttributeString();

        bool IsDirectory();
    }

    public interface IFileSystem
    {
        IFile OpenFile(string path, bool write);

        IFileInfo GetFileInfo(string path);

        string[] GetFiles(string path);

        string[] GetFiles(string path, string wildcard);

        string[] GetDirectories(string path);

        string[] GetDirectories(string path, string wildcard);

        bool DirectoryExists(string path);

        bool FileExists(string path);

        bool CreateDirectory(string path);

        bool Move(string oldPath, string newPath);

        bool Delete(string path);
    }

    public interface IFileSystemClassFactory
    {
        IFileSystem Create(string user, string password);
    }
}