//-----------------------------------------------------------------------------
// <copyright file="RoomTypes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    /// <summary>Room types.</summary>
    /// <remarks>
    /// TODO: Attach these to RoomBehavior (defaulting as None)? Revise or consider a few common needs as booleans, etc?
    /// TODO: These could be used to determine if weather events are printed for players at the location, etc.
    /// </remarks>
    [Flags]
    public enum RoomType : uint
    {
        /// <summary>Lack of any room type.</summary>
        None = 0,

        /// <summary>City room type.</summary>
        City = 1,

        /// <summary>Forest room type. Most forests are Deciduous.</summary>
        Forest = 2,

        /// <summary>Plains room type.</summary>
        Plains = 4,

        /// <summary>Jungle room type.</summary>
        Jungle = 8,

        /// <summary>River room type.</summary>
        River = 16,

        /// <summary>Ocean room type.</summary>
        Ocean = 32,

        /// <summary>Cave room type.</summary>
        Cave = 64,

        /// <summary>Snowy room type.</summary>
        Snowy = 128,

        /// <summary>Hills room type.</summary>
        Hills = 256,

        /// <summary>Mountains room type.</summary>
        Mountains = 512,

        /// <summary>Indoors room type.</summary>
        Indoors = 1024,

        /// <summary>Dry room type.</summary>
        Dry = 2048,

        /// <summary>Humid room type.</summary>
        Humid = 4096,

        /// <summary>Air/Sky room type.</summary>
        Air = 8192,

        /// <summary>Outer space room type.</summary>
        OuterSpace = 16384,

        /// <summary>Underwater room type.</summary>
        Underwater = 32768,

        /// <summary>The surface room type used to denote the surface of something.</summary>
        Surface = 65536,

        /// <summary>The room could go on forever to the player, like a big room or the open sea.</summary>
        Endless = 131072,

        /// <summary>Not as big as Endless, but big.</summary>
        Large = 262144,

        /// <summary>A room built from metal: Indoors | Dry</summary>
        MetalRoom = Indoors | Dry,

        /// <summary>A room built from stone: Indoors | Mountains</summary>
        StoneRoom = Indoors | Mountains,

        /// <summary>A room built from wood: Indoors | Forest</summary>
        WoodRoom = Indoors | Forest,

        /// <summary>Sewer: Indoors | Cave | River</summary>
        Sewer = Indoors | Cave | River,

        /// <summary>A tree branch large enough to walk on:  Forest | Surface</summary>
        TreeSurface = Forest | Surface,

        /// <summary>Alpine biome room type: Mountains | Forest | Snowy</summary>
        Alpine = Mountains | Forest | Snowy,

        /// <summary>Tundra biome room type: Plains | Snowy | Dry</summary>
        Tundra = Plains | Snowy | Dry,

        /// <summary>Chaparral bimoe room type  (Wild West scenery): Dry | Plains | Hills</summary>
        Chaparral = Dry | Plains | Hills,

        /// <summary>Savanna biome room type (African Plains): Dry | Plains</summary>
        Savanna = Dry | Plains,

        /// <summary>Rainforest biome room type (South America): Forest | Humid</summary>
        Rainforest = Forest | Humid,

        /// <summary>Swamp biome room type (wet marsh): Plains | Humid</summary>
        Swamp = Plains | Humid,

        /// <summary>Taiga biome room type (Mountain Forest): Forest | Mountains</summary>
        Taiga = Forest | Mountains
    }
}