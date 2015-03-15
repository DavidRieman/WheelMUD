//-----------------------------------------------------------------------------
// <copyright file="StandardFileSystemClassFactory.cs" company="WheelMUD Development Team">
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
    using WheelMUD.Utilities;

    public class StandardFileSystemClassFactory : IFileSystemClassFactory
    {
        public IFileSystem Create(string sUser, string sPassword)
        {
            //if (UserData.GetInstance().HasUser(sUser) && UserData.GetInstance().GetUserPassword(sUser) == sPassword)
            //{
            //    return new StandardFileSystemObject(UserData.GetInstance().GetUserStartingDirectory(sUser));
            //}

            string serverFolder = MudEngineAttributes.Instance.FTPServerRootFolder;
            return new StandardFileSystemObject(serverFolder);
        }
    }
}
