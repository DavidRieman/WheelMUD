//-----------------------------------------------------------------------------
// <copyright file="GameConfiguration.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;

    /// <summary>Provides simplified, shared access to configuration properties for the game engine.</summary>
    /// <remarks>
    /// Common properties expected to be useful for most or all MUD implementations can be exposed with static property
    /// getters here.  Additional and custom configuration properties can also be placed in the App.config file, and
    /// then accessed by name through GetAppConfigBool / GetAppConfigString. 
    /// </remarks>
    public static class GameConfiguration
    {
        static GameConfiguration()
        {
            Initialize();
        }

        private static void Initialize()
        {
            // Many of the configuration properties are read once and cached by ConfigurationManager, but since we
            // may some day want to respect re-initializing upon file change or upon command similar to updating
            // other systems on the fly, we will start by ensuring the section has been refreshed. TODO: Re-inits?
            ConfigurationManager.RefreshSection("appSettings");

            // Properties that are read extremely often may wish to cache their values, to reduce redundant
            // function calls and dereferencing operations that we know will result in an unchanging value.
            DefaultRoomID = GetAppConfigString("DefaultRoomID");
            Name = GetAppConfigString("GameName");
            TelnetPort = GetAppConfigInt("TelnetPort");

            // Generally we want to pull information from the executing assembly (rather than core assemblies),
            // so that a custom game solution/harness can apply their own assembly information.
            var assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetName().Version.ToString();
            Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;

            // Additional work to modify raw settings should generally be done once and cached, as follows.
            string root = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string rootPath = Path.Combine(root, "WheelMUD");
            string gamePath = Path.Combine(rootPath, GameConfiguration.Name);
            string fullPath = Path.Combine(gamePath, "Files");
            DataStoragePath = fullPath;
        }

        /// <summary>Gets or sets the copyright for this MUD.</summary>
        /// <remarks>Potentially rendered in splash screens. Configure via assembly information (GlobalAssemblyInfo.cs if you don't have your own).</remarks>
        public static string Copyright { get; private set; }

        /// <summary>Gets the data storage path for WheelMUD files.</summary>
        /// <returns>A string containing the root data directory for WheelMUD files.</returns>
        public static string DataStoragePath { get; private set; }

        /// <summary>Gets the default room ID, where players spawn in when new or unable to restore a saved player location.</summary>
        /// <value>The default room ID.</value>
        public static string DefaultRoomID { get; private set; }

        /// <summary>Gets the Game Name as configured through the active application's App.config file.</summary>
        public static string Name { get; private set; }

        /// <summary>Gets the port to receive incoming telnet connections on. Configured via App.config.</summary>
        public static int TelnetPort { get; private set; }

        /// <summary>Gets the Game Version number as established through the active application's assembly info.</summary>
        /// <remarks>
        /// This can be seen in the admin console, and displayed through splash screen templates if you wish to make it
        /// easy to reconcile base versioning between servers (such as test, staging, and production servers). However,
        /// beware that it might not update with non-rebooting drop-in updates.
        /// </remarks>
        public static string Version { get; private set; }

        /// <summary>Gets the website for this game.</summary>
        /// <remarks>Potentially rendered in splash screens. Configure via App.config.</remarks>
        public static string Website { get; private set; }

        public static bool GetAppConfigBool(string appSettingsName)
        {
            return bool.Parse(ConfigurationManager.AppSettings[appSettingsName]);
        }

        public static int GetAppConfigInt(string appSettingsName)
        {
            return int.Parse(ConfigurationManager.AppSettings[appSettingsName]);
        }

        public static string GetAppConfigString(string appSettingsName)
        {
            return ConfigurationManager.AppSettings[appSettingsName];
        }
    }
}
