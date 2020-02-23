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
        public int Priority { get; set; }
    }
}
