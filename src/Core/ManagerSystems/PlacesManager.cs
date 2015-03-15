//-----------------------------------------------------------------------------
// <copyright file="PlacesManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   High level manager that provides tracking and global collection of all places.
//   Created: August 2006 by Foxedup
//   Modified: December 2009 by bengecko.  Pointed it at AbstractManager
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    using WheelMUD.Interfaces;
    using WheelMUD.Utilities;

    /// <summary>
    /// High level manager that provides tracking and global collection of all places.
    /// </summary>
    public class PlacesManager : ManagerSystem
    {
        [ExportSystem]
        public class PlacesManagerExporter : SystemExporter
        {
            public override ISystem Instance
            {
                get { return PlacesManager.Instance; }
            }

            public override Type SystemType
            {
                get { return typeof(PlacesManager); }
            }
        }

        /// <summary>The singleton instance synchronization locking object.</summary>
        private static readonly object instanceLockObject = new object();

        /// <summary>The singleton instance of this class.</summary>
        private static PlacesManager instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="PlacesManager"/> class from being created. 
        /// Initializes a new instance of the PlacesManager class.
        /// </summary>
        private PlacesManager()
        {
            // @@@ assign to ItemManager instance? is it needed? currently disabled...
            this.WorldBehavior = new WorldBehavior();
            this.World = new Thing(this.WorldBehavior)
            {
                Name = MudEngineAttributes.Instance.MudName
            };
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static PlacesManager Instance
        {
            get
            {
                // Using if-lock-if pattern to avoid locks for most cases yet create only once instance in early initialization.
                if (instance == null)
                {
                    lock (instanceLockObject)
                    {
                        if (instance == null)
                        {
                            instance = new PlacesManager();
                        }
                    }
                }

                return instance;
            }
        }

        /// <summary>Gets the world.</summary>
        public Thing World { get; private set; }

        /// <summary>
        /// Gets the world behavior.
        /// </summary>
        public WorldBehavior WorldBehavior { get; private set; }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
            this.SystemHost.UpdateSystemHost(this, "Starting...");

            this.WorldBehavior.Load();

            this.SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            this.SystemHost.UpdateSystemHost(this, "Stopping...");

            //@@@this.WorldBehavior.Areas.Clear();

            this.SystemHost.UpdateSystemHost(this, "Stopped");
        }
    }
}