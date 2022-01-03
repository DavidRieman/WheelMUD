//-----------------------------------------------------------------------------
// <copyright file="IServerHarnessCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ServerHarness
{
    using System.Collections.Generic;
    using WheelMUD.Main;

    /// <summary>Interface for administrative server harness commands.</summary>
    public interface IServerHarnessCommand
    {
        /// <summary>Gets a brief description of the command, for display in command help lists.</summary>
        string Description { get; }

        /// <summary>Gets the set of aliases which will invoke this command, with the first one as the primary display name.</summary>
        IEnumerable<string> Names { get; }

        void Execute(Application app, MultiUpdater display, string[] words);
    }
}