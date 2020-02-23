//-----------------------------------------------------------------------------
// <copyright file="DefaultSplashScreenRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using WheelMUD.Utilities;

    [RendererExports.SplashScreen(0)]
    public class DefaultSplashScreenRenderer : RendererDefinitions.SplashScreen
    {
        private static readonly List<string> splashScreens = new List<string>();

        static DefaultSplashScreenRenderer()
        {
            LoadSplashScreens();
        }

        /// <summary>Load the splash screens.</summary>
        private static void LoadSplashScreens()
        {
            string name = Configuration.GetDataStoragePath();
            string path = Path.Combine(Path.GetDirectoryName(name), "Files");
            path = Path.Combine(path, "SplashScreens");

            var dirInfo = new DirectoryInfo(path);
            var files = new List<FileInfo>(dirInfo.GetFiles());

            foreach (var fileInfo in files)
            {
                string splashContent;
                using (var streamReader = new StreamReader(fileInfo.FullName))
                {
                    splashContent = streamReader.ReadToEnd();
                }

                var attributes = MudEngineAttributes.Instance;
                string renderedScreen = string.Format(splashContent,
                    attributes.MudName,    // {0} in splash screen files
                    attributes.Version,    // {1} in splash screen files
                    attributes.Website,    // {2} in splash screen files
                    attributes.Copyright); // {3} in splash sceren files

                splashScreens.Add(renderedScreen);
            }
        }

        public override string Render()
        {
            if (!splashScreens.Any())
            {
                return $"Welcome to {MudEngineAttributes.Instance.MudName}.<%nl%>";
            }

            var random = new Random();
            return splashScreens[random.Next(0, splashScreens.Count)];
        }
    }
}
