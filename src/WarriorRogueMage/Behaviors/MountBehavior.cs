//-----------------------------------------------------------------------------
// <copyright file="MountBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Behavior that deals with a mounts in the game, i.e. Horses, Mules, Ostriches,
//   etc.
//   Author: Fastalanasa
//   Date: May 12, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage.Behaviors
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>A behavior to house mount-related functionality.</summary>
    public class MountBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the MountBehavior class.</summary>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public MountBehavior(Dictionary<string, object> instanceProperties)  : base(instanceProperties)
        {
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            throw new NotImplementedException();
        }
    }
}