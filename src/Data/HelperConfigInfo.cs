//-----------------------------------------------------------------------------
// <copyright file="HelperConfigInfo.cs" company="WheelMUD Development Team">
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

    /// <summary>Class to read connection string configuration info for the NHibernate Session in the Helpers.cs class.</summary>
    public class HelperConfigInfo
    {
        /// <summary>The HelperConfigInfo singleton instance.</summary>
        private static readonly HelperConfigInfo SingletonInstance = new HelperConfigInfo();

        /// <summary>Prevents a default instance of the <see cref="HelperConfigInfo"/> class from being created.</summary>
        private HelperConfigInfo()
        {
            this.GetConfigSettings();
        }

        /// <summary>Gets the singleton instance of the <see cref="HelperConfigInfo"/> class.</summary>
        public static HelperConfigInfo Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Gets or sets the name of the connection string.</summary>
        /// <value>The name of the connection string.</value>
        public string ConnectionStringName { get; set; }

        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }

        /// <summary>Gets or sets the database provider.</summary>
        /// <value>The database provider.</value>
        public string Provider { get; set; }

        /// <summary>Gets the configuration settings.</summary>
        private void GetConfigSettings()
        {
            string configFile = Configuration.GetConnectionStringConfigFilePath();
            string defaultName = ConfigurationManager.AppSettings["DefaultConnectionStringName"];

            if (defaultName == null)
            {
                defaultName = "WheelMUDSQLite";
            }

            if (!File.Exists(configFile))
            {
                this.CreateConfigFile();
            }

            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
            configMap.ExeConfigFilename = configFile;
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            this.ConnectionStringName = config.ConnectionStrings.ConnectionStrings[defaultName].Name;
            this.ConnectionString = config.ConnectionStrings.ConnectionStrings[defaultName].ConnectionString;
            this.Provider = config.ConnectionStrings.ConnectionStrings[defaultName].ProviderName;

            if (this.ConnectionString.Contains("Files\\WheelMud.net.db"))
            {
                string path = Path.GetDirectoryName(configFile);
                string fullPath = Path.Combine(path, "WheelMud.net.db");

                this.ConnectionString = this.ConnectionString.Replace("Files\\WheelMud.net.db", fullPath);
            }
        }

        /// <summary>Creates the connection configuration file.</summary>
        private void CreateConfigFile()
        {
            string file = Configuration.GetConnectionStringConfigFilePath();

            var dirPath = Path.GetDirectoryName(file);
            Directory.CreateDirectory(dirPath);

            using (var writer = File.CreateText(Configuration.GetConnectionStringConfigFilePath()))
            {
                writer.WriteLine("<configuration>");
                writer.WriteLine("<connectionStrings>");
                writer.WriteLine("  <clear/>");
                writer.WriteLine("  <add name=\"WheelMUDSQLite\" providerName=\"System.Data.SQLite\" connectionString=\"Data Source = Files\\WheelMud.net.db; Version = 3; \"/>");
                writer.WriteLine("</connectionStrings>");
                writer.WriteLine("</configuration>");
                writer.Flush();
            }
        }
    }
}