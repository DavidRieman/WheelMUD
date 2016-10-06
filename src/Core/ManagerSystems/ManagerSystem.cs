//-----------------------------------------------------------------------------
// <copyright file="ManagerSystem.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   High level manager that provides tracking and global collection of Things.
//   Created: December 2009 by bengecko.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using WheelMUD.Interfaces;

    /// <summary>A base class for the various manager systems; handles much of the otherwise-redundant system details.</summary>
    public abstract class ManagerSystem : ISystem
    {
        /// <summary>Gets the host of the manager system.</summary>
        public ISystemHost SystemHost { get; private set; }

        /// <summary>The synchronization locking object for the manager.</summary>
        protected readonly object lockObject = new object();

        /// <summary>Subscribes this system to the specified system host, so that host can receive updates.</summary>
        /// <param name="systemHost">The system host to receive our updates.</param>
        public void SubscribeToSystemHost(ISystemHost systemHost)
        {
            this.SystemHost = systemHost;
        }

        /// <summary>Start this manager system.</summary>
        public abstract void Start();

        /// <summary>Stop this manager system.</summary>
        public abstract void Stop();

        /// <summary>Allows the sub system host to receive an update when subscribed to this system.</summary>
        /// <param name="sender">The sender of the update.</param>
        /// <param name="msg">The message to be sent.</param>
        public void UpdateSubSystemHost(ISubSystem sender, string msg)
        {
            this.SystemHost.UpdateSystemHost(this, msg);
        }
    }
}