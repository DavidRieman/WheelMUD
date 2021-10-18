//-----------------------------------------------------------------------------
// <copyright file="DefaultCommandsListRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;
using System.Collections.Generic;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    [RendererExports.CommandsList(0)]
    public class DefaultCommandsListRenderer : RendererDefinitions.CommandsList
    {
        public override OutputBuilder Render(TerminalOptions terminalOptions, IEnumerable<Command> commands, string categoryName)
        {
            var output = new OutputBuilder().AppendLine($"<%yellow%>{categoryName} commands<%n%>:");
            foreach (var command in commands)
            {
                // NEED AN ALTERNATE NON-MXP VERSION, AND NEED SECURE LINE API TO BE THE ONLY WAY TO SEND SECURE LINES - Name to make very clear it cannot contain user-generated content? "AppendTrustedSecureLine" with extra comments?
                output.AppendLine($"{AnsiSequences.MxpSecureLine}<send \"help {command.Name}\">{command.Name.PadRight(15)}</send> {command.Description}");
            }
            return output;
        }
    }
}
