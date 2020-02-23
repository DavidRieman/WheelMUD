//-----------------------------------------------------------------------------
// <copyright file="IExportWithPriority.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Interfaces
{
    /// <summary>Helps identify MEF export attributes which adhere to a priority pattern.</summary>
    /// <remarks>Several of the WheelMUD export systems will first sort by highest priority when choosing which of them should be active.</remarks>
    public interface IExportWithPriority
    {
        /// <summary>Gets or sets the priority of the exported instance. Only the highest priority version gets utilized.</summary>
        /// <remarks>
        /// Default exports (those that ship with WheelMUD core libraries) are priority 0, while an individual game system
        /// may export things at priority 100, and one-off versions created specifically for a MUD instance should have a
        /// higher priority than that. You could disable a customized version simply by setting the priority negative.
        /// </remarks>
        public int Priority { get; set; }
    }
}
