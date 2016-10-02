//-----------------------------------------------------------------------------
// <copyright file="ExportTalentAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team. See LICENSE.txt. This file is
//   subject to the Microsoft Public License. All other rights reserved.
// </copyright>
// <summary>
//   Created by: Karak
//   Date      : June 5, 2011
// </summary>
//-----------------------------------------------------------------------------

namespace WarriorRogueMage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using WheelMUD.Utilities;

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportTalentAttribute : ExportAttribute
    {
        public ExportTalentAttribute()
            : base(typeof(Talent))
        {
        }

        public ExportTalentAttribute(IDictionary<string, object> metadata)
        {
            PropertyTools.SetProperties(this, metadata);
        }
    }
}