//-----------------------------------------------------------------------------
// <copyright file="DefaultInventoryRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Linq;
using WheelMUD.Server;

namespace WheelMUD.Core
{
    [RendererExports.Inventory(0)]
    public class DefaultInventoryRenderer : RendererDefinitions.Inventory
    {
        public override OutputBuilder Render(TerminalOptions terminalOptions, Thing player)
        {
            var senses = player.FindBehavior<SensesBehavior>();
            var output = new OutputBuilder();

            var invThings = player.Children.Where(presentThing => senses.CanPerceiveThing(presentThing)).ToArray();

            output.AppendLine(invThings.Length > 0
                ? "<%yellow%>Searching your inventory, you find:<%n%>"
                : "<%yellow%>You found no inventory.<%n%>");

            foreach (var presentThing in invThings)
            {
                output.AppendLine($"  <%magenta%>{presentThing.FullName}<%n%>");
            }

            return output;
        }
    }
}
