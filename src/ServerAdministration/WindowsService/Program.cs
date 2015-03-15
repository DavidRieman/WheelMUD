//-----------------------------------------------------------------------------
// <copyright file="Program.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Administration.WindowsService
{
    using System;
    using System.Windows.Forms;
    using HoytSoft.Common.Services;

    /// <summary>
    /// The main entry point for the service
    /// </summary>
    public class Program
    {
        private static string[] arguments;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the program.</param>
        [STAThread]
        public static void Main(string[] args)
        {
            arguments = args;
            ProcessSwitches(args);
        }

        /// <summary>
        /// Loop through the command line arguments passed into the application, and parse each switch
        /// </summary>
        /// <param name="args">String array of command line switches to parse</param>
        private static void ProcessSwitches(string[] args)
        {
            if (args.Length > 0)
            {
                // If command line arguments are specified, then loop through them and parse them.
                for (int i = 0; i < args.Length; i++)
                {
                    ParseSwitch(args[i]);
                }
            }
            else
            {
                // If no command line arguments are passed, parse null to just run the service.
                ParseSwitch(string.Empty);
            }
        }

        /// <summary>
        /// Parse the command line switch and execute the appropriate method based on the switch
        /// </summary>
        /// <param name="cmdSwitch">The command line switch to parse</param>
        private static void ParseSwitch(string cmdSwitch)
        {
            switch (cmdSwitch.ToLower())
            {
                // Run in interactive mode
                case "-interactive":
                    {
                        RunInteractive();
                        break;
                    }

                // Just run the service
                default:
                    {
                        // More than one user Service may run within the same process. To add
                        // another service to this process, change the following line to
                        // create a second service object. For example,
                        //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
                        ////var servicesToRun = new ServiceBase[] { new WheelMUDService() };
                        ServiceBase.RunService(arguments, typeof(WheelMUDService));
                        break;
                    }
            }
        }

        /// <summary>
        /// Run the program in interactive mode.
        /// </summary>
        private static void RunInteractive()
        {
            Application.EnableVisualStyles();

            var service = new WheelMUDService();
            var debugForm = new ServiceController { Service = service };

            Application.Run(debugForm);
        }
    }
}