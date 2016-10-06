//-----------------------------------------------------------------------------
// <copyright file="UpdateActionsCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Command to recompose (reload and update) the CommandManager Actions.
// </summary>
//-----------------------------------------------------------------------------

namespace TestHarness.Commands
{
    using System.Collections.Generic;
    using WheelMUD.Core;
    using WheelMUD.Main;

    public class UpdateActionsCommand : ITestHarnessCommand
    {
        private readonly string[] names = { "UPDATE-ACTIONS", "UPDATE", "update", "u" };

        public IEnumerable<string> Names
        {
            get { return names; }
        }

        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            // @@@ TODO: Test, migrate to file system watcher (at the Application layer) instead?
            CommandManager.Instance.Recompose();
        }
    }
}