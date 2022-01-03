//-----------------------------------------------------------------------------
// <copyright file="CreatorExports.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace WheelMUD.Core
{
    /// <summary>A collection of export attributes used for marking creator classes to be discovered by WheelMUD MEF composition.</summary>
    public static class CreatorExports
    {
        /// <summary>Class for exporting an Area Creator for composition into the WheelMUD framework.</summary>
        public class Area : BasePrioritizedExportAttribute
        {
            public Area(int priority) : base(priority, typeof(CreatorDefinitions.Area)) { }
            public Area(IDictionary<string, object> metadata) : base(metadata) { }
        }

        /// <summary>Class for exporting a World Creator for composition into the WheelMUD framework.</summary>
        public class World : BasePrioritizedExportAttribute
        {
            public World(int priority) : base(priority, typeof(CreatorDefinitions.World)) { }
            public World(IDictionary<string, object> metadata) : base(metadata) { }
        }
    }
}
