//-----------------------------------------------------------------------------
// <copyright file="Consciousness.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   An enumeration of possible consciousness states.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>An enumeration of possible consciousness states.</summary>
    public enum Consciousness
    {
        /// <summary>The entity is awake.</summary>
        Awake,

        /// <summary>The entity is asleep.</summary>
        Asleep,

        /// <summary>The entity is unconscious.</summary>
        Unconscious,

        /// <summary>The entity is dead.</summary>
        Dead,

        /// <summary>The entity is in an unknown state.</summary>
        Unknown,
    }
}