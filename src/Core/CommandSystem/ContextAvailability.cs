//-----------------------------------------------------------------------------
// <copyright file="ContextAvailability.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: Decenber 2010 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    /// <summary>The contextual availability, as it applies to a context command, etc.</summary>
    [Flags]
    public enum ContextAvailability
    {
        /// <summary>Available to nobody.</summary>
        ToNone = 0,

        /// <summary>Available to our parent (or parents if we have a MultipleParentsBehavior).</summary>
        ToParent = 1,

        /// <summary>Available to our children.</summary>
        ToChildren = 2,

        /// <summary>Available to our siblings.</summary>
        ToSiblings = 4,

        /// <summary>Available to oruself.</summary>
        ToSelf = 8,
    }
}