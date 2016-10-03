//-----------------------------------------------------------------------------
// <copyright file="RoomTypes.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Enum of accepted room types that are usable.
//   Updated by Feverdream @ 4/24/2010 to use actual flag values and extend enum 
//   for real-earth Biom scenerios.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    
    /// <summary>Room types.</summary>
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

        /// <summary>A room built from metal: RoomType.Indoors | RoomType.Dry</summary>
        MetalRoom = RoomType.Indoors | RoomType.Dry,

        /// <summary>A room built from stone: RoomType.Indoors | RoomType.Mountains</summary>
        StoneRoom = RoomType.Indoors | RoomType.Mountains,

        /// <summary>A room built from wood: RoomType.Indoors | RoomType.Forest</summary>
        WoodRoom = RoomType.Indoors | RoomType.Forest,

        /// <summary>Sewer: RoomType.Indoors | RoomType.Cave | RoomType.River</summary>
        Sewer = RoomType.Indoors | RoomType.Cave | RoomType.River,

        /// <summary>A tree branch large enough to walk on:  RoomType.Forest | RoomType.Surface</summary>
        TreeSurface = RoomType.Forest | RoomType.Surface,

        /// <summary>Alpine Biom room type: RoomType.Mountains | RoomType.Forest | RoomType.Snowy</summary>
        Alpine = RoomType.Mountains | RoomType.Forest | RoomType.Snowy,

        /// <summary>Tundra Biom room type: RoomType.Plains | RoomType.Snowy | RoomType.Dry</summary>
        Tundra = RoomType.Plains | RoomType.Snowy | RoomType.Dry,

        /// <summary>Chaparral Biom room type  (Wild West scenery): RoomType.Dry | RoomType.Plains | RoomType.Hills</summary>
        Chaparral = RoomType.Dry | RoomType.Plains | RoomType.Hills,

        /// <summary>Savanna Biom room type (African Plains): RoomType.Dry | RoomType.Plains</summary>
        Savanna = RoomType.Dry | RoomType.Plains,

        /// <summary>Rainforest Biom room type (South America): RoomType.Forest | RoomType.Humid</summary>
        Rainforest = RoomType.Forest | RoomType.Humid,

        /// <summary>Swamp Biom room type (wet marsh): RoomType.Plains | RoomType.Humid</summary>
        Swamp = RoomType.Plains | RoomType.Humid,

        /// <summary>Taiga Biom room type (Mountain Forest): RoomType.Forest | RoomType.Mountains</summary>
        Taiga = RoomType.Forest | RoomType.Mountains
    }
}