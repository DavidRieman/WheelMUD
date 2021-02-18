//-----------------------------------------------------------------------------
// <copyright file="SensoryTypeMeasurement.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    /// <summary>An enum used for making senses more granular.</summary>
    /// <remarks>
    /// This holds what measurements are allowed to use when dealing with sensory levels.
    /// TODO: This is probably too sciency for random builders and developers to have to think about. An abstract scale
    ///       like 0-100 for "how easy to perceive" with a given sense type (hearing/vision and so on) should suffice
    ///       for the basic Core systems.
    /// </remarks>
    public enum SensoryTypeMeasurement
    {
        /// <summary>Used for debugging.</summary>
        Debug,

        /// <summary>How loud a sound is.</summary>
        Decibel,

        /// <summary>How bright something is.</summary>
        Lumen,

        /// <summary>Used for smell and taste. Both senses use the same measurement.</summary>
        PartsPerMillion,

        /// <summary>Provides tactile feedback to the touch sense.</summary>
        PoundsPerSquareInch
    }
}