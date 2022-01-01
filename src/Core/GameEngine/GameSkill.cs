﻿//-----------------------------------------------------------------------------
// <copyright file="GameSkill.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Newtonsoft.Json;
using WheelMUD.Interfaces;
using WheelMUD.Utilities.Interfaces;

namespace WheelMUD.Core
{
    /// <summary>Holds data for an individual rule-set skill.</summary>
    public class GameSkill : IPersistsWithPlayer, INamed
    {
        /// <summary>Gets or sets the name.</summary>
        /// <value>The name of this skill.</value>
        public string Name { get; set; }

        /// <summary>Gets or sets the description for this skill.</summary>
        /// <value>The description of this game skill.</value>
        public string Description { get; set; }

        /// <summary>Gets or sets the controlling <see cref="Thing"/> attribute.</summary>
        public string ControllingAttribute { get; set; }

        /// <summary>Gets or sets the player thing for this skill.</summary>
        [JsonIgnore]
        public Thing PlayerThing { get; set; }

        /// <summary>Called when a parent has just been assigned to this skill. (Refer to PlayerThing)</summary>
        public virtual void OnAddSkill()
        {
        }

        /// <summary>Called when the current parent of this skill is about to be removed. (Refer to PlayerThing.)</summary>
        public virtual void OnRemoveSkill()
        {
        }
    }
}