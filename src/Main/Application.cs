//-----------------------------------------------------------------------------
// <copyright file="Application.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The core application, which can be housed in a console, service, etc.
// </summary>
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

    using WheelMUD.Core;
    using WheelMUD.Data;
    using WheelMUD.Interfaces;
    using WheelMUD.Utilities;

    /// <summary>The core application, which can be housed in a console, service, etc.</summary>
    public class Application : ISuperSystem, ISuperSystemSubscriber
    {
        /// <summary>The singleton instance of this class.</summary>
        private static Application instance = new Application();

        /// <summary>A list of subscribers of this super system.</summary>
        private readonly List<ISuperSystemSubscriber> subscribers = new List<ISuperSystemSubscriber>();

        /// <summary>The view engine.</summary>
        private ViewEngine viewEngine;
        
        /// <summary>Prevents a default instance of the <see cref="Application"/> class from being created.</summary>
        private Application()
        {
            UnhandledExceptionHandler.Register(this.Notify);
        }

        /// <summary>Gets the singleton instance of this <see cref="Application"/>.</summary>
        public static Application Instance
        {
            get { return instance; }
        }

        /// <summary>Gets or sets the available systems.</summary>
        /// <value>The available systems.</value>
        [ImportMany]
        private List<SystemExporter> AvailableSystems { get; set; }

        /// <summary>Dispose of any resources consumed by Application.</summary>
        public void Dispose()
        {
        }

        /// <summary>Subscribe to the specified super system subscriber.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        public void SubscribeToSystem(ISuperSystemSubscriber sender)
        {
            if (this.subscribers.Contains(sender))
            {
                throw new DuplicateNameException("The subscriber is already subscribed to Super System events.");
            }

            this.subscribers.Add(sender);
        }

        /// <summary>Unsubscribe from the specified super system subscriber.</summary>
        /// <param name="sender">The unsubscribing system; generally use 'this'.</param>
        public void UnSubscribeFromSystem(ISuperSystemSubscriber sender)
        {
            this.subscribers.Remove(sender);
        }

        /// <summary>Start the application.</summary>
        public void Start()
        {
#if DEBUG
            EnsureFilesArePresent();
            EnsureDataIsPresent();
#endif

            this.InitializeSystems();
        }

        private void EnsureFilesArePresent()
        {
            string appPath = Assembly.GetExecutingAssembly().Location;
            var appFile = new FileInfo(appPath);

            if (appFile.Directory == null || string.IsNullOrEmpty(appFile.Directory.FullName))
            {
                throw new DirectoryNotFoundException("Could not find the application directory.");
            }

            string appDir = appFile.Directory.FullName;
            string destDir = Configuration.GetDataStoragePath();

            if (!Directory.Exists(destDir))
            {
                // If the database file doesn't exist, try to copy the original source.
                string sourcePath = null;
                int i = appDir.IndexOf("\\systemdata\\", System.StringComparison.Ordinal);
                if (i > 0)
                {
                    sourcePath = appDir.Substring(0, i) + "\\systemdata\\Files\\";
                }
                else
                {
                    sourcePath = Path.GetDirectoryName(appDir);
                    sourcePath = Path.Combine(sourcePath + "\\systemdata\\Files\\");
                }

                DirectoryCopy(sourcePath, destDir, true);
            }
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
            this.Notify("Shutting Down...");
            this.Notify("Stopping Services...");

            CoreManager.Instance.Stop();

            this.Notify("Server is now Stopped");
        }

        /// <summary>Send an update to the system host.</summary>
        /// <param name="sender">The sending system.</param>
        /// <param name="msg">The message to be sent.</param>
        public void UpdateSystemHost(ISystem sender, string msg)
        {
            this.Notify(sender.GetType().Name + " - " + msg);
        }

        /// <summary>Temporary help system just so that we have something here if the person types help at the console.</summary>
        public void DisplayHelp()
        {
            var path = Path.Combine(Configuration.GetDataStoragePath(), "ConsoleHelp.txt");
            var help = File.ReadAllText(path);
            this.Notify(this.viewEngine.RenderView(help));
        }

        /// <summary>Notify subscribers of the specified message.</summary>
        /// <param name="message">The message to pass along.</param>
        public void Notify(string message)
        {
            foreach (ISuperSystemSubscriber subscriber in this.subscribers)
            {
                subscriber.Notify(message);
            }
        }

        /// <summary>Ensures that the database and such are present; copies the default if not.</summary>
        private static void EnsureDataIsPresent()
        {
            string currentProviderName = Helpers.GetCurrentProviderName();

            if (currentProviderName.ToLowerInvariant() == "system.data.sqlite")
            {
                // Only for SQLite
                // Make sure that the database is in the right place.
                const string DatabaseName = "WheelMud.net.db";
                string appPath = Assembly.GetExecutingAssembly().Location;
                var appFile = new FileInfo(appPath);

                if (appFile.Directory == null || string.IsNullOrEmpty(appFile.Directory.FullName))
                {
                    throw new DirectoryNotFoundException("Could not find the application directory.");
                }

                string appDir = appFile.Directory.FullName;
                string destDir = Configuration.GetDataStoragePath();
                string destPath = Path.Combine(destDir, DatabaseName);

                if (!File.Exists(destPath))
                {
                    // If the database file doesn't exist, try to copy the original source.
                    string sourcePath = null;
                    int i = appDir.IndexOf("\\systemdata\\", System.StringComparison.Ordinal);
                    if (i > 0)
                    {
                        sourcePath = appDir.Substring(0, i) + "\\systemdata\\SQL\\SQLite";
                        sourcePath = Path.Combine(sourcePath, DatabaseName);
                    }
                    else
                    {
                        sourcePath = Path.GetDirectoryName(appDir);
                        sourcePath = Path.Combine(sourcePath + "\\systemdata\\SQL\\SQLite", DatabaseName);
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
            this.viewEngine = new ViewEngine { ReplaceNewLine = false };
            this.viewEngine.AddContext("MudAttributes", MudEngineAttributes.Instance);

            this.Notify(this.DisplayStartup());
            this.Notify("Starting Application.");

            // Add environment variables needed by the program.
            VariableProcessor.Set("app.path", AppDomain.CurrentDomain.BaseDirectory);

            // Find and prepare all the application's most recent systems from those discovered by MEF.
            var systemExporters = this.GetLatestSystems();
            CoreManager.Instance.SubSystems = new List<ISystem>();
            foreach (var systemExporter in systemExporters)
            {
                CoreManager.Instance.SubSystems.Add(systemExporter.Instance);
            }

            CoreManager.Instance.SubscribeToSystem(this);
            CoreManager.Instance.Start();

            this.Notify("All services are started. Server is fully operational.");
        }

        /// <summary>Gets the latest versions of each of our composed systems.</summary>
        /// <returns>A list of SystemExporters as used to instantiate our systems.</returns>
        private List<SystemExporter> GetLatestSystems()
        {
            DefaultComposer.Container.ComposeParts(this);

            // Find the Type of each distinct available system.  ToList forces LINQ to process immediately.
            var systems = new List<SystemExporter>();
            var systemTypes = from s in this.AvailableSystems select s.SystemType;
            var distinctTypeNames = (from t in systemTypes select t.FullName).Distinct().ToList();

            foreach (string systemTypeName in distinctTypeNames)
            {
                // Add only the single most-recent version of this type (if there were more than one found).
                SystemExporter systemToAdd = (from s in this.AvailableSystems
                                              where s.SystemType.FullName == systemTypeName
                                              orderby s.SystemType.Assembly.GetName().Version.Major descending,
                                                      s.SystemType.Assembly.GetName().Version.Minor descending,
                                                      s.SystemType.Assembly.GetName().Version.Build descending,
                                                      s.SystemType.Assembly.GetName().Version.Revision descending
                                              select s).FirstOrDefault();
                systems.Add(systemToAdd);
            }

            return systems;
        }

        /// <summary>Display startup texts.</summary>
        /// <returns>The startup splash screen text.</returns>
        private string DisplayStartup()
        {
            var filePath = Path.Combine(Configuration.GetDataStoragePath(), "ConsoleOpen.txt");
            var splash = File.ReadAllText(filePath);
            return this.viewEngine.RenderView(splash);
        }
    }
}