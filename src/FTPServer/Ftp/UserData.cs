//-----------------------------------------------------------------------------
// <copyright file="UserData.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>//   
//   Created: 2004 By David McClarnon (dmcclarnon@ntlworld.com)
//   Modified: June 16, 2010 by Fastalanasa.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Windows.Forms;

    [Serializable]
    class UserDataItem
    {
        private string password = string.Empty;
        private string startingDirectory = "C:\\";

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string StartingDirectory
        {
            get { return startingDirectory; }
            set { startingDirectory = value; }
        }
    }

    public class UserData
    {
        private static UserData instance = null;

        private Hashtable mapUserToData = null;

        protected UserData()
        {
            mapUserToData = new Hashtable();
        }

        public static UserData GetInstance()
        {
            if (instance == null)
            {
                instance = new UserData();
            }

            return instance;
        }

        public string[] Users
        {
            get
            {
                ICollection collectionUsers = mapUserToData.Keys;
                var asUsers = new string[collectionUsers.Count];

                int nIndex = 0;

                foreach (string sUser in collectionUsers)
                {
                    asUsers[nIndex] = sUser;
                    nIndex++;
                }

                return asUsers;
            }
        }

        public int UserCount
        {
            get
            {
                return mapUserToData.Count;
            }
        }

        public void AddUser(string sUser)
        {
            mapUserToData.Add(sUser, new UserDataItem());
        }

        public void RemoveUser(string sUser)
        {
            mapUserToData.Remove(sUser);
        }

        public string GetUserPassword(string sUser)
        {
            UserDataItem item = GetUserItem(sUser);

            if (item != null)
            {
                return item.Password;
            }

            return string.Empty;
        }

        public void SetUserPassword(string sUser, string sPassword)
        {
            UserDataItem item = GetUserItem(sUser);

            if (item != null)
            {
                item.Password = sPassword;
            }
        }

        public string GetUserStartingDirectory(string sUser)
        {
            UserDataItem item = GetUserItem(sUser);

            if (item != null)
            {
                return item.StartingDirectory;
            }

            return "C:\\";
        }

        public void SetUserStartingDirectory(string sUser, string sDirectory)
        {
            UserDataItem item = GetUserItem(sUser);

            if (item != null)
            {
                item.StartingDirectory = sDirectory;
            }
        }

        public bool HasUser(string sUser)
        {
            UserDataItem item = GetUserItem(sUser);
            return item != null;
        }

        public bool Save(string sFileName)
        {
            try
            {
                var formatter = new BinaryFormatter();
                var fileStream = new FileStream(sFileName, FileMode.Create);
                formatter.Serialize(fileStream, mapUserToData);
                fileStream.Close();
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return true;
            }

            try
            {
                var formatter = new BinaryFormatter();
                var fileStream = new FileStream(fileName, FileMode.Open);
                mapUserToData = formatter.Deserialize(fileStream) as Hashtable;
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool Save()
        {
            return Save(GetDefaultPath());
        }

        public bool Load()
        {
            return Load(GetDefaultPath());
        }

        private UserDataItem GetUserItem(string sUser)
        {
            return mapUserToData[sUser] as UserDataItem;
        }

        private string GetDefaultPath()
        {
            return Path.Combine(Application.StartupPath, "Users.dat");
        }
    }
}