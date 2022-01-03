//-----------------------------------------------------------------------------
// <copyright file="DynamicServerHarnessCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ServerHarness
{
    using System;
    using System.Collections.Generic;
    using WheelMUD.Main;

    public class DynamicServerHarnessCommand : IServerHarnessCommand
    {
        private readonly Action command;

        public DynamicServerHarnessCommand(Action command, string[] names, string description)
        {
            Names = names;
            Description = description;
            this.command = command;
        }

        public string Description { get; private set; }

        public IEnumerable<string> Names { get; private set; }

        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            command();
        }
    }
}
