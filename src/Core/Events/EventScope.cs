//-----------------------------------------------------------------------------
// <copyright file="EventScope.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: January 2012 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Events
{
    /// <summary>The applicable scope for an event.</summary>
    public enum EventScope
    {
        /// <summary>The event is to be broadcast from the current thing's parent(s) and down through all children.</summary>
        ParentsDown,

        /// <summary>The event is to be broadcast from the current thing and down through all children.</summary>
        SelfDown,
    }
}