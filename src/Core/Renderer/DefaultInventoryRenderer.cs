//-----------------------------------------------------------------------------
// <copyright file="DefaultInventoryRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Text;

namespace WheelMUD.Core.Renderer
{
    [RendererExports.Inventory(0)]
    public class DefaultInventoryRenderer : RendererDefinitions.Inventory
    {
        public override string Render(Thing player)
        {
            var senses = player.FindBehavior<SensesBehavior>();
            var sb = new StringBuilder();

            bool hasNoticedSomething = false;
            foreach (var presentThing in player.Children)
            {
                if (senses.CanPerceiveThing(presentThing))
                {
                    if (!hasNoticedSomething)
                    {
                        sb.AppendLine($"<%yellow%>Searching your inventory, you find:<%n%>");
                        hasNoticedSomething = true;
                    }
                    sb.AppendLine($"  <%magenta%>{presentThing.FullName}<%n%>");
                }
            }
            if (!hasNoticedSomething)
            {
                sb.AppendLine($"<%yellow%>You found no inventory.<%n%>");
            }

            return sb.ToString();
        }
    }
}
