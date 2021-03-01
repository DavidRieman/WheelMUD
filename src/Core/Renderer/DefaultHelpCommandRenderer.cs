//-----------------------------------------------------------------------------
// <copyright file="DefaultHelpTopicRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    [RendererExports.HelpCommand(0)]
    public class DefaultHelpCommandRenderer : RendererDefinitions.HelpCommand
    {
        public override string Render(TerminalOptions terminalOptions, Command command)
        {
            var sb = new AnsiBuilder();
            sb.AppendLine($"<%yellow%>{command.Name.ToUpper()}<%n%>:");
            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                sb.AppendLine(command.Description);
            }
            // TODO: Add command.Usage?  Rename Example to "Examples" as string[]?
            if (!string.IsNullOrWhiteSpace(command.Example))
            {
                sb.AppendLine("<%yellow%>USAGE<%n%>:");
                sb.AppendLine(command.Example);
            }
            // TODO: Add command.SeeAlso as string[]?
            return sb.ToString();
        }
    }
}
