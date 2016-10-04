//-----------------------------------------------------------------------------
// <copyright file="ExportGameGenderAttribute.cs" company="WheelMUD Development Team">
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

    /// <summary>An [ExportGameRace] attribute to mark GameRaces for export through MEF.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportGameGenderAttribute : ExportAttribute
    {
        /// <summary>Initializes a new instance of the ExportGameGenderAttribute class.</summary>
        public ExportGameGenderAttribute()
            : base(typeof(GameGender))
        {
        }
    }
}