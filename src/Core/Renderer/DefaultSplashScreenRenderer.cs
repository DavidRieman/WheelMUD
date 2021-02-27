//-----------------------------------------------------------------------------
// <copyright file="DefaultSplashScreenRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WheelMUD.Utilities;

namespace WheelMUD.Core.Renderer
{
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
            string path = Path.Combine(GameConfiguration.DataStoragePath, "SplashScreens");
            var dirInfo = new DirectoryInfo(path);
            var files = new List<FileInfo>(dirInfo.GetFiles());

            foreach (var fileInfo in files)
            {
                string splashContent;
                using (var streamReader = new StreamReader(fileInfo.FullName))
                {
                    splashContent = streamReader.ReadToEnd();
                }

                string renderedScreen = string.Format(splashContent,
                    GameConfiguration.Name,        // {0} in splash screen files
                    GameConfiguration.Version,     // {1} in splash screen files
                    GameConfiguration.Website,     // {2} in splash screen files
                    GameConfiguration.Copyright);  // {3} in splash screen files

                splashScreens.Add(renderedScreen);
            }
        }

        public override string Render()
        {
            if (!splashScreens.Any())
            {
                return $"Welcome to {GameConfiguration.Name}.{AnsiSequences.NewLine}";
            }

            var random = new Random();
            return splashScreens[random.Next(0, splashScreens.Count)];
        }
    }
}
