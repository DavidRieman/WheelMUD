﻿//-----------------------------------------------------------------------------
// <copyright file="ExportGameModifierAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;

namespace WheelMUD.Core
{
    /// <summary>An [ExportGameModifier] attribute to mark GameModifiers for export through MEF.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportGameModifierAttribute : ExportAttribute
    {
        /// <summary>Initializes a new instance of the ExportGameModifierAttribute class.</summary>
        public ExportGameModifierAttribute()
            : base(typeof(GameModifier))
        {
        }
    }
}