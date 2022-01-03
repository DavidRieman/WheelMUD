﻿//-----------------------------------------------------------------------------
// <copyright file="SimpleTestBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Core;

namespace WheelMUD.Tests.Behaviors
{
    /// <summary>Simple implementation of Behavior, for testing purposes.</summary>
    public class SimpleTestBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the <see cref="SimpleTestBehavior"/> class.</summary>
        public SimpleTestBehavior()
            : base(null)
        {
        }

        /// <summary>Sets default properties for this behavior.</summary>
        protected override void SetDefaultProperties()
        {
        }
    }
}