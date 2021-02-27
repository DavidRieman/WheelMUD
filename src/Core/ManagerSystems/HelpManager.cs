//-----------------------------------------------------------------------------
// <copyright file="HelpManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WheelMUD.Core.Interfaces;
using WheelMUD.Server.Interfaces;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    /// <summary>High level manager that provides maintains help information.</summary>
    public class HelpManager : ManagerSystem
    {
        /// <summary>Prevents a default instance of the <see cref="HelpManager"/> class from being created.</summary>
        private HelpManager()
        {
            HelpTopics = new List<HelpTopic>();
        }

        /// <summary>Gets the singleton instance of HelpManager.</summary>
        /// <value>The HelpManager instance.</value>
        public static HelpManager Instance { get; } = new HelpManager();

        /// <summary>Gets the complete list of HelpTopics that are loaded into memory.</summary>
        public List<HelpTopic> HelpTopics { get; private set; }

        /// <summary>Starts the manager and loads all help records into memory</summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");

            string dataRoot = GameConfiguration.DataStoragePath;
            string helpDir = Path.Combine(dataRoot, "Help");

            if (!Directory.Exists(helpDir))
            {
                Directory.CreateDirectory(helpDir);
            }

            foreach (string file in Directory.GetFiles(helpDir))
            {
                // We need to read in each line of the file and put it back together explicitly with AnsiSequences.NewLines,
                // so that we are prepared to send the contents with the expected NewLine of the Telnet protocal, regardless
                // of what line endings the help file was saved with.
                var contentLines = File.ReadAllLines(file);
                string contents = string.Join(AnsiSequences.NewLine, contentLines);

                // The file name depicts the help topic. Something like "foo_bar" would be a single keyword with an underscore
                // as part of the help topic alias, while "foo__bar" depicts two aliases ("foo" and "bar").
                string nameOnly = Path.GetFileNameWithoutExtension(file);
                string[] aliases = nameOnly.Split("__");
                var helpTopic = new HelpTopic(contents, new List<string>(aliases));
                HelpTopics.Add(helpTopic);
            }

            SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops the manager and unloads all help records from memory.</summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");

            lock (lockObject)
            {
                HelpTopics.Clear();
            }

            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Find a HelpTopic via the help topic alias.</summary>
        /// <param name="helpTopic">The help topic alias to look up.</param>
        /// <returns>The help topic, if found.</returns>
        public HelpTopic FindHelpTopic(string helpTopic)
        {
            // Find any exact match for the topic. (If we too aggressively included results for partial matches, we
            // might lose the ability to fall back to checking for help on commands or other dynamic help info.)
            // However, a help file can use multiple aliases by separating them with "__" in the help file name, with
            // the "primary" alias (one we would display to the user when listing topics) being the first one.
            return HelpTopics.Find(h => h.Aliases.Contains(helpTopic, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>Registers the <see cref="HelpManager"/> system for export.</summary>
        /// <remarks>Assists with non-rebooting updates of the <see cref="HelpManager"/> system through MEF.</remarks>
        [ExportSystem(0)]
        public class HelpManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            public override ISystem Instance => HelpManager.Instance;

            /// <summary>Gets the Type of this system.</summary>
            public override Type SystemType => typeof(HelpManager);
        }
    }
}