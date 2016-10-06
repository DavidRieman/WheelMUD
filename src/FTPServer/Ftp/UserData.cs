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

    public class UserData
    {
        private static readonly UserData SingletonInstance = new UserData();

        private Hashtable mapUserToData = null;

        /// <summary>Initializes a new instance of the <see cref="UserData"/> class.</summary>
        protected UserData()
        {
            mapUserToData = new Hashtable();
        }

        public static UserData Instance
        {
            get { return SingletonInstance; }
        }

        public string[] Users
        {
            get
            {
                ICollection collectionUsers = mapUserToData.Keys;
                var asUsers = new string[collectionUsers.Count];
                int index = 0;
                foreach (string user in collectionUsers)
                {
                    asUsers[index] = user;
                    index++;
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

        public void AddUser(string user)
        {
            mapUserToData.Add(user, new UserDataItem());
        }

        public void RemoveUser(string user)
        {
            mapUserToData.Remove(user);
        }

        public string GetUserPassword(string user)
        {
            UserDataItem item = GetUserItem(user);
            if (item != null)
            {
                return item.Password;
            }

            return string.Empty;
        }

        public void SetUserPassword(string user, string password)
        {
            UserDataItem item = GetUserItem(user);
            if (item != null)
            {
                item.Password = password;
            }
        }

        public string GetUserStartingDirectory(string user)
        {
            UserDataItem item = GetUserItem(user);
            if (item != null)
            {
                return item.StartingDirectory;
            }

            return "C:\\";
        }

        public void SetUserStartingDirectory(string user, string directory)
        {
            UserDataItem item = GetUserItem(user);
            if (item != null)
            {
                item.StartingDirectory = directory;
            }
        }

        public bool HasUser(string user)
        {
            UserDataItem item = GetUserItem(user);
            return item != null;
        }

        public bool Save(string fileName)
        {
            try
            {
                var formatter = new BinaryFormatter();
                var fileStream = new FileStream(fileName, FileMode.Create);
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

        private UserDataItem GetUserItem(string user)
        {
            return mapUserToData[user] as UserDataItem;
        }

        private string GetDefaultPath()
        {
            return Path.Combine(Application.StartupPath, "Users.dat");
        }

        [Serializable]
        private class UserDataItem
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
    }
}