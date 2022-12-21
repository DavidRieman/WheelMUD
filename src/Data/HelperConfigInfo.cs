//-----------------------------------------------------------------------------
// <copyright file="HelperConfigInfo.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Configuration;
using System.IO;
using WheelMUD.Utilities;

namespace WheelMUD.Data
{
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

        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
        public string DocumentConnectionString { get; private set; }

        /// <summary>Gets or sets the name of the document storage provider.</summary>
        /// <value>The document storage provider name.</value>
        public string DocumentDataProviderName { get; private set; }

        public bool UserAccountIsPlayerCharacter { get; private set; }
        public bool PlayerCharacterNamesMustUseSingleCapital { get; private set; }

        /// <summary>If supplied, this command will be executed on behalf of freshly connected players.</summary>
        /// <remarks>TODO: Consider whether this or a similar value should exist specifically for reconnected players who were still in the world.</remarks>
        public string AutomaticLoginCommand { get; private set; }

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

            var documentSettings = GetConnectionStringSettings("DocumentDataProviderName", "RavenDB");
            DocumentDataProviderName = documentSettings.Name;
            DocumentConnectionString = documentSettings.ConnectionString;

            // Replace any tokens like {DataDir} in the connection strings with evaluated values.
            // This prevents new administrators from having to adjust App.config for user-specific paths.
            var dataDir = GameConfiguration.DataStoragePath + Path.DirectorySeparatorChar;
            DocumentConnectionString = DocumentConnectionString.Replace("{DataDir}", dataDir);
        }
    }
}