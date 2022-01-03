//-----------------------------------------------------------------------------
// <copyright file="MountBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Core;

namespace WarriorRogueMage.Behaviors
{
    /// <summary>A behavior to house mount-related functionality (such as for horses, mules, motorcycles, and so on).</summary>
    public class MountBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the MountBehavior class.</summary>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public MountBehavior(Dictionary<string, object> instanceProperties) : base(instanceProperties)
        {
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            throw new NotImplementedException();
        }
    }
}