﻿//-----------------------------------------------------------------------------
// <copyright file="Talent.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WheelMUD.Core;

    /// <summary>Class to represent the basic Talent in the Warrior, Rogue, and Mage game system.</summary>
    public class Talent : IEquatable<Talent>
    {
        // TODO: Talents and such should not make permanent modifications to base stats. This is being redesigned.
        //       Perhaps we will end up with a system to register class instances to an Active Modifiers list, to
        //       all get called on for turning a "base" value into "current" value any time we're about to use it.
        //       See: https://github.com/WheelMud/WheelMUD/issues/49

        /// <summary>Initializes a new instance of the <see cref="Talent"/> class.</summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="talentType">Type of the talent.</param>
        public Talent(string name, string description, TalentType talentType)
        {
            this.Name = name;
            this.Description = description;
            this.TalentType = talentType;
        }

        /// <summary>Initializes a new instance of the <see cref="Talent"/> class.</summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="talentType">Type of the talent.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="rules">The rules.</param>
        public Talent(string name, string description, TalentType talentType, Thing parent, params string[] rules)
        {
            this.Name = name;
            this.Description = description;
            this.TalentType = talentType;
            this.Rules = rules.ToList();
            this.PlayerThing = parent;
        }

        /// <summary>Initializes a new instance of the <see cref="Talent"/> class.</summary>
        /// <param name="parent">The parent.</param>
        public Talent(Thing parent)
        {
            this.PlayerThing = parent;
        }

        /// <summary>Gets or sets the player thing.</summary>
        [JsonIgnore]
        public Thing PlayerThing { get; set; }

        /// <summary>Gets the description.</summary>
        public string Description { get; private set; }

        /// <summary>Gets the name of this talent.</summary>
        public string Name { get; private set; }

        /// <summary>Gets the type of this talent.</summary>
        public TalentType TalentType { get; private set; }

        /// <summary>Gets the rules.</summary>
        public List<string> Rules { get; private set; }

        /// <summary>Called when a parent has just been assigned to this talent. (Refer to this.PlayerThing.)</summary>
        public virtual void OnAddTalent()
        {
        }

        /// <summary>Called when the current parent of this talent is about to be removed. (Refer to this.PlayerThing.)</summary>
        public virtual void OnRemoveTalent()
        {
        }

        /// <summary>Called when the game engine, or other systems, need to activate the talent.</summary>
        /// <remarks>Some talents are not automatic and can only be used/activated in certain situations.</remarks>
        public virtual void OnActivateTalent()
        {
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Talent other)
        {
            if (this.TalentType == other.TalentType && this.Name == other.Name)
            {
                return true;
            }

            return false;
        }
    }
}