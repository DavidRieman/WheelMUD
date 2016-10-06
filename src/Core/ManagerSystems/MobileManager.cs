//-----------------------------------------------------------------------------
// <copyright file="MobileManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   The mobiles manager class.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Interfaces;

    /// <summary>The mobiles manager class.</summary>
    /// <remarks>
    /// @@@ TODO: Provide search ability of registered 'mobiles' through LINQ rather than, 
    /// or in addition to, specific-purpose search methods.
    /// </remarks>
    public class MobileManager : ManagerSystem
    {
        /// <summary>The singleton instance of this class.</summary>
        private static readonly MobileManager SingletonInstance = new MobileManager();

        /// <summary>The list of managed mobiles.</summary>
        private readonly List<Thing> mobiles = new List<Thing>();

        /// <summary>Prevents a default instance of the <see cref="MobileManager"/> class from being created.</summary>
        private MobileManager()
        {
        }

        /// <summary>Gets the singleton instance of the MobileManager class.</summary>
        public static MobileManager Instance
        {
            get { return SingletonInstance; }
        }

        /// <summary>Called when an action has been received, the manager can then put the action onto the queue.</summary>
        /// <param name="sender">The entity sending the action.</param>
        /// <param name="actionInput">The action input to be enqueued.</param>
        public void OnActionReceived(Thing sender, ActionInput actionInput)
        {
            CommandManager.Instance.EnqueueAction(actionInput);
        }

        /// <summary>Registers a mobile with the mobile manager.</summary>
        /// <param name="mobile">An instance of a mobile to be registered.</param>
        public void RegisterMobile(Thing mobile)
        {
            this.mobiles.Add(mobile);
        }

        /// <summary>Starts this system.</summary>
        public override void Start()
        {
            this.SystemHost.UpdateSystemHost(this, "Starting...");

            ////temp. Add a mobile explicitly here.
            ////TODO: replace with proper persistence mechanism
            ////var basicGuardMobBrain = new BasicGuardMobBrain();
            ////var mobile = new Mobile(basicGuardMobBrain, CoreManager.Instance.PlacesManager.World);
            ////mobile.ActionReceived += mobile_ActionReceived;
            ////basicGuardMobBrain.Entity = mobile;
            ////mobile.Name = "George";
            ////Room room = CoreManager.Instance.PlacesManager.World.FindRoom("1");
            ////mobile.Move(room);
            ////basicGuardMobBrain.Start();
            ////_mobiles.Add(mobile);
            ////mobile.Load();

            this.SystemHost.UpdateSystemHost(this, "Started");
        }

        /// <summary>Stops this system's individual components.</summary>
        public override void Stop()
        {
            this.SystemHost.UpdateSystemHost(this, "Stopping...");

            lock (this.lockObject)
            {
                this.mobiles.Clear();
            }

            this.SystemHost.UpdateSystemHost(this, "Stopped");
        }

        /// <summary>Finds a mobile using the predicate passed.</summary>
        /// <param name="predicate">The predicate to match.</param>
        /// <returns>The IMobile found.</returns>
        public Thing FindMobile(Predicate<Thing> predicate)
        {
            lock (this.lockObject)
            {
                return this.mobiles.Find(predicate);
            }
        }

        /// <summary>Finds a mobile using a name or part name.</summary>
        /// <param name="name">The name of the mobile to return.</param>
        /// <param name="partialMatch">Used to indicate whether the search criteria can look at just the start of the name.</param>
        /// <returns>The IMobile found.</returns>
        public Thing FindMobileByName(string name, bool partialMatch)
        {
            name = name.ToLower();
            lock (this.lockObject)
            {
                Thing mobile = this.mobiles.Find(m => m.Name.ToLower().Equals(name));

                if (mobile == null && partialMatch)
                {
                    mobile = this.mobiles.Find(m => m.Name.ToLower().StartsWith(name));
                }

                return mobile;
            }
        }

        /// <summary>Finds a Mobile object by the identifier.</summary>
        /// <param name="id">Identifier of the mobile.</param>
        /// <returns>returns a mobile object.</returns>
        public Thing FindMobileById(string id)
        {
            lock (this.lockObject)
            {
                Thing mobile = this.mobiles.Find(m => m.ID.Equals(id));
                return mobile;
            }
        }

        /* @@@ ?
        /// <summary>This method is called when a mobile receives an action.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="actionInput">The action input received.</param>
        /// <param name="delay">The delay before the action should occur.</param>
        private static void Mobile_ActionReceived(IController sender, ActionInput actionInput, TimeSpan delay)
        {
            CoreManager.Instance.CommandManager.Enqueue(actionInput);
        }*/

        /// <summary>Registers the <see cref="MobileManager"/> system for export.</summary>
        /// <remarks>Assists with non-rebooting updates of the <see cref="MobileManager"/> system through MEF.</remarks>
        [ExportSystem]
        public class MobileManagerExporter : SystemExporter
        {
            /// <summary>Gets the singleton system instance.</summary>
            public override ISystem Instance
            {
                get { return MobileManager.Instance; }
            }

            /// <summary>Gets the Type of this system.</summary>
            public override Type SystemType
            {
                get { return typeof(MobileManager); }
            }
        }
    }
}