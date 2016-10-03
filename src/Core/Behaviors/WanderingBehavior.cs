//-----------------------------------------------------------------------------
// <copyright file="WanderingBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 6/12/2011
//   Purpose   : Behavior that allows NPC to wander randomly, thus becoming a mobile.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Collections.Generic;

    /// <summary>Behavior that allows NPC to wander randomly, thus becoming a mobile.</summary>
    public class WanderingBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the <see cref="WanderingBehavior"/> class.</summary>
        public WanderingBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="WanderingBehavior"/> class.</summary>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public WanderingBehavior(Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            // Nothing so far.
        }
    }
}