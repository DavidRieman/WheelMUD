//-----------------------------------------------------------------------------
// <copyright file="MudEngineAttributes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Utilities
{
    using System;
    using System.IO;
    using System.Reflection;
    using Nini.Config;

    /// <summary>MUD Engine Attributes.</summary>
    public class MudEngineAttributes
    {
        /// <summary>The MudEngineAttributes singleton instance.</summary>
        private static readonly MudEngineAttributes SingletonInstance = new MudEngineAttributes();

        /// <summary>The .NET configuration source.</summary>
        private readonly DotNetConfigSource config;

        /// <summary>The version of the engine.</summary>
        private string version;

        /// <summary>The default room ID.</summary>
        private int defaultRoomID;

        /// <summary>Prevents a default instance of the <see cref="MudEngineAttributes"/> class from being created.</summary>
        private MudEngineAttributes()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "mud.config");
            this.config = new DotNetConfigSource(path);
            this.GetConfigSettings();
        }

        /// <summary>Gets the singleton instance of the <see cref="MudEngineAttributes"/> class.</summary>
        public static MudEngineAttributes Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Gets the current version of the MUD.</summary>
        public string Version
        {
            get { return this.version; }
        }

        /// <summary>Gets or sets the default room ID.</summary>
        /// <value>The default room ID.</value>
        public int DefaultRoomID
        {
            get { return this.defaultRoomID; }

            set { this.defaultRoomID = value; }
        }

        /// <summary>Gets or sets the name of the MUD.</summary>
        public string MudName { get; set; }

        /// <summary>Gets or sets the telnet port.</summary>
        public int TelnetPort { get; set; }

        /// <summary>Gets or sets the website for this MUD.</summary>
        public string Website { get; set; }

        /// <summary>Gets or sets the template file location that will be used to format the help text.</summary>
        public string HelpTopicFormatingTemplateFile { get; set; }

        /// <summary>Gets or sets the template file location that will be used to format the room text.</summary>
        public string RoomFormatingTemplateFile { get; set; }

        /// <summary>Gets or sets the template file location that will be used to format the entity's (NPC, Mobs) text.</summary>
        public string EntityFormatingTemplateFile { get; set; }

        /// <summary>Gets or sets the root directory for the FTP server.</summary>
        public string FTPServerRootFolder { get; set; }

        /// <summary>Gets or sets the master rule set file.</summary>
        /// <value>The master rule set file.</value>
        public string MasterRuleSetFile { get; set; }

        /// <summary>Gets or sets the name of the current rule set.</summary>
        /// <value>The name of the current rule set.</value>
        public string CurrentRuleSetName { get; set; }

        /// <summary>Saves this instance.</summary>
        /// <returns>Whether the Save method was successful or not.</returns>
        public bool Save()
        {
            try
            {
                this.config.Configs["EngineAttributes"].Set("name", this.MudName);
                this.config.Configs["EngineAttributes"].Set("port", this.TelnetPort);
                this.config.Configs["EngineAttributes"].Set("website", this.Website);
                this.config.Configs["UniverseAttributes"].Set("defaultroom", this.DefaultRoomID);
                this.config.Configs["RuleSetAttributes"].Set("masterfile", this.MasterRuleSetFile);
                this.config.Configs["RuleSetAttributes"].Set("currentruleset", this.CurrentRuleSetName);
                this.config.Configs["Templates"].Set("roomviewtemplate", this.RoomFormatingTemplateFile);
                this.config.Configs["Templates"].Set("entityviewtemplate", this.EntityFormatingTemplateFile);
                this.config.Configs["Templates"].Set("helpviewtemplate", this.HelpTopicFormatingTemplateFile);
                this.config.Configs["FTP"].Set("RootFolder", this.FTPServerRootFolder);

                this.config.Save();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("MudEngineAttributes.Save()\n{0}", e.Message));
            }

            return false;
        }

        /// <summary>Gets the configuration settings.</summary>
        private void GetConfigSettings()
        {
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
        }
    }
}