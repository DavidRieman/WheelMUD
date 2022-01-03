//-----------------------------------------------------------------------------
// <copyright file="ImmobileEffect.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace WheelMUD.Effects
{
    /// <summary>An immobilization effect.</summary>
    public class ImmobileEffect : Effect
    {
        /// <summary>Initializes a new instance of the ImmobileEffect class.</summary>
        public ImmobileEffect()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the ImmobileEffect class.</summary>
        /// <param name="instanceID">ID of the effect instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this effect instance.</param>
        public ImmobileEffect(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceID;
        }

        /// <summary>Sets the default properties of this effect instance.</summary>
        protected override void SetDefaultProperties()
        {
            throw new NotImplementedException();
        }
    }
}