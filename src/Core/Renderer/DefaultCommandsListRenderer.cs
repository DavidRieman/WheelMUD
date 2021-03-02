//-----------------------------------------------------------------------------
// <copyright file="DefaultCommandsListRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;
using WheelMUD.Utilities;
using System.Collections.Generic;

namespace WheelMUD.Core
{
    [RendererExports.CommandsList(0)]
    public class DefaultCommandsListRenderer : RendererDefinitions.CommandsList
    {
        public override string Render(TerminalOptions terminalOptions, IEnumerable<Command> commands, string categoryName)
        {
            var ab = new AnsiBuilder();
            ab.AppendLine($"<%yellow%>{categoryName} commands<%n%>:");
            foreach (var command in commands)
            {
                ab.AppendLine($"<%mxpsecureline%><send \"help {command.Name}\">{command.Name.PadRight(15)}</send> {command.Description}");
            }
            return ab.ToString();
        }
    }
}
