//-----------------------------------------------------------------------------
// <copyright file="ExportGameStatAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: June 2011 by Karak.
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>An [ExportGameStat] attribute to mark GameStats for export through MEF.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportGameStatAttribute : ExportAttribute
    {
        /// <summary>Initializes a new instance of the ExportGameStatAttribute class.</summary>
        public ExportGameStatAttribute()
            : base(typeof(GameStat))
        {
        }
    }
}