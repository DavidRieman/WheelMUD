//-----------------------------------------------------------------------------
// <copyright file="ExportActionAttribute.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Created: November 2010 by Karak.
//   http://codebetter.com/blogs/glenn.block/archive/2009/12/04/building-hello-mef-part-ii-metadata-and-why-being-lazy-is-a-good-thing.aspx
// </summary>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.ComponentModel.Composition;
    using WheelMUD.Actions;

    /// <summary>An attribute to export GameActions with.</summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportGameActionAttribute : ExportAttribute
    {
        /// <summary>Initializes a new instance of the ExportGameActionAttribute class.</summary>
        public ExportGameActionAttribute()
            : base(typeof(GameAction))
        {
        }
    }
}