//-----------------------------------------------------------------------------
// <copyright file="Sense.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using WheelMUD.Core.Enums;

    /// <summary>A sense as used for perception.</summary>
    public class Sense
    {
        /// <summary>Gets or sets the name of this sense.</summary>
        /// <value>The name of this sense.</value>
        public SensoryType SensoryType { get; set; }

        /// <summary>Gets or sets the high threshold.</summary>
        /// <value>The high threshold.</value>
        public int HighThreshold { get; set; }

        /// <summary>Gets or sets the low threshold.</summary>
        /// <value>The low threshold.</value>
        public int LowThreshold { get; set; }

        /// <summary>Gets or sets a value indicating whether this <see cref="Sense"/> is enabled.</summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets the message prefix.</summary>
        /// <value>The message prefix.</value>
        public string MessagePrefix { get; set; }

        /// <summary>Gets or sets the measurement.</summary>
        /// <value>The measurement.</value>
        public SensoryTypeMeasurement Measurement { get; set; }
    }
}