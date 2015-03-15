//-----------------------------------------------------------------------------
// <copyright file="MudEngineAttributes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//  Class to read connection string configuration info for the NHibernate Session
//  class.   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    using System.Configuration;
    using System.IO;

    using Configuration = WheelMUD.Utilities.Configuration;

    /// <summary>
    /// Class to read connection string configuration info for the NHibernate Session
    /// in the Helpers.cs class.
    /// </summary>
    public class HelperConfigInfo
    {
        /// <summary>The synchronization locking object.</summary>
        private static readonly object lockObject = new object();

        /// <summary>The HelperConfigInfo singleton instance.</summary>
        private static HelperConfigInfo instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="HelperConfigInfo"/> class from being created. 
        /// </summary>
        private HelperConfigInfo()
        {
            this.GetConfigSettings();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static HelperConfigInfo Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new HelperConfigInfo();
                    }

                    return instance;
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the connection string.
        /// </summary>
        /// <value>The name of the connection string.</value>
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the database provider.
        /// </summary>
        /// <value>The database provider.</value>
        public string Provider { get; set; }

        /// <summary>
        /// Gets the configuration settings.
        /// </summary>
        private void GetConfigSettings()
        {
            string connectionStringExe = Configuration.GetConnectionStringExePath();
            string configFile = Configuration.GetConnectionStringConfigFilePath();
            string defaultName = ConfigurationManager.AppSettings["DefaultConnectionStringName"];

            if (defaultName == null)
            {
                defaultName = "WheelMUDSQLite";
            }

            if (!File.Exists(connectionStringExe))
            {
                this.CreateDummyExe(connectionStringExe);
            }

            if (!File.Exists(configFile))
            {
                this.CreateConfigFile();
            }

            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(connectionStringExe);

            this.ConnectionStringName = config.ConnectionStrings.ConnectionStrings[defaultName].Name;
            this.ConnectionString = config.ConnectionStrings.ConnectionStrings[defaultName].ConnectionString;
            this.Provider = config.ConnectionStrings.ConnectionStrings[defaultName].ProviderName;

            if (this.ConnectionString.Contains("Files\\WheelMud.net.db"))
            {
                string path = Path.GetDirectoryName(connectionStringExe);
                string fullPath = Path.Combine(path, "WheelMud.net.db");

                this.ConnectionString = this.ConnectionString.Replace("Files\\WheelMud.net.db", fullPath);
            }
        }

        private void CreateDummyExe(string path)
        {
            File.Create(path);
        }

        private void CreateConfigFile()
        {
            string file = Configuration.GetConnectionStringFilePath();

            StreamReader stream = File.OpenText(file);

            string contents = stream.ReadToEnd();
            stream.Close();

            StreamWriter writer = File.CreateText(Configuration.GetConnectionStringConfigFilePath());
            writer.WriteLine("<configuration>");
            writer.WriteLine(contents);
            writer.WriteLine("</configuration>");
            writer.Flush();
            writer.Close();
        }
    }
}
