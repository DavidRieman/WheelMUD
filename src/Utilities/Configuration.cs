//-----------------------------------------------------------------------------
// <copyright file="Configuration.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Provides help methods for configuration code in the MUD engine.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    using System;
    using System.IO; 
    using System.Reflection;

    using Nini.Config;

    /// <summary>
    /// Provides help methods for configuration code in the MUD engine.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets the data storage path for WheelMUD files.
        /// </summary>
        /// <returns>A string containing the root data directory for WheelMUD files.</returns>
        public static string GetDataStoragePath()
        {
            string root = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string rootPath = Path.Combine(root, "WheelMUD");
            string mudName = GetMudName();
            string mudPath = Path.Combine(rootPath, mudName);
            string fullPath = Path.Combine(mudPath, "Files");

            return fullPath;
        }

        /// <summary>
        /// Gets the connection string exe path.
        /// </summary>
        /// <returns>A string containing the path to the connectionstrings.exe file.</returns>
        public static string GetConnectionStringExePath()
        {
            string root = GetDataStoragePath();

            return Path.Combine(root, "connectionstrings.exe");
        }

        /// <summary>
        /// Gets the connection string file path.
        /// </summary>
        /// <returns>A string containing the path to the connectionstrings.exe.config file.</returns>
        public static string GetConnectionStringFilePath()
        {
            string root = GetDataStoragePath();

            return Path.Combine(root, "connectionstrings.config");
        }

        /// <summary>
        /// Gets the connection string configuration file path.
        /// </summary>
        /// <returns>The path to the configuration file that contains the connection strings.</returns>
        public static string GetConnectionStringConfigFilePath()
        {
            string root = GetDataStoragePath();

            return Path.Combine(root, "connectionstrings.exe.config");
        }

        private static string GetMudName()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "mud.config");
            var config = new DotNetConfigSource(path);

            string mudName = config.Configs["EngineAttributes"].GetString("name");

            return mudName;
        }
    }
}
