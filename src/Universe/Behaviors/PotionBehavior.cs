﻿//-----------------------------------------------------------------------------
// <copyright file="PotionBehavior.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using WheelMUD.Core;

namespace WheelMUD.Universe
{
    /// <summary>A potion item behavior adds the ability to quaff/sip from an item.</summary>
    public class PotionBehavior : Behavior
    {
        /// <summary>Initializes a new instance of the PotionBehavior class.</summary>
        public PotionBehavior()
            : base(null)
        {
        }

        /// <summary>Initializes a new instance of the PotionBehavior class.</summary>
        /// <param name="instanceID">ID of the behavior instance.</param>
        /// <param name="instanceProperties">Dictionary of properties to spawn this behavior instance with, if any.</param>
        public PotionBehavior(long instanceID, Dictionary<string, object> instanceProperties)
            : base(instanceProperties)
        {
            ID = instanceID;
        }

        /// <summary>Gets or sets the maximum sips this potion can contain.</summary>
        public int MaxSips { get; set; }

        /// <summary>Gets or sets the number of sips left in this potion.</summary>
        public int SipsLeft { get; set; }

        /// <summary>Gets or sets the potion type of the effect of this potion.</summary>
        public string PotionType { get; set; }

        /// <summary>Gets or sets the modifier (strength) of the effect of this potion.</summary>
        public int Modifier { get; set; }

        /// <summary>Gets or sets the duration of the effect of this potion.</summary>
        public TimeSpan Duration { get; set; }

        /// <summary>Have the specified entity drink the potion.</summary>
        /// <param name="entity">The entity to drink the potion.</param>
        public void Drink(Thing entity)
        {
            if (SipsLeft <= 0)
            {
                return;
            }

            /* TODO: Turn all Effects into Behaviors with Duration and re-implement.
            StatEffect effect;

            if (entity.Effects.TryCreateEffect<StatEffect>(entity, out effect))
            {
                effect.Modifier = Modifier;
                effect.Name = PotionType;
                effect.Apply(Duration);
            }
            */
            SipsLeft--;

            ////Save(); TODO: Bring back as Save from base class?
        }

        /// <summary>Sets the default properties of this behavior instance.</summary>
        protected override void SetDefaultProperties()
        {
            MaxSips = 5;
            SipsLeft = 2;
            Duration = new TimeSpan(0, 0, 15);
            Modifier = 100;
            PotionType = "health";
        }
    }
}