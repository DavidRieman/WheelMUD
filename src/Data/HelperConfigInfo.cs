//-----------------------------------------------------------------------------
// <copyright file="HelperConfigInfo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Data
{
    using System.Configuration;
    using System.IO;
    using WheelMUD.Utilities;

    /// <summary>Class to simplify reading application and data configuration information.</summary>
    public class AppConfigInfo
    {
        /// <summary>Prevents a default instance of the <see cref="AppConfigInfo"/> class from being created.</summary>
        private AppConfigInfo()
        {
            GetConfigSettings();
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
            UserAccountIsPlayerCharacter = GameConfiguration.GetAppConfigBool("UserAccountIsPlayerCharacter");
            PlayerCharacterNamesMustUseSingleCapital = GameConfiguration.GetAppConfigBool("PlayerCharacterNamesMustUseSingleCapital");

            var relationalSettings = GetConnectionStringSettings("RelationalDataProviderName", "WheelMUDSQLite");
            var documentSettings = GetConnectionStringSettings("DocumentDataProviderName", "RavenDB");
            RelationalDataProviderName = relationalSettings.Name;
            RelationalConnectionString = relationalSettings.ConnectionString;
            DocumentDataProviderName = documentSettings.Name;
            DocumentConnectionString = documentSettings.ConnectionString;

            // Replace any tokens like {DataDir} in the connection strings with evaluated values.
            // This prevents new administrators from having to adjust App.config for user-specific paths.
            var dataDir = GameConfiguration.DataStoragePath + Path.DirectorySeparatorChar;
            RelationalConnectionString = RelationalConnectionString.Replace("{DataDir}", dataDir);
            DocumentConnectionString = DocumentConnectionString.Replace("{DataDir}", dataDir);
        }
    }
}