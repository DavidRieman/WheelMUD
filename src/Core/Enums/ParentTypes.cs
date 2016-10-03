//-----------------------------------------------------------------------------
// <copyright file="ParentTypes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 7/3/2009 6:56:40 PM
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Enums
{
    /// <summary>The types of parents that exits in WheelMUD.</summary>
    /// <remarks>
    /// @@@ For now this should mirror the ParentType[s] table, until such a time as 
    ///     types are detected/loaded automatically and this enum becomes irrelevant.
    /// </remarks>
    public enum ParentTypes
    {
        /// <summary>The parent is a room.</summary>
        Room = 1,

        /// <summary>The parent is a player.</summary>
        Player = 2,

        /// <summary>The parent is a item.</summary>
        Item = 3,

        /// <summary>The parent is a mob.</summary>
        Mob = 4,

        /// <summary>The parent is a effect.</summary>
        Effect = 5,

        /// <summary>The parent is a behavior.</summary>
        Behavior = 6,

        /// <summary>The parent is a currency.</summary>
        Currency = 7,

        /// <summary>The parent is a Exit End. (Added for completeness.)</summary>
        ExitEnd = 8,

        /// <summary>The parent is a Exit. (Added for completeness.)</summary>
        Exit = 9,

        /// <summary>The parent is a Door. (Added for completeness.)</summary>
        Door = 10,

        /// <summary>The parent is a DoorLock. (Added for completeness.)</summary>
        DoorLock = 11,

        /// <summary>The parent is a ContainerLock. (Added for completeness.)</summary>
        ContainerLock = 12,

        /// <summary>The parent is a Mobile. (Added for completeness.)</summary>
        Mobile = 13,

        /// <summary>The parent is a Area. (Added for completeness.)</summary>
        Area = 14,

        /// <summary>The parent is a World. (Added for completeness.)</summary>
        World = 15,

        /// <summary>The parent is a side of a door.</summary>
        DoorSide = 16,
    }
}