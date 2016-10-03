//-----------------------------------------------------------------------------
// <copyright file="SensoryTypeMeasurement.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Fastalanasa
//   Date      : 5/5/2009 7:54:31 AM
//   Purpose   : An enum used for making senses more granular. This holds what
//               measurements are allowed to use when dealing with sensory
//               levels.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Enums
{
    /// <summary>Replace this with this class summary.</summary>
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