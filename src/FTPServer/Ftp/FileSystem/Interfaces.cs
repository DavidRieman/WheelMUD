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
        int Read(byte[] abData, int nDataSize);
        int Write(byte[] abData, int nDataSize);
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
        IFile OpenFile(string sPath, bool fWrite);
        IFileInfo GetFileInfo(string sPath);

        string[] GetFiles(string sPath);
        string[] GetFiles(string sPath, string sWildcard);
        string[] GetDirectories(string sPath);
        string[] GetDirectories(string sPath, string sWildcard);

        bool DirectoryExists(string sPath);
        bool FileExists(string sPath);

        bool CreateDirectory(string sPath);
        bool Move(string sOldPath, string sNewPath);
        bool Delete(string sPath);
    }

    public interface IFileSystemClassFactory
    {
        IFileSystem Create(string sUser, string sPassword);
    }
}
