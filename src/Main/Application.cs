//-----------------------------------------------------------------------------
// <copyright file="Application.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Main
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using WheelMUD.Core;
    using WheelMUD.Data;
    using WheelMUD.Interfaces;
    using WheelMUD.Utilities;

    /// <summary>The core application, which can be housed in a console, service, etc.</summary>
    public class Application : ISuperSystem, ISuperSystemSubscriber
    {
        /// <summary>A list of subscribers of this super system.</summary>
        private readonly List<ISuperSystemSubscriber> subscribers = new List<ISuperSystemSubscriber>();

        /// <summary>Prevents a default instance of the <see cref="Application"/> class from being created.</summary>
        private Application()
        {
            UnhandledExceptionHandler.Register(Notify);
        }

        /// <summary>Gets the singleton instance of this <see cref="Application"/>.</summary>
        public static Application Instance { get; } = new Application();

        /// <summary>Gets or sets the available systems.</summary>
        /// <value>The available systems.</value>
        [ImportMany]
        private Lazy<SystemExporter, ExportSystemAttribute>[] AvailableSystems { get; set; }

        /// <summary>Dispose of any resources consumed by Application.</summary>
        public void Dispose()
        {
        }

        /// <summary>Subscribe to the specified super system subscriber.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        public void SubscribeToSystem(ISuperSystemSubscriber sender)
        {
            if (subscribers.Contains(sender))
            {
                throw new DuplicateNameException("The subscriber is already subscribed to Super System events.");
            }

            subscribers.Add(sender);
        }

        /// <summary>Unsubscribe from the specified super system subscriber.</summary>
        /// <param name="sender">The unsubscribing system; generally use 'this'.</param>
        public void UnSubscribeFromSystem(ISuperSystemSubscriber sender)
        {
            subscribers.Remove(sender);
        }

        /// <summary>Start the application.</summary>
        public void Start()
        {
#if DEBUG
            EnsureFilesArePresent();
            EnsureDataIsPresent();
#endif

            InitializeSystems();
        }

        private static void EnsureFilesArePresent()
        {
            string appPath = Assembly.GetExecutingAssembly().Location;
            var appFile = new FileInfo(appPath);

            if (appFile.Directory == null || string.IsNullOrEmpty(appFile.Directory.FullName))
            {
                throw new DirectoryNotFoundException("Could not find the application directory.");
            }

            string appDir = appFile.Directory.FullName;

            if (!Directory.Exists(GameConfiguration.DataStoragePath))
            {
                // If the database file doesn't exist, try to copy the original source.
                string sourcePath = null;
                int i = appDir.IndexOf(Path.DirectorySeparatorChar + "systemdata" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
                if (i > 0)
                {
                    sourcePath = appDir.Substring(0, i) + Path.DirectorySeparatorChar + "systemdata" + Path.DirectorySeparatorChar + "Files" + Path.DirectorySeparatorChar;
                }
                else
                {
                    // The binDebug folder now houses a sub-folder like "netcoreapp3.1" so we need to go up
                    // two levels to search for the starting system data.
                    sourcePath = Path.GetDirectoryName(Path.GetDirectoryName(appDir));
                    sourcePath = Path.Combine(sourcePath + Path.DirectorySeparatorChar + "systemdata" + Path.DirectorySeparatorChar + "Files" + Path.DirectorySeparatorChar);
                }

                DirectoryCopy(sourcePath, GameConfiguration.DataStoragePath, true);
            }

            // TODO: Create a link in the bin folder to the program data folder, for administrative convenience.
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        /// <summary>Stop the application.</summary>
        public void Stop()
        {
            Notify("Shutting Down...");
            Notify("Stopping Services...");

            CoreManager.Instance.Stop();

            Notify("Server is now Stopped");
        }

        /// <summary>Send an update to the system host.</summary>
        /// <param name="sender">The sending system.</param>
        /// <param name="msg">The message to be sent.</param>
        public void UpdateSystemHost(ISystem sender, string msg)
        {
            Notify(sender.GetType().Name + " - " + msg);
        }

        /// <summary>Notify subscribers of the specified message.</summary>
        /// <param name="message">The message to pass along.</param>
        public void Notify(string message)
        {
            foreach (ISuperSystemSubscriber subscriber in subscribers)
            {
                subscriber.Notify(message);
            }
        }

        public string BasicAdministrativeGameInfo
        {
            get
            {
                var sb = new StringBuilder();
                string fancyLine = "-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-";
                sb.AppendLine(fancyLine);
                sb.AppendLine($"  Active Game:  {GameConfiguration.Name}  version {GameConfiguration.Version}");
                if (!string.IsNullOrWhiteSpace(GameConfiguration.Website))
                {
                    sb.AppendLine($"Game Website: {GameConfiguration.Website}");
                }
                sb.AppendLine(fancyLine);
                sb.AppendLine("This game is built from a base of WheelMUD. For more information about the base");
                sb.AppendLine("game engine, visit: https://github.com/DavidRieman/WheelMUD");
                sb.AppendLine();
                sb.AppendLine("This application runs the game server. When running as a console program, it");
                sb.AppendLine("provides an adminstrative command prompt. Type \"HELP\" to list the commands.");
                sb.AppendLine();
                sb.AppendLine("If Windows prompts you for networking access options, you may wish to allow all");
                sb.AppendLine("access if you want to let additional computers/players join the game instance.");
                sb.AppendLine();
                sb.AppendLine("To connect to the game server as a player, you need a telnet client. From this");
                sb.AppendLine("machine you can connect to an IP address of the local machine at port " + GameConfiguration.TelnetPort);
                sb.AppendLine("For example, if you are using Windows and install the basic \"Telnet Client\"");
                sb.AppendLine("option through \"Turn Windows features on or off\", you can open a command");
                sb.AppendLine("prompt and type this command:  telnet 127.0.0.1 " + GameConfiguration.TelnetPort);
                sb.AppendLine("Or you could use the command:  telnet localhost " + GameConfiguration.TelnetPort);
                sb.AppendLine();
                return sb.ToString();
            }
        }

        /// <summary>Ensures that the database and such are present; copies the default if not.</summary>
        private static void EnsureDataIsPresent()
        {
            if ("sqlite".Equals(AppConfigInfo.Instance.RelationalDataProviderName, StringComparison.OrdinalIgnoreCase))
            {
                // Only for SQLite: Make sure that the database is in the right place.
                const string DatabaseName = "WheelMud.net.db";
                string appPath = Assembly.GetExecutingAssembly().Location;
                var appFile = new FileInfo(appPath);

                if (appFile.Directory == null || string.IsNullOrEmpty(appFile.Directory.FullName))
                {
                    throw new DirectoryNotFoundException("Could not find the application directory.");
                }

                string appDir = appFile.Directory.FullName;
                string destPath = Path.Combine(GameConfiguration.DataStoragePath, DatabaseName);

                if (!File.Exists(destPath))
                {
                    // If the database file doesn't exist, try to copy the original source.
                    string sourcePath = null;
                    int i = appDir.IndexOf(Path.DirectorySeparatorChar + "systemdata" + Path.DirectorySeparatorChar, StringComparison.Ordinal);
                    if (i > 0)
                    {
                        sourcePath = appDir.Substring(0, i) + Path.DirectorySeparatorChar + "systemdata" + Path.DirectorySeparatorChar + "SQL" + Path.DirectorySeparatorChar + "SQLite";
                        sourcePath = Path.Combine(sourcePath, DatabaseName);
                    }
                    else
                    {
                        // The binDebug folder now houses a sub-folder like "netcoreapp3.1" so we need to go up two
                        // levels to search for the starting system data.
                        sourcePath = Path.GetDirectoryName(Path.GetDirectoryName(appDir));
                        sourcePath = Path.Combine(sourcePath + Path.DirectorySeparatorChar + "systemdata" + Path.DirectorySeparatorChar + "SQL" + Path.DirectorySeparatorChar + "SQLite", DatabaseName);
                    }

                    if (File.Exists(sourcePath))
                    {
                        File.Copy(sourcePath, destPath);
                    }
                    else
                    {
                        throw new FileNotFoundException("SQLite database was not present in application files directory nor at the expected src location.");
                    }
                }
            }
        }

        /// <summary>Initializes the systems of this application.</summary>
        private void InitializeSystems()
        {
            Notify(DisplayStartup());
            Notify("Starting Application.");

            // Add environment variables needed by the program.
            VariableProcessor.Set("app.path", AppDomain.CurrentDomain.BaseDirectory);

            // Find and prepare all the application's most recent systems from those discovered by MEF.
            var systemExporters = GetLatestSystems();
            CoreManager.Instance.SubSystems = new List<ISystem>();
            foreach (var systemExporter in systemExporters)
            {
                CoreManager.Instance.SubSystems.Add(systemExporter.Instance);
            }

            CoreManager.Instance.SubscribeToSystem(this);
            CoreManager.Instance.Start();

            Notify("All services are started. Server is fully operational.");
        }

        /// <summary>Gets the latest versions of each of our composed systems.</summary>
        /// <returns>A list of SystemExporters as used to instantiate our systems.</returns>
        private List<SystemExporter> GetLatestSystems()
        {
            // If you get a dependency load error here right after trying to update a dependency version (e.g. through NuGet),
            // check to ensure all libraries which reference that dependency have been updated to the same version.
            DefaultComposer.Container.ComposeParts(this);

            // Find the Type of each distinct available system.  ToList forces LINQ to process immediately.
            var systems = new List<SystemExporter>();
            var systemTypes = from s in AvailableSystems select s.Value.SystemType;
            var distinctTypeNames = (from t in systemTypes select t.Name).Distinct().ToList();

            foreach (string systemTypeName in distinctTypeNames)
            {
                // Add only the single most-recent version of this type (if there were more than one found).
                SystemExporter systemToAdd = (from s in AvailableSystems
                                              let type = s.Value.SystemType
                                              where type.Name == systemTypeName
                                              orderby s.Metadata.Priority descending,
                                                      type.Assembly.GetName().Version.Major descending,
                                                      type.Assembly.GetName().Version.Minor descending,
                                                      type.Assembly.GetName().Version.Build descending,
                                                      type.Assembly.GetName().Version.Revision descending
                                              select s.Value).FirstOrDefault();
                systems.Add(systemToAdd);
            }

            return systems;
        }

        /// <summary>Display startup texts for the game administrator.</summary>
        /// <returns>The startup splash screen text.</returns>
        private string DisplayStartup()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Starting up... " + DateTime.Now.ToString());
            sb.AppendLine(BasicAdministrativeGameInfo);
            return sb.ToString();
        }
    }
}