//-----------------------------------------------------------------------------
// <copyright file="ExportGameRuleAttribute.cs" company="WheelMUD Development Team">
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
    using WheelMUD.Rules;

    /// <summary>An [ExportGameRule] attribute to mark GameRules for export through MEF.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportGameRuleAttribute : ExportAttribute
    {
        /// <summary>Initializes a new instance of the ExportGameRuleAttribute class.</summary>
        public ExportGameRuleAttribute()
            : base(typeof(GameRule))
        {
        }
    }
}