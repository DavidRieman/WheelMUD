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

namespace WheelMUD.Core
{
    [RendererExports.CommandsCategories(0)]
    public class DefaultCommandsCategoriesRenderer : RendererDefinitions.CommandsCategories
    {
        public override OutputBuilder Render(IEnumerable<Command> commands)
        {
            // Build a list of categories for the commands available to this player.
            var output = new OutputBuilder();
            output.AppendLine("Please specify a command category:");

            foreach (CommandCategory category in Enum.GetValues(typeof(CommandCategory)))
            {
                if (category == CommandCategory.None) continue;
                var matchingcommands = commands.Where(command => command.Category.HasFlag(category));
                if (matchingcommands.Any())
                {
                    output.AppendLine($"<%mxpsecureline%><send \"commands {category}\">{category}</send> ({matchingcommands.Count()})");
                }
            }

            return output;
        }
    }
}
