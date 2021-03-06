//-----------------------------------------------------------------------------
// <copyright file="DefaultHelpTopicRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;

namespace WheelMUD.Core
{
    [RendererExports.HelpCommand(0)]
    public class DefaultHelpCommandRenderer : RendererDefinitions.HelpCommand
    {
        public override OutputBuilder Render(Command command)
        {
            var output = new OutputBuilder();
            output.AppendLine($"<%yellow%>{command.Name.ToUpper()}<%n%>:");
            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                output.AppendLine(command.Description);
            }
            // TODO: Add command.Usage?  Rename Example to "Examples" as string[]?
            if (!string.IsNullOrWhiteSpace(command.Example))
            {
                output.AppendLine("<%yellow%>USAGE<%n%>:");
                output.AppendLine(command.Example);
            }
            // TODO: Add command.SeeAlso as string[]?
            return output;
        }
    }
}
