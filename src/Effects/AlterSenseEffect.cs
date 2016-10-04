//-----------------------------------------------------------------------------
// <copyright file="AlterSenseEffect.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   An effect which alters it's parent's senses.
//   Created: August 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Effects
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Core;

    /// <summary>An effect which alters a thing's ability to sense (positively or negatively).</summary>
    /// <remarks>
    /// @@@ TODO: Add a concept of 'current' values of sense capabilities etc to SenseManager or whatnot (not 
    ///  just the base capabilities nor should we ever touch those base values) - the current senses manager 
    ///  could subscribe to events which add/remove things like AlterSenseEffect and recalculate accordingly.
    /// </remarks>
    public class AlterSenseEffect : Effect
    {
        /// <summary>Initializes a new instance of the AlterSenseEffect class.</summary>
        public AlterSenseEffect()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the AlterSenseEffect class.</summary>
        /// <param name="instanceID">ID of the effect instance.</param>
        /// <param name="instanceProperties">The dictionary of propertyNames-propertyValues for this effect instance.</param>
        public AlterSenseEffect(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            this.ID = instanceID;
        }

        /// <summary>Gets or sets the type of sense which is to be altered.</summary>
        public SensoryType SensoryType { get; set; }

        /// <summary>Gets or sets the amount to alter the sense by.</summary>
        public int AlterAmount { get; set; }

        /// <summary>Sets the default properties of this effect instance.</summary>
        protected override void SetDefaultProperties()
        {
            this.SensoryType = SensoryType.None;
            this.AlterAmount = 0;
            this.Duration = new TimeSpan(0, 0, 30);
        }
    }
}