//-----------------------------------------------------------------------------
// <copyright file="ServerHarnessCommands.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ServerHarness
{
    using System.Collections.Generic;
    using System.Linq;

    public class ServerHarnessCommands
    {
        static ServerHarnessCommands()
        {
            // TODO: Compose, per below (potentially using DefaultComposer).
        }

        // TODO: GitHub #58: Use MEF to compose implementers of IServerHarnessCommand to keep this automatically up to date.
        private static IServerHarnessCommand[] ComposedCommands => new IServerHarnessCommand[]
        {
            new HelpCommand(), new UpdateActionsCommand(), new RunTestsCommand(), new DebugExploreDocumentsCommand()
        };

        public static List<DynamicServerHarnessCommand> DynamicCommands { get; private set; } = new List<DynamicServerHarnessCommand>();

        public static IServerHarnessCommand[] AllCommands
        {
            get { return ComposedCommands.Union(DynamicCommands).ToArray(); }
        }
    }
}
