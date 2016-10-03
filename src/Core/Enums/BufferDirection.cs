//-----------------------------------------------------------------------------
// <copyright file="BufferDirection.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Consumable types.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Enums
{
    /// <summary>Consumable types.</summary>
    public enum BufferDirection
    {
        /// <summary>Get the next set of data.</summary>
        Forward = 0,

        /// <summary>Get the prior set of data.</summary>
        Backward = 1,

        /// <summary>Repeat the last set of data.</summary>
        Repeat = 2,

        /// <summary>Moves forward sending all data.</summary>
        ForwardAllData = 3
    }
}