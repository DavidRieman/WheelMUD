//-----------------------------------------------------------------------------
// <copyright file="RunTestsCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ServerHarness
{
    using System.Collections.Generic;
    using WheelMUD.Data;
    using WheelMUD.Main;

    public class DebugExploreDocumentsCommand : IServerHarnessCommand
    {
        public string Description => "Opens the configured document database tool for manual data exploration and minupulation.";

        public IEnumerable<string> Names => new string[] { "DEBUGEXPLORE", "DEBUG-EXPLORE", "DOCS", "DOCUMENTS" };

        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            Helpers.DebugExploreDocuments();
        }
    }
}