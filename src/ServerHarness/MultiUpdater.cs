﻿//-----------------------------------------------------------------------------
// <copyright file="MultiUpdater.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ServerHarness
{
    using System.Collections.Generic;
    using WheelMUD.Interfaces;

    /// <summary>Logs notifications to a multiple notifications updaters.</summary>
    public class MultiUpdater : ISuperSystemSubscriber
    {
        /// <summary>All notifiers utilized by this MultiUpdater.</summary>
        private List<ISuperSystemSubscriber> notifiers = new List<ISuperSystemSubscriber>();

        /// <summary>Initializes a new instance of the MultiUpdater class.</summary>
        /// <param name="notifiers">All the notifiers that this MultiUpdater will utilize.</param>
        public MultiUpdater(params ISuperSystemSubscriber[] notifiers)
        {
            this.notifiers.AddRange(notifiers);
        }

        /// <summary>Finalizes an instance of the MultiUpdater class.</summary>
        ~MultiUpdater()
        {
            this.Dispose();
        }

        /// <summary>Dispose of all resources utilized by this MultiUpdater.</summary>
        public void Dispose()
        {
            if (this.notifiers == null)
            {
                return;
            }

            foreach (var notifier in this.notifiers)
            {
                notifier.Dispose();
            }

            this.notifiers = null;
        }

        /// <summary>Notify user of the specified message via logging to a text file.</summary>
        /// <param name="message">The message to pass along.</param>
        public void Notify(string message)
        {
            if (this.notifiers == null)
            {
                return;
            }

            foreach (var notifier in this.notifiers)
            {
                notifier.Notify(message);
            }
        }
    }
}