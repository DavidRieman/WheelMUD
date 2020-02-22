//-----------------------------------------------------------------------------
// <copyright file="UserData.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Ftp
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class UserData
    {
        private static readonly UserData SingletonInstance = new UserData();

        private Hashtable mapUserToData = null;

        /// <summary>Initializes a new instance of the <see cref="UserData"/> class.</summary>
        protected UserData()
        {
            this.mapUserToData = new Hashtable();
        }

        public static UserData Instance
        {
            get { return SingletonInstance; }
        }

        public string[] Users
        {
            get
            {
                ICollection collectionUsers = this.mapUserToData.Keys;
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
                return this.mapUserToData.Count;
            }
        }

        public void AddUser(string user)
        {
            this.mapUserToData.Add(user, new UserDataItem());
        }

        public void RemoveUser(string user)
        {
            this.mapUserToData.Remove(user);
        }

        public string GetUserPassword(string user)
        {
            UserDataItem item = this.GetUserItem(user);
            if (item != null)
            {
                return item.Password;
            }

            return string.Empty;
        }

        public void SetUserPassword(string user, string password)
        {
            UserDataItem item = this.GetUserItem(user);
            if (item != null)
            {
                item.Password = password;
            }
        }

        public string GetUserStartingDirectory(string user)
        {
            UserDataItem item = this.GetUserItem(user);
            if (item != null)
            {
                return item.StartingDirectory;
            }

            return "C:\\";
        }

        public void SetUserStartingDirectory(string user, string directory)
        {
            UserDataItem item = this.GetUserItem(user);
            if (item != null)
            {
                item.StartingDirectory = directory;
            }
        }

        public bool HasUser(string user)
        {
            UserDataItem item = this.GetUserItem(user);
            return item != null;
        }

        public bool Save(string fileName)
        {
            try
            {
                var formatter = new BinaryFormatter();
                var fileStream = new FileStream(fileName, FileMode.Create);
                formatter.Serialize(fileStream, this.mapUserToData);
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
                this.mapUserToData = formatter.Deserialize(fileStream) as Hashtable;
            }
            catch (IOException)
            {
                return false;
            }

            return true;
        }

        public bool Save()
        {
            return this.Save(this.GetDefaultPath());
        }

        public bool Load()
        {
            return this.Load(this.GetDefaultPath());
        }

        private UserDataItem GetUserItem(string user)
        {
            return this.mapUserToData[user] as UserDataItem;
        }

        private string GetDefaultPath()
        {
            // @@@ TODO: Study usage and repair using proper temp data storage area, rather than relying
            //     on System.Windows.Forms for: Path.Combine(Application.StartupPath, "Users.dat");
            return string.Empty;
        }

        [Serializable]
        private class UserDataItem
        {
            private string password = string.Empty;
            private string startingDirectory = "C:\\";

            public string Password
            {
                get { return this.password; }
                set { this.password = value; }
            }

            public string StartingDirectory
            {
                get { return this.startingDirectory; }
                set { this.startingDirectory = value; }
            }
        }
    }
}