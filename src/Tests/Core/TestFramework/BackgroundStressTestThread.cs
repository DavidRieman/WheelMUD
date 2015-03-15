//-----------------------------------------------------------------------------
// <copyright file="BackgroundStressTestThread.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: March 2014 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Tests
{
    using System;
    using System.Linq;

    /// <summary>A background test thread, for stress testing purposes.</summary>
    public class BackgroundStressTestThread : BackgroundTestThread
    {
        /// <summary>Initializes a new instance of the BackgroundStressTestThread class.</summary>
        /// <param name="endTime">When to finish running the stress thread.</param>
        /// <param name="testAction">The action to perform, repeatedly, until the endTime.</param>
        public BackgroundStressTestThread(DateTime endTime, Action testAction)
            : base(() =>
            {
                while (DateTime.Now < endTime)
                {
                    testAction();
                }
            })
        {
        }
    }
}