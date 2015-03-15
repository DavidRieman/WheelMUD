//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Permissive License. All other rights reserved.
// </copyright>
// <summary>
//   A program for copying a folder into the ProgramData area of the OS.
// </summary>
//-----------------------------------------------------------------------------

namespace CopyToAppData
{
    using System;
    using System.IO;
    using System.Reflection;

    using Nini.Config;

    /// <summary>
    /// The main CopyToAppData program class.
    /// </summary>
    public class CopyToAppDataProgram
    {
        /// <summary>
        /// The main entry point for the program.
        /// </summary>
        /// <param name="args">The command line arguments specified (if any).</param>
        /// <returns>An application exit code (being non-zero to represent error conditions).</returns>
        public static int Main(string[] args)
        {
            string commonAppData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string dataPath = Path.Combine(commonAppData, "WheelMUD");
            string mudName = GetMudName();
            dataPath = Path.Combine(dataPath, mudName);
            string dataFolderPath = Path.Combine(dataPath, "Files");

            if (args.Length <= 0)
            {
                return Usage();
            }

            string filesLocation = args[0];
            if (!Directory.Exists(filesLocation))
            {
                Console.WriteLine("Files location specified does not exist.");
                return Usage();
            }

            if (Directory.Exists(dataFolderPath))
            {
                // For now we don't support an overwrite flag but if we did we could check it and just run 
                //    DeleteFolderContents(dataFolderPath);
                // Otherwise, if the target already exists, we are already "done" since we can't overwrite it.
                Console.WriteLine("Target folder already exists; CopyToAppData is done.");
                return 0;
            }

            return CopyFolder(filesLocation, dataFolderPath);
        }

        /// <summary>
        /// Deep copy the entire folder from one path to another.
        /// </summary>
        /// <param name="sourceFolder">The source folder to copy.</param>
        /// <param name="destFolder">The designation of the folder copy.</param>
        /// <returns>An application exit code (being non-zero to represent error conditions).</returns>
        public static int CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest);
            }

            string[] folders = Directory.GetDirectories(sourceFolder);
            foreach (string folder in folders)
            {
                if (!folder.Contains(".svn"))
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);
                }
            }

            return 0;
        }

        private static string GetMudName()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "mud.config");
            var config = new DotNetConfigSource(path);

            string mudName = config.Configs["EngineAttributes"].GetString("name");

            return mudName;
        }

        ////static public void DeleteFolderContents(string root)
        ////{
        ////    string[] contents = Directory.GetFiles(root);
        ////    if (contents.Length > 0)
        ////    {
        ////        foreach (var file in contents)
        ////        {
        ////            FileAttributes fileAttributes = File.GetAttributes(file);
        ////            if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
        ////            {
        ////                File.SetAttributes(file, FileAttributes.Normal);
        ////            }
        ////            File.Delete(file);
        ////        }
        ////    }
        ////    string[] folders = Directory.GetDirectories(root);
        ////    if (folders.Length > 0)
        ////    {
        ////        foreach (var folder in folders)
        ////        {
        ////            DeleteFolderContents(folder);
        ////            if (Directory.Exists(folder))
        ////            {
        ////                Directory.Delete(folder); 
        ////            }
        ////        } 
        ////    }
        ////    else
        ////    {
        ////        Directory.Delete(root);
        ////    }
        ////}

        /// <summary>
        /// Print the usage info.
        /// </summary>
        /// <returns>An application exit code (being non-zero since we did not succeed in a copy action).</returns>
        private static int Usage()
        {
            Console.WriteLine("USAGE: CopyToAppData ExistingFilesPath");
            return -1;
        }
    }
}