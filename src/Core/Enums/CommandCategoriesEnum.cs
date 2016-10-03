//-----------------------------------------------------------------------------
// <copyright file="CommandCategoriesEnum.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Interfaces for Entities.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Attributes
{
    using System;

    /// <summary>Categories that actions can be filed under.</summary>
    [Flags]
    public enum CommandCategory
    {
        /// <summary>No categories.</summary>
        None = 0,

        /// <summary>Action is classified as an activity action ("push").</summary>
        Activities = 1,

        /// <summary>Action is classified as an administration action ("goto").</summary>
        Admin = 2,

        /// <summary>Action is classified as a combat action ("attack").</summary>
        Combat = 4,

        /// <summary>Action is classified as a commercial action ("buy").</summary>
        Commercial = 8,

        /// <summary>Action is classified as a communication action ("yell").</summary>
        Communicate = 16,

        /// <summary>Action is classified as a configuration action ("alias").</summary>
        Configure = 32,

        /// <summary>Action is classified as an item interaction action ("drink").</summary>
        Item = 64,

        /// <summary>Action is classified as an informational action ("inventory").</summary>
        Inform = 128,

        /// <summary>Action is classified as a player action ("friend add").</summary>
        Player = 256,

        /// <summary>Action is classified as a temporary action ("ThunderClap").</summary>
        Temporary = 1024,

        /// <summary>Action is classified as a travel action ("move").</summary>
        Travel = 2048
    }
}