//-----------------------------------------------------------------------------
// <copyright file="DefaultPerceivedRoomRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    using System.Linq;
    using System.Text;

    [RendererExports.PerceivedRoom(0)]
    public class DefaultPerceivedRoomRenderer : RendererDefinitions.PerceivedRoom
    {
        public override string Render(Thing viewer, Thing room)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"<%red%><%b%>{room.Name}<%n%><%nl%>");
            sb.AppendLine($"{room.Description}<%nl%>");

            // TODO: Perhaps group things in the room by things you can pick up, things that are alive, etc?
            //   var senses = viewer.Behaviors.FindFirst<SensesBehavior>();
            //   var exits = senses.PerceiveExits();
            //   var entities = senses.PerceiveEntities();
            //   var items = senses.PerceiveItems();
            if (room.Children.Any())
            {
                sb.AppendLine($"<%yellow%>Here you see:<%n%>");
                foreach (var presentThing in room.Children)
                {
                    sb.AppendLine($"  <%magenta%>{presentThing.FullName}<%n%>");
                }
            }

            return sb.ToString();
        }
    }
}
