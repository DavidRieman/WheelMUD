//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

using Topshelf;

namespace WheelMUD.Administration.WindowsService
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The main entry point for the service
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the program.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.DependsOnEventLog();
                x.StartAutomatically();
                x.RunAsLocalService();
                x.Service<WheelMudService>(s =>
                {
                    s.ConstructUsing(name => new WheelMudService());
                    s.WhenStarted(service => service.Start(null));
                    s.WhenStopped(service => service.Stop(null));
                });


                x.SetDescription("Allows the WheelMUD MUD Server to run as a Windows Service.");
                x.SetDisplayName("WheelMUD Server Windows Service");
                x.SetServiceName("WheelMUDWindowsService");

                x.AddCommandLineSwitch("-interactive", (flag) => {
                    if (flag)
                    {

                    }
                } );
            });
        }
    }
}