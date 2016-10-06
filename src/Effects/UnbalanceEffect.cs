//-----------------------------------------------------------------------------
// <copyright file="UnbalanceEffect.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An Unbalance effect.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Effects
{
    using System.Collections.Generic;

    /// <summary>An Unbalance effect.</summary>
    public class UnbalanceEffect : Effect
    {
        /// <summary>Initializes a new instance of the UnbalanceEffect class.</summary>
        public UnbalanceEffect()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the UnbalanceEffect class.</summary>
        /// <param name="instanceID">ID of the effect instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this effect instance.</param>
        public UnbalanceEffect(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Sets the default properties of this effect instance.</summary>
        protected override void SetDefaultProperties()
        {
            ////throw new NotImplementedException();
        }
    }
}