//-----------------------------------------------------------------------------
// <copyright file="WheelMUDService.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The WheelMUD Server Windows Service.
// </summary>
//-----------------------------------------------------------------------------

namespace TestHarness
{
    using System;
    using Topshelf;
    using WheelMUD.Main;

    public class WheelMudService : ServiceControl
    {
        /// <summary>The application.</summary>
        private readonly Application application;

        /// <summary>Initializes a new instance of the <see cref="WheelMudService"/> class.</summary>
        public WheelMudService()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            this.application = Application.Instance;
        }

        public bool Start(HostControl hostControl)
        {
            this.application.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            this.application.Stop();
            return true;
        }
    }
}