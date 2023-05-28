//-----------------------------------------------------------------------------
// <copyright file="StandardFileSystemClassFactory.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities;

namespace WheelMUD.Ftp.FileSystem
{
    public class StandardFileSystemClassFactory : IFileSystemClassFactory
    {
        public IFileSystem Create(string user, string password)
        {
            // TODO: https://github.com/DavidRieman/WheelMUD/issues/176: Repair FTP plugin.
            //if (UserData.Instance.HasUser(user) && UserData.GetInstance().GetUserPassword(user) == password)
            //{
            //    return new StandardFileSystemObject(UserData.Instance.GetUserStartingDirectory(user));
            //}

            string serverFolder = GameConfiguration.GetAppConfigString("FTPServerRootFolder");
            return new StandardFileSystemObject(serverFolder);
        }
    }
}