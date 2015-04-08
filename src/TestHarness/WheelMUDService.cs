//-----------------------------------------------------------------------------
// <copyright file="WheelMUDService.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The WheelMUD Server Windows Service.
// </summary>
//-----------------------------------------------------------------------------

using Topshelf;
using System;
using WheelMUD.Main;

namespace TestHarness
{
    public partial class WheelMudService : ServiceControl
    {
        /// <summary>The application.</summary>
        private readonly Application _application;

        /// <summary>Initializes a new instance of the WheelMUDService class.</summary>
        public WheelMudService()
        {
            InitializeComponent();
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _application = Application.Instance;
        }

        public bool Start(HostControl hostControl)
        {
            _application.Start();

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _application.Stop();

            return true;
        }
    }
}