﻿//-----------------------------------------------------------------------------
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

    /// <summary>Provides help methods for configuration code in the MUD engine.</summary>
    public class Configuration
    {
        /// <summary>Gets the data storage path for WheelMUD files.</summary>
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
        
        private static string GetMudName()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "mud.config");
            //var config = new DotNetConfigSource(path);
            //string mudName = config.Configs["EngineAttributes"].GetString("name");
            //return mudName;
            // @@@ TODO: USE CONSISTENT CONFIG SOURCES; ELIMINATE mud.config AND USE JUST App.config?
            return "HardCodedWheelMUD";
        }
    }
}
