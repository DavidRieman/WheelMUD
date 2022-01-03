﻿//-----------------------------------------------------------------------------
// <copyright file="CurrencyBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Universe
{
    /// <summary>CurrencyBehavior denotes an object that can be used as currency.</summary>
    public class CurrencyBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the CurrencyBehavior class.</summary>
        public CurrencyBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the CurrencyBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this behavior instance.</param>
        public CurrencyBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceID;
        }

        /// <summary>Sets the default properties of this effect instance.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}