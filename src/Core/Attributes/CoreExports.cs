//-----------------------------------------------------------------------------
// <copyright file="CoreExports.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace WheelMUD.Core
{
    /// <summary>A collection of export attributes used for marking a class to be discovered by WheelMUD MEF composition.</summary>
    /// <remarks>
    /// For example, if you want to replace the default core CommandManager system entirely, you can build a class which
    /// implements ISystem and has its own "CommandManagerExporter : SystemExporter", marked with "[CoreExports.System(1)]".
    /// Exports with a higher "priority" value always get priority over exports of the same name with lower priority.
    /// This class groups the most "core" systemic export attributes. See also CreatorExports and RendererExports for more
    /// composition export attributes related to those areas.
    /// </remarks>
    public static class CoreExports
    {
        public class GameAction : BasePrioritizedExportAttribute
        {
            public GameAction(int priority) : base(priority, typeof(Core.GameAction)) { }
            public GameAction(IDictionary<string, object> metadata) : base(metadata) { }
        }

        public class System : BasePrioritizedExportAttribute
        {
            public System(int priority) : base(priority, typeof(SystemExporter)) { }
            public System(IDictionary<string, object> metadata) : base(metadata) { }
        }
    }
}
