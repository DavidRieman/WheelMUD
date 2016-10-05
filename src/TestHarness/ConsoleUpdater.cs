//-----------------------------------------------------------------------------
// <copyright file="ConsoleUpdater.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Purpose: Console Updater.
// </summary>
//-----------------------------------------------------------------------------

namespace TestHarness
{
    using System;
    using WheelMUD.Interfaces;

    /// <summary>Console Updater.</summary>
    public class ConsoleUpdater : ISuperSystemSubscriber
    {
        /// <summary>Notify user of the specified message.</summary>
        /// <param name="message">The message to pass along.</param>
        public void Notify(string message)
        {
            Console.WriteLine(message);
            Console.Write("> ");
        }

        /// <summary>Dispose of any resources used by this ConsoleUpdater.</summary>
        public void Dispose()
        {
        }
    }
}