//-----------------------------------------------------------------------------
// <copyright file="DefaultHelpTopicRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System;
    using System.Text;
    using WheelMUD.Interfaces;

    [RendererExports.HelpCommand(0)]
    public class DefaultHelpCommandRenderer : RendererDefinitions.HelpCommand
    {
        public override string Render(ITerminal terminal, Command command)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<%yellow%>{command.Name.ToUpper()}<%n%>:");
            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                sb.AppendLine(command.Description);
            }
            // TODO: Add command.Usage?  Rename Example to "Examples"?
            if (!string.IsNullOrWhiteSpace(command.Example))
            {
                sb.AppendLine($"<%yellow%>Example Usage<%n%>:");
                sb.AppendLine(command.Example);
            }
            // TODO: Add command.SeeAlso?
            return sb.ToString();
        }
    }
}
