//-----------------------------------------------------------------------------
// <copyright file="SensoryType.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;

namespace WheelMUD.Core
{
    /// <summary>Sensory types, for contextual perception systems.</summary>
    /// <remarks>TODO: Consider leaving very few basic common senses here; split others out as reserved value comments or hosting in Reference Implementations as example extensions.</remarks>
    [Flags]
    public enum SensoryType : uint
    {
        /// <summary>Lack of any sense.</summary>
        None = 0,

        /// <summary>Sight, you're using it to read this.</summary>
        Sight = 1,

        /// <summary>Hearing, the sense to hear by.</summary>
        Hearing = 1 << 1,

        /// <summary>Able to touch the world around, this would also be able to feel earthquakes, etc.</summary>
        Touch = 1 << 2,

        /// <summary>Able to taste.</summary>
        Taste = 1 << 3,

        /// <summary>Able to smell.</summary>
        Smell = 1 << 4,

        /// <summary>A special Debug sense.</summary>
        /// <remarks>TODO: REMOVE NOW? Also remove the MessagePrefix concept in general? All perception strings should be designed from the perspective of the first successful sense.</remarks>
        Debug = 1 << 5,

        /// <summary>All the senses (including future or game-specific extensions).</summary>
        All = 0xFFFFFFFF,

        // Note that Core sensory events will generally be built only against the most basic sensory types, in order to keep scope in check,
        // as having to consider everything from NightVision, HeatVision, UltraVioetVision, XRayVision and so on is way too much to design
        // output for when trying to just decribe an action's result to potential witnesses. However, to their own ends...
        // Game implementations can extend this without modifying Core code, by providing their own constants that cast to SensoryType.
        //   public const SensoryType SensoryTypeSonar = (SensoryType)(1ul << 14);
        //   public const SensoryType SensoryTypeQuantumScannerTech = (SensoryType)(1ul << 15);
        // Keep in mind that Core may use many SensoryMessages built against the original senses, so if providing custom new-sense responses
        // is something you want from those, you may want to export your own versions of those commands with these customizations in place.
    }
}