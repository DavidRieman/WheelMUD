//-----------------------------------------------------------------------------
// <copyright file="ParentRelationshipType.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Bengecko November 2009
// </summary>
//-----------------------------------------------------------------------------

/* TODO: Currently this is unused, but the idea is that a Thing's Children could be classified this way.
 *       Need to consider whether this is the right approach, or whether this indicates that there should
 *       be something like an "AdornmentBehavior" instead.  I'm leaning towards the latter. -Karak
namespace WheelMUD.Core.Enums
{
    /// <summary>The types of parents that exits in WheelMUD.</summary>
    /// <remarks>
    /// @@@ For now this should mirror the ParentUsage[s] table, until such a time as 
    ///     types are detected/loaded automatically and this enum becomes irrelevant.
    /// </remarks>
    public enum ParentRelationshipType
    {
        /// <summary>The child is contained within the parent object.</summary>
        Containment = 1,

        /// <summary>The child adorns the parent object.</summary>
        Adornment = 2,
    }
}
*/