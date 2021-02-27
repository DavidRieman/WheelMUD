//-----------------------------------------------------------------------------
// <copyright file="CoreManager.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core.Interfaces;
using WheelMUD.Server.Interfaces;

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Data;

    /// <summary>
    /// Example of a Registry (singleton) object available throughout the application that contains
    /// all the major services, for easy access no matter where you are in the system.
    /// </summary>
    public class CoreManager : ISuperSystem
    {
        /// <summary>The list of super system subscribers.</summary>
        private readonly List<ISuperSystemSubscriber> subscribers = new List<ISuperSystemSubscriber>();

        /// <summary>Prevents a default instance of the CoreManager class from being created.</summary>
        private CoreManager()
        {
        }

        /// <summary>Gets the CoreManager singleton instance.</summary>
        public static CoreManager Instance { get; } = new CoreManager();

        /// <summary>Gets or sets a list of system plug-ins.</summary>
        [ImportMany(typeof(ISystemPlugIn))]
        public List<ISystemPlugIn> SystemPlugIns { get; set; }

        /// <summary>Gets or sets a set of sub-systems.</summary>
        public List<ISystem> SubSystems { get; set; }

        /// <summary>Subscribe to the specified super system subscriber.</summary>
        /// <param name="sender">The subscribing system; generally use 'this'.</param>
        public void SubscribeToSystem(ISuperSystemSubscriber sender)
        {
            if (subscribers.Contains(sender))
            {
                throw new DuplicateNameException("The subscriber is already subscribed to Super System events: " + sender);
            }

            subscribers.Add(sender);
        }

        /// <summary>Unsubscribe from the specified super system subscriber.</summary>
        /// <param name="sender">The unsubscribing system; generally use 'this'.</param>
        public void UnSubscribeFromSystem(ISuperSystemSubscriber sender)
        {
            subscribers.Remove(sender);
        }

        /// <summary>Start the CoreManager.</summary>
        public void Start()
        {
            // Load the initial plugins and such; note that ATM CoreManager itself is not recomposable, but the idea
            // is to allow individual subsystems/plugins to  recompose on the fly w/out server reboots, etc.
            // TODO: Implement file system watcher on the execution directory to trigger auto-recompositions.
            DefaultComposer.Container.ComposeParts(this);

            SubscribeToSystems();

            foreach (var system in SubSystems)
            {
                system.Start();
            }

            if (SystemPlugIns != null)
            {
                // Start all the plugins
                foreach (var systemPlugIn in SystemPlugIns)
                {
                    systemPlugIn.Start();
                }
            }
        }

        /// <summary>Stop the CoreManager.</summary>
        public void Stop()
        {
            try
            {
                foreach (var system in SubSystems)
                {
                    system.Stop();
                }

                if (SystemPlugIns != null)
                {
                    // Stop all the plugins
                    foreach (var systemPlugIn in SystemPlugIns)
                    {
                        systemPlugIn.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(string.Format("CoreManager.Stop():\n{0}\n{1}", ex.Message, ex.StackTrace));
                throw;
            }
        }

        /// <summary>Send an update to the system host.</summary>
        /// <param name="sender">The sending system.</param>
        /// <param name="msg">The message to be sent.</param>
        public void UpdateSystemHost(ISystem sender, string msg)
        {
            NotifySubscribers(sender + " - " + msg);
        }

        /// <summary>Notify subscribers of the supplied message.</summary>
        /// <param name="message">The message to pass along.</param>
        private void NotifySubscribers(string message)
        {
            foreach (ISuperSystemSubscriber subscriber in subscribers)
            {
                subscriber.Notify(message);
            }
        }

        /// <summary>Subscribe to notifications from the various systems.</summary>
        private void SubscribeToSystems()
        {
            // Subscribe each system to the supersystem.
            foreach (var system in SubSystems)
            {
                system.SubscribeToSystemHost(this);
            }

            if (SystemPlugIns != null)
            {
                // Subscribe all the plugins to the host
                foreach (var systemPlugIn in SystemPlugIns)
                {
                    systemPlugIn.SubscribeToSystemHost(this);
                }
            }
        }
    }
}