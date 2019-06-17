//-----------------------------------------------------------------------------
// <copyright file="HelperConfigInfo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//  Class to simplify reading application and data configuration information.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    using System.Configuration;
    using System.IO;
    using Configuration = WheelMUD.Utilities.Configuration;

    /// <summary>Class to read connection string configuration info for the NHibernate Session in the Helpers.cs class.</summary>
    public class AppConfigInfo
    {
        /// <summary>Prevents a default instance of the <see cref="AppConfigInfo"/> class from being created.</summary>
        private AppConfigInfo()
        {
            this.GetConfigSettings();
        }

        /// <summary>Gets the singleton instance of the <see cref="AppConfigInfo"/> class.</summary>
        public static AppConfigInfo Instance { get; } = new AppConfigInfo();

        /// <summary>Gets or sets the name of the connection string.</summary>
        /// <value>The name of the connection string.</value>
        public string ConnectionStringName { get; private set; }

        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
        public string RelationalConnectionString { get; private set; }
        public string DocumentConnectionString { get; private set; }

        /// <summary>Gets or sets the name of the relational database provider.</summary>
        /// <value>The relational database provider name.</value>
        public string RelationalDataProviderName { get; private set; }

        /// <summary>Gets or sets the name of the document storage provider.</summary>
        /// <value>The document storage provider name.</value>
        public string DocumentDataProviderName { get; private set; }

        public bool UserAccountIsPlayerCharacter { get; private set; }
        public bool PlayerCharacterNamesMustUseSingleCapital { get; private set; }

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
            this.UserAccountIsPlayerCharacter = this.GetBool("UserAccountIsPlayerCharacter");
            this.PlayerCharacterNamesMustUseSingleCapital = this.GetBool("PlayerCharacterNamesMustUseSingleCapital");

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

        private bool GetBool(string appSettingsName)
        {
            return bool.Parse(ConfigurationManager.AppSettings[appSettingsName]);
        }
    }
}