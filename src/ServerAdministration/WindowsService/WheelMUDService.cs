//-----------------------------------------------------------------------------
// <copyright file="WheelMUDService.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The WheelMUD Server Windows Service.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Administration.WindowsService
{
    using System;
    using HoytSoft.Common.Services;
    using WheelMUD.Main;

    /// <summary>The WheelMUD Windows service.</summary>
    [Service(
        "WheelMUDWindowsService",
        "WheelMUD Server Windows Service",
        "Allows the WheelMUD MUD Server to run as a Windows Service.",
        AutoInstall = true,
        ServiceType = ServiceType.OwnProcess,
        ServiceStartType = ServiceStartType.AutoStart,
        ServiceControls = ServiceControls.StartAndStop | ServiceControls.Shutdown,
        LogName = "WheelMUDWindowsService")
    ]
    public partial class WheelMUDService : ServiceBase
    {
        /// <summary>The application.</summary>
        private readonly Application application;

        /// <summary>Initializes a new instance of the WheelMUDService class.</summary>
        public WheelMUDService()
        {
            this.InitializeComponent();
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.application = Application.Instance;
        }

        /// <summary>Starts the service.</summary>
        public void StartService()
        {
            this.Start();
        }

        /// <summary>Stops the service.</summary>
        public void StopService()
        {
            this.Stop();
        }

        /// <summary>This method is called when the service starts.</summary>
        protected override void Start()
        {
            this.application.Start();
            base.Start();
        }

        /// <summary>This method is called when the service stops.</summary>
        protected override void Stop()
        {
            this.application.Stop();
            base.Stop();
        }
    }
}