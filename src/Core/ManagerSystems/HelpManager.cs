//-----------------------------------------------------------------------------
// <copyright file="HelpManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   High level manager that provides tracking and global collection of all items.
//   Created: December 2009 by bengecko.
//   Modified by: Pure October 2010
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using WheelMUD.Interfaces;

    /// <summary>High level manager that provides maintains help information.</summary>
    public class HelpManager : ManagerSystem
    {
        /// <summary>The singleton instance of this class.</summary>
        private static readonly HelpManager SingletonInstance = new HelpManager();

        /// <summary>Prevents a default instance of the <see cref="HelpManager"/> class from being created.</summary>
        private HelpManager()
        {
            this.HelpTopics = new List<HelpTopic>();
        }

        /// <summary>Gets the singleton instance of HelpManager.</summary>
        /// <value>The HelpManager instance.</value>
        public static HelpManager Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Gets the complete list of HelpTopics that are loaded into memory.</summary>
        public List<HelpTopic> HelpTopics { get; private set; }

        /// <summary>Starts the manager and loads all help records into memory</summary>
        public override void Start()
        {
            this.SystemHost.UpdateSystemHost(this, "Starting...");

            string dataRoot = Utilities.Configuration.GetDataStoragePath();
            string helpDir = Path.Combine(dataRoot, "Help");

            if (!Directory.Exists(helpDir))
            {
                Directory.CreateDirectory(helpDir);
            }

            foreach (string file in Directory.GetFiles(helpDir))
            {
                string contents = File.ReadAllText(file);
                string[] fileParts = file.Split(Path.DirectorySeparatorChar);
                string[] fileAliases = fileParts[fileParts.Length - 1].Split(',');
                var helpTopic = new HelpTopic(contents, new List<string>(fileAliases));
                this.HelpTopics.Add(helpTopic);
            }

            this.SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops the manager and unloads all help records from memory.</summary>
        public override void Stop()
        {
            this.SystemHost.UpdateSystemHost(this, "Stopping...");

            lock (this.lockObject)
            {
                this.HelpTopics.Clear();
            }

            this.SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Find a HelpTopic via the help topic alias.</summary>
        /// <param name="helpTopic">The help topic alias to look up.</param>
        /// <returns>The help topic, if found.</returns>
        public HelpTopic FindHelpTopic(string helpTopic)
        {
            Predicate<HelpTopic> finder = (HelpTopic h) => { return h.Aliases.Contains(helpTopic); };
            return this.HelpTopics.Find(finder);
        }

        /// <summary>Registers the <see cref="HelpManager"/> system for export.</summary>
        /// <remarks>Assists with non-rebooting updates of the <see cref="HelpManager"/> system through MEF.</remarks>
        [ExportSystem]
        public class HelpManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            public override ISystem Instance
            {
                get { return HelpManager.Instance; }
            }

            /// <summary>Gets the Type of this system.</summary>
            public override Type SystemType
            {
                get { return typeof(HelpManager); }
            }
        }
    }
}