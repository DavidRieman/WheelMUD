//-----------------------------------------------------------------------------
// <copyright file="RunTestsCommand.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace ServerHarness.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using WheelMUD.Main;

    public class DebugExploreDocumentsCommand : IServerHarnessCommand
    {
        private readonly string[] names = { "DEBUGEXPLORE", "DEBUG-EXPLORE", "DOCS", "DOCUMENTS" };

        public IEnumerable<string> Names
        {
            get { return this.names; }
        }

        public void Execute(Application app, MultiUpdater display, string[] words)
        {
            WheelMUD.Data.Helpers.DebugExploreDocuments();
        }
    }
}