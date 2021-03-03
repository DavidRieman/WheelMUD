//-----------------------------------------------------------------------------
// <copyright file="DefaultCommandsCategoriesRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    [RendererExports.CommandsCategories(0)]
    public class DefaultCommandsCategoriesRenderer : RendererDefinitions.CommandsCategories
    {
        public override string Render(TerminalOptions terminalOptions, IEnumerable<Command> commands)
        {
            // Build a list of categories for the commands available to this player.
            var ab = new AnsiBuilder();
            ab.AppendLine("Please specify a command category:");

            foreach (CommandCategory category in Enum.GetValues(typeof(CommandCategory)))
            {
                if (category == CommandCategory.None) continue;
                var matchingcommands = commands.Where(command => command.Category.HasFlag(category));
                if (matchingcommands.Any())
                {
                    ab.AppendLine($"<%mxpsecureline%><send \"commands {category}\">{category}</send> ({matchingcommands.Count()})");
                }
            }

            return ab.ToString();
        }
    }
}
