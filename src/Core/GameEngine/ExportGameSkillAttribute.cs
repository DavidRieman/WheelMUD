//-----------------------------------------------------------------------------
// <copyright file="ExportGameSkillAttribute.cs" company="WheelMUD Development Team">
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

    /// <summary>An [ExportGameSkill] attribute to mark GameSkills for export through MEF.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportGameSkillAttribute : ExportAttribute
    {
        /// <summary>Initializes a new instance of the ExportGameSkillAttribute class.</summary>
        public ExportGameSkillAttribute()
            : base(typeof(GameSkill))
        {
        }
    }
}