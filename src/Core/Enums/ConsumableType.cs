//-----------------------------------------------------------------------------
// <copyright file="ConsumableType.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Consumable types.
//   Updated by Feverdream @ 4/24/2010 to use actual flag values and extend enum for real-world raw resource scenerios.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Enums
{
    using System;

    /// <summary>Consumable types.</summary>
    [Flags]
    public enum ConsumableType
    {
        /// <summary>We have no idea what this is.</summary>
        Unknown = 0,

        /// <summary>Cloth like cotton or silk.</summary>
        Cloth = 1,

        /// <summary>Choppable trees and such are comprised of wood.</summary>
        Wood = 2,

        /// <summary>Leather makes it better.</summary>
        Leather = 4,

        /// <summary>Gold, Iron, Steel</summary>
        Metal = 8,

        /// <summary>Dust, Mountains, Sand.</summary>
        Rock = 16,

        /// <summary>Rock Melted down and reformed into clear sheets.</summary>
        Glass = 32,

        /// <summary>Grass, herbs, etc.</summary>
        Vegitation = 64,

        /// <summary>Meat, bones.</summary>
        Flesh = 128,

        /// <summary>Letters, notes, paper money, deeds to land.</summary>
        Paper = 256,

        /// <summary>Water, Oil, Coffee.</summary>
        Liquid = 512,

        /// <summary>Light, Electricity, Heat</summary>
        Energy = 1024,

        /// <summary>Simple explanation: Air and Ashes mixed.</summary>
        Plastic = 2048,

        /// <summary>Clay cooked into shapes.</summary>
        Ceramic = ConsumableType.Rock | ConsumableType.Metal,

        /// <summary>The hardness of rock, the clearness of glass.</summary>
        Crystal = ConsumableType.Rock | ConsumableType.Glass,
    }
}