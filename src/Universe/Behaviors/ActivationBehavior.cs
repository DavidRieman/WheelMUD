//-----------------------------------------------------------------------------
// <copyright file="ActivationBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: June 2009 by Karak.
//   Updated: November 2009 by bengecko - AbstractItemBehavior
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Universe
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>An activation item behavior adds the ability to trigger effects upon item activation.</summary>
    public class ActivationBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the <see cref="ActivationBehavior"/> class.</summary>
        public ActivationBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ActivationBehavior"/> class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public ActivationBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            throw new NotImplementedException();
        }
    }
}