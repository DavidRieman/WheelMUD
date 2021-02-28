//-----------------------------------------------------------------------------
// <copyright file="DefaultCommandsCategoriesRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;

namespace WheelMUD.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [RendererExports.CommandsCategories(0)]
    public class DefaultCommandsCategoriesRenderer : RendererDefinitions.CommandsCategories
    {
        public override string Render(TerminalOptions terminalOptions, IEnumerable<Command> commands)
        {
            // Build a list of categories for the commands available to this player.
            var sb = new StringBuilder();
            sb.AppendLine("Please specify a command category:");

            foreach (CommandCategory category in Enum.GetValues(typeof(CommandCategory)))
            {
                if (category != CommandCategory.None)
                {
                    var matchingcommands = commands.Where(command => command.Category.HasFlag(category));
                    if (matchingcommands.Any())
                    {
                        sb.AppendLine($"<%mxpsecureline%><send \"commands {category}\">{category}</send> ({matchingcommands.Count()})");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
