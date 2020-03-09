//-----------------------------------------------------------------------------
// <copyright file="UpdateActionsCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ServerHarness
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Main;

    /// <summary>Command to recompose (reload and update) the CommandManager Actions.</summary>
    [ExportServerHarnessCommand(0)]
    public class UpdateActionsCommand : IServerHarnessCommand
    {
        public string Description => "Recomposes the CommandManager system's actions. Game actions performed afterwards will use new code from dropped-in, updated DLLs.";

        public IEnumerable<string> Names => new string[] { "UPDATE-ACTIONS", "UPDATE", "update", "u" };

        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            // @@@ TODO: Test, migrate to file system watcher (at the Application layer) instead?
            CommandManager.Instance.Recompose();
        }
    }
}