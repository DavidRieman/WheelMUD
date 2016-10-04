//-----------------------------------------------------------------------------
// <copyright file="TestThingID.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: August 2011 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests
{
    /// <summary>Helpers for creating temporary ID for test Things, without having to hit any DB.</summary>
    public static class TestThingID
    {
        /// <summary>Synchronization locking object.</summary>
        private static object lockObject = new object();

        /// <summary>The current ID to be assigned to new Things to guarantee unique test IDs.</summary>
        private static int currentID = 0;
        
        /// <summary>Generate the next temporary per-execution unique ID for a test Thing.</summary>
        /// <param name="thingType">The type of thing (like "testroom").</param>
        /// <returns>A temporary per-execution unique ID to use for a test Thing.</returns>
        public static string Generate(string thingType)
        {
            // Lock, just in case multiple tests or whatnot can try to generate an ID at the same time, etc.
            lock (lockObject)
            {
                currentID++;
                return string.Format("{0}/{1}", thingType, currentID.ToString());
            }
        }
    }
}