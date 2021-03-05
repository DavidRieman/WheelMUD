//-----------------------------------------------------------------------------
// <copyright file="DefaultHelpTopicsRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;

namespace WheelMUD.Core
{
    [RendererExports.HelpTopics(0)]
    public class DefaultHelpTopicsRenderer : RendererDefinitions.HelpTopics
    {
        public override string Render(TerminalOptions terminalOptions)
        {
            var ab = new OutputBuilder(terminalOptions);
            ab.AppendLine("TODO: LIST OUT ALL HELP TOPICS FOUND VIA HelpManager.");
            ab.AppendLine();
            ab.AppendLine("You can also use the \"commands\" command to list out commands, and you can get help");
            ab.AppendLine("for a specific command with \"help <command name>\"");
            return ab.ToString();
        }
    }
}
