//-----------------------------------------------------------------------------
// <copyright file="CommandCategoriesEnum.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Core
{
    /// <summary>Categories that actions can be filed under.</summary>
    [Flags]
    public enum CommandCategory : uint
    {
        /// <summary>No categories.</summary>
        None = 0,

        /// <summary>Action is classified as an activity action ("push").</summary>
        Activities = 1,

        /// <summary>Action is classified as an administration action ("goto").</summary>
        Admin = 1 << 1,

        /// <summary>Action is classified as a combat action ("attack").</summary>
        Combat = 1 << 2,

        /// <summary>Action is classified as a commercial action ("buy").</summary>
        Commercial = 1 << 3,

        /// <summary>Action is classified as a communication action ("yell").</summary>
        Communicate = 1 << 4,

        /// <summary>Action is classified as a configuration action ("alias").</summary>
        Configure = 1 << 5,

        /// <summary>Action is classified as an item interaction action ("drink").</summary>
        Item = 1 << 6,

        /// <summary>Action is classified as an informational action ("inventory").</summary>
        Inform = 1 << 7,

        /// <summary>Action is classified as a player action ("friend add").</summary>
        Player = 1 << 8,

        /// <summary>Action is classified as a temporary action ("ThunderClap").</summary>
        Temporary = 1 << 9,

        /// <summary>Action is classified as a travel action ("move").</summary>
        Travel = 1 << 10
    }
}