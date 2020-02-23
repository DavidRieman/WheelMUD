//-----------------------------------------------------------------------------
// <copyright file="MudEngineAttributes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    using System.IO;
    using System.Reflection;

    /// <summary>MUD Engine Attributes.</summary>
    public class MudEngineAttributes
    {
        /// <summary>Prevents a default instance of the <see cref="MudEngineAttributes"/> class from being created.</summary>
        private MudEngineAttributes()
        {
            this.GetConfigSettings();
        }

        /// <summary>Gets the singleton instance of the <see cref="MudEngineAttributes"/> class.</summary>
        public static MudEngineAttributes Instance { get; } = new MudEngineAttributes();

        /// <summary>Gets the current version of the MUD.</summary>
        /// <remarks>Potentially rendered in splash screens. Generates from assembly information (GlobalAssemblyInfo.cs if you don't have your own).</remarks>
        public string Version { get; private set; }

        /// <summary>Gets or sets the default room ID.</summary>
        /// <value>The default room ID.</value>
        public int DefaultRoomID { get; set; }

        /// <summary>Gets or sets the name of the MUD.</summary>
        /// <remarks>Potentially rendered in splash screens. Configure via App.config.</remarks>
        public string MudName { get; private set; }

        /// <summary>Gets or sets the telnet port.</summary>
        public int TelnetPort { get; set; }

        /// <summary>Gets or sets the copyright for this MUD.</summary>
        /// <remarks>Potentially rendered in splash screens. Configure via assembly information (GlobalAssemblyInfo.cs if you don't have your own).</remarks>
        public string Copyright { get; private set; }

        /// <summary>Gets or sets the website for this MUD.</summary>
        /// <remarks>Potentially rendered in splash screens. Configure via App.config.</remarks>
        public string Website { get; private set; }

        /// <summary>Gets or sets the root directory for the FTP server.</summary>
        public string FTPServerRootFolder { get; set; }

        /// <summary>Gets or sets the master rule set file.</summary>
        /// <value>The master rule set file.</value>
        public string MasterRuleSetFile { get; set; }

        /// <summary>Gets or sets the name of the current rule set.</summary>
        /// <value>The name of the current rule set.</value>
        public string CurrentRuleSetName { get; set; }

        /// <summary>Gets the configuration settings.</summary>
        private void GetConfigSettings()
        {
            /* @@@ TODO: REPAIR - VIA APP.CONFIG INSTEAD OF MUD.CONFIG?
            this.MudName = this.config.Configs["EngineAttributes"].GetString("name");
            this.TelnetPort = this.config.Configs["EngineAttributes"].GetInt("port");
            this.Website = this.config.Configs["EngineAttributes"].GetString("website");
            this.defaultRoomID = this.config.Configs["UniverseAttributes"].GetInt("defaultroom");
            this.MasterRuleSetFile = this.config.Configs["RuleSetAttributes"].GetString("masterfile");
            this.CurrentRuleSetName = this.config.Configs["RuleSetAttributes"].GetString("currentruleset");
            this.RoomFormatingTemplateFile = this.config.Configs["Templates"].GetString("roomviewtemplate");
            this.EntityFormatingTemplateFile = this.config.Configs["Templates"].GetString("entityviewtemplate");
            this.HelpTopicFormatingTemplateFile = this.config.Configs["Templates"].GetString("helpviewtemplate");
            this.version = this.GetType().Assembly.GetName().Version.ToString();
            string runDir = Configuration.GetDataStoragePath();
            this.FTPServerRootFolder = runDir;
            ////this.config.Configs["FTP"].GetString("RootFolder").Replace("%FTPRUNDIR%", runDir);
            */

            this.MudName = "WheelMUD";
            this.TelnetPort = 4000; 
            this.Website = "www.wheelmud.net";
            this.DefaultRoomID = 1;

            // TODO: Discover active rules and stuff through Composition; Admin simply includes libraries they want active, with Priority export settings.
            this.MasterRuleSetFile = "WarriorRogueMageMaster.xml";
            this.CurrentRuleSetName = "Warrior, Rogue, and Mage";

            // Generally we want to pull information from the executing assembly (rather than core assemblies), so that a
            // custom game solution/harness can apply their own assembly information.
            var assembly = Assembly.GetExecutingAssembly();
            this.Version = assembly.GetName().Version.ToString();
            this.Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        }
    }
}