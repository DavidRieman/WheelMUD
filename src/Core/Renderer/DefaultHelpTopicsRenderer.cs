//-----------------------------------------------------------------------------
// <copyright file="DefaultHelpTopicsRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Text;
    using WheelMUD.Interfaces;

    [RendererExports.HelpTopics(0)]
    public class DefaultHelpTopicsRenderer : RendererDefinitions.HelpTopics
    {
        public override string Render(ITerminal terminal)
        {
            var sb = new StringBuilder();
            sb.AppendAnsiLine("TODO: LIST OUT ALL HELP TOPICS FOUND VIA HelpManager.");
            sb.AppendAnsiLine();
            sb.AppendAnsiLine("You can also use the \"commands\" command to list out commands, and you can get help");
            sb.AppendAnsiLine("for a specific command with \"help <command name>\"");
            return sb.ToString();
        }
    }
}
