// Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is subject to the Microsoft Public License.  All other rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace WheelMUD.Server
{
    /// <summary>A collection of export attributes used for marking server-related classes to be discovered by MEF composition.</summary>
    /// <remarks>
    /// For example, if you want to add a new connection protocol like WebSocket, you can make a class like WebSocketGameConnectionManager
    /// which would have its own exporter class marked with "[ServerExports.GameConnectionManager(0)]".
    /// Exports with a higher "priority" value get priority over exports of the type same name with lower priority. In this way, you can
    /// replace existing managers by choosing the same class name but exporting it with a higher priority number.
    /// </remarks>
    public static class ServerExports
    {
        [MetadataAttribute]
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class GameConnectionManager : ExportAttribute
        {
            public GameConnectionManager(int priority) : base(typeof(GameConnectionManagerExporter))
            {
                Priority = priority;
            }

            /// <summary>Initializes a new instance of the GameConnectionManager attribute from metadata.</summary>
            /// <param name="metadata">The metadata dictionary provided by MEF.</param>
            public GameConnectionManager(IDictionary<string, object> metadata) : base(typeof(GameConnectionManagerExporter))
            {
                if (metadata.TryGetValue(nameof(Priority), out object priority))
                {
                    Priority = Convert.ToInt32(priority);
                }
            }

            public int Priority { get; set; }
        }
    }
}
