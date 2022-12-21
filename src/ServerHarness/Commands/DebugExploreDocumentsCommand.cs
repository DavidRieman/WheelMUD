﻿//-----------------------------------------------------------------------------
// <copyright file="DebugExploreDocumentsCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Collections.Generic;
using WheelMUD.Data;
using WheelMUD.Main;

namespace ServerHarness
{
    [ExportServerHarnessCommand(0)]
    public class DebugExploreDocumentsCommand : IServerHarnessCommand
    {
        public string Description => "Opens the configured document database tool for manual data exploration and manipulation.";

        public IEnumerable<string> Names => new string[] { "DB", "DOCS", "DOCUMENTS", "DEBUGEXPLORE", "DEBUG-EXPLORE",  };

        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            Helpers.DebugExploreDocuments();
        }
    }
}