//-----------------------------------------------------------------------------
// <copyright file="PlacesManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using WheelMUD.Data.Repositories;
using WheelMUD.Utilities;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>High level manager that provides tracking and global collection of all places.</summary>
    public class PlacesManager : ManagerSystem, IRecomposable
    {
        /// <summary>Prevents a default instance of the <see cref="PlacesManager"/> class from being created.</summary>
        private PlacesManager() { }

        /// <summary>Gets the singleton instance of the <see cref="PlacesManager"/> system.</summary>
        public static PlacesManager Instance { get; } = new PlacesManager();

        /// <summary>Gets the world.</summary>
        public Thing World { get; private set; }

        /// <summary>Gets the world behavior.</summary>
        public WorldBehavior WorldBehavior { get; private set; }

        /// <summary>Gets the default starting location for new players (or players whose saved location is no longer valid).</summary>
        /// <remarks>Typically this location is a Room for Room-based MUDs. It can be reconfigured through App.config.</remarks>
        public Thing DefaultStartingLocation { get; private set; }

        /// <summary>Starts this system's individual components.</summary>
        public override void Start()
        {
            SystemHost.UpdateSystemHost(this, "Starting...");
            Recompose();

            World = DocumentRepository<Thing>.Load("world/root");
            World?.RepairParentTree();
            WorldBehavior = World?.FindBehavior<WorldBehavior>();

            if (WorldBehavior == null)
            {
                SystemHost.UpdateSystemHost(this, "Could not load a functional world/root! Attempting world generation...");
                CreateDefaultWorld();
                WorldBehavior = World?.FindBehavior<WorldBehavior>();

                if (World == null || WorldBehavior == null)
                {
                    throw new Exception("World could not be found and could not be regenerated. Cannot start server.");
                }

                World.Save();
            }

            // Wait for any pending world loads to be complete, before deciding whether there are any areas which need
            // to be regenerated from scratch, etc.
            while (ThingManager.Instance.LoadsPending)
            {
                Thread.Sleep(10);
            }
            UpdateAreas();

            // If no custom systems have already defined the DefaultStartingLocation, find the default room for players
            // to start in via the default mechanism.
            DefaultStartingLocation ??= FindDefaultStartingLocation();

            SystemHost.UpdateSystemHost(this, "Started");
        }

        [ImportMany]
        public List<Lazy<CreatorDefinitions.World, CreatorExports.World>> WorldCreators { get; set; }

        [ImportMany]
        public List<Lazy<CreatorDefinitions.Area, CreatorExports.Area>> AreaCreators { get; set; }

        private void CreateDefaultWorld()
        {
            lock (lockObject)
            {
                var worldCreator = DefaultComposer.GetInstance(WorldCreators);
                World = worldCreator?.Create();
            }
        }

        private void UpdateAreas()
        {
            lock (lockObject)
            {
                bool updatedWorld = false;
                var areaCreators = DefaultComposer.GetInstances(AreaCreators);
                foreach (var area in areaCreators)
                {
                    if (ThingManager.Instance.FindThing(area.ID) == null)
                    {
                        var newArea = area.Create();
                        if (newArea != null)
                        {
                            World.Add(newArea);
                            newArea.Save();
                            updatedWorld = true;
                        }
                    }
                }

                if (updatedWorld)
                {
                    World.Save();
                }
            }
        }

        /// <summary>Recompose the subcomponents of this PlacesManager.</summary>
        public void Recompose()
        {
            lock (lockObject)
            {
                DefaultComposer.Container.ComposeParts(this);
            }
        }
        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            SystemHost.UpdateSystemHost(this, "Stopping...");
            SystemHost.UpdateSystemHost(this, "Stopped");
        }

        private static Thing FindDefaultStartingLocation()
        {
            return (from t in ThingManager.Instance.Things
                    where t.Id == GameConfiguration.DefaultRoomID
                    select t).FirstOrDefault();
        }

        /// <summary>Registers the <see cref="PlacesManager"/> system for export.</summary>
        /// <remarks>Assists with non-rebooting updates of the <see cref="PlacesManager"/> system through MEF.</remarks>
        [CoreExports.System(0)]
        public class PlacesManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            public override ISystem Instance => PlacesManager.Instance;

            /// <summary>Gets the Type of this system.</summary>
            public override Type SystemType => typeof(PlacesManager);
        }
    }
}