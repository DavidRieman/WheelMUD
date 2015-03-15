//-----------------------------------------------------------------------------
// <copyright file="SensoryType.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Enum of accepted Senses that are usable.
//   Edited by Feverdream @ 4/24/2010 to add missing XRay Vision and Magnified Vision values.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;

    /// <summary>Sensory types.</summary>
    /// <remarks>@@@ TODO: Sonar? Extensibility?</remarks>
    [Flags]
    public enum SensoryType
    {
        /// <summary>Lack of any sense.</summary>
        None = 0,

        /// <summary>Sight, you're using it to read this.</summary>
        Sight = 1,

        /// <summary>Hearing, the sense to hear by.</summary>
        Hearing = 2,

        /// <summary>Able to touch the world around, this would also be able to feel earthquakes, etc.</summary>
        Touch = 4,

        /// <summary>Able to taste.</summary>
        Taste = 8,

        /// <summary>Able to smell.</summary>
        Smell = 16,

        /// <summary>Able to see in the dark.</summary>
        NightVision = 32,

        /// <summary>Heat vision</summary>
        HeatVision = 64,

        /// <summary>Ultraviolet vision</summary>
        UltraViolet = 128,

        /// <summary>Magnified vision, to see small things or things before unseen to the naked eye.</summary>
        MagnifiedVision = 512,

        /// <summary>X-Ray vision</summary>
        XRay = 1024,

        /// <summary>A special Debug sense.</summary>
        Debug = 2048,

        /// <summary>All the senses.</summary>
        All = 4096,
    }
}