//-----------------------------------------------------------------------------
// <copyright file="WheelMUDService.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using Topshelf;
using WheelMUD.Main;

namespace ServerHarness
{
    /// <summary>The WheelMUD Server Windows Service.</summary>
    public class WheelMudService : ServiceControl
    {
        /// <summary>The application.</summary>
        private readonly Application application;

        /// <summary>Initializes a new instance of the <see cref="WheelMudService"/> class.</summary>
        public WheelMudService()
        {
            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            application = Application.Instance;
        }

        public bool Start(HostControl hostControl)
        {
            application.Start();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            application.Stop();
            return true;
        }
    }
}