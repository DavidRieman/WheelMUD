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
        public string RelationalConnectionString { get; set; }
        public string DocumentConnectionString { get; set; }

        /// <summary>Gets or sets the name of the relational database provider.</summary>
        /// <value>The relational database provider name.</value>
        public string RelationalDataProviderName { get; set; }

        /// <summary>Gets or sets the name of the document storage provider.</summary>
        /// <value>The document storage provider name.</value>
        public string DocumentDataProviderName { get; set; }

        private ConnectionStringSettings GetConnectionStringSettings(string appSettingsName, string defaultProviderName)
        {
            var providerName = ConfigurationManager.AppSettings[appSettingsName];
            if (providerName == null)
            {
                providerName = defaultProviderName;
            }
            return ConfigurationManager.ConnectionStrings[providerName];
        }

        /// <summary>Gets the configuration settings.</summary>
        private void GetConfigSettings()
        {
            var relationalSettings = this.GetConnectionStringSettings("RelationalDataProviderName", "WheelMUDSQLite");
            var documentSettings = this.GetConnectionStringSettings("DocumentDataProviderName", "RavenDB");
            this.RelationalDataProviderName = relationalSettings.Name;
            this.RelationalConnectionString = relationalSettings.ConnectionString;
            this.DocumentDataProviderName = documentSettings.Name;
            this.DocumentConnectionString = documentSettings.ConnectionString;

            // Replace any tokens like {DataDir} in the connection strings with evaluated values.
            // This prevents new administrators from having to adjust App.config for user-specific paths.
            var dataDir = Configuration.GetDataStoragePath() + Path.DirectorySeparatorChar;
            this.RelationalConnectionString = this.RelationalConnectionString.Replace("{DataDir}", dataDir);
            this.DocumentConnectionString = this.DocumentConnectionString.Replace("{DataDir}", dataDir);
        }
    }
}