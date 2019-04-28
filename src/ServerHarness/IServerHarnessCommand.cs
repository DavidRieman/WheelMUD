//-----------------------------------------------------------------------------
// <copyright file="IServerHarnessCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
// <summary>
//   Purpose: Interface for test harness commands.
// </summary>
//-----------------------------------------------------------------------------

namespace ServerHarness
{
    using System.Collections.Generic;
    using WheelMUD.Main;

    public interface IServerHarnessCommand
    {
        IEnumerable<string> Names { get; }

        void Execute(Application app, MultiUpdater display, string[] words);
    }
}