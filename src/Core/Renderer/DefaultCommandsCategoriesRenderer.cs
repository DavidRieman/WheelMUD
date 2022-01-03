//-----------------------------------------------------------------------------
// <copyright file="DefaultCommandsCategoriesRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using WheelMUD.Server;

namespace WheelMUD.Core
{
    [RendererExports.CommandsCategories(0)]
    public class DefaultCommandsCategoriesRenderer : RendererDefinitions.CommandsCategories
    {
        public override OutputBuilder Render(TerminalOptions terminalOptions, IEnumerable<Command> commands)
        {
            // Build a list of categories for the commands available to this player.
            var output = new OutputBuilder().AppendLine("Please specify a command category:");

            foreach (CommandCategory category in Enum.GetValues(typeof(CommandCategory)))
            {
                if (category == CommandCategory.None) continue;
                var matchingcommands = commands.Where(command => command.Category.HasFlag(category));
                if (matchingcommands.Any())
                {
                    // THIS DOESN'T WORK ANYMORE - NEED TO USE SECURE LINE API AND HAVE SEPARATE OUTPUT FOR NON-MXP CLIENTS!
                    output.AppendLine($"<%mxpsecureline%><send \"commands {category}\">{category}</send> ({matchingcommands.Count()})");
                }
            }

            return output;
        }
    }
}
