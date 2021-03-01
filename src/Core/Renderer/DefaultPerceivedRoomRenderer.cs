//-----------------------------------------------------------------------------
// <copyright file="DefaultPerceivedRoomRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    [RendererExports.PerceivedRoom(0)]
    public class DefaultPerceivedRoomRenderer : RendererDefinitions.PerceivedRoom
    {
        public override string Render(Thing viewer, Thing room)
        {
            var senses = viewer.FindBehavior<SensesBehavior>();
            var sb = new AnsiBuilder();
            if (senses.CanPerceiveThing(room))
            {
                sb.AppendLine($"<%red%><%b%>{room.Name}<%n%><%nl%>");
                sb.AppendLine($"{room.Description}<%nl%>");
            }
            else
            {
                sb.AppendLine("You cannot perceive much of note about the room.");
            }

            // TODO: Perhaps group things in the room by things you can pick up, things that are alive, etc?
            //   var senses = viewer.Behaviors.FindFirst<SensesBehavior>();
            //   var exits = senses.PerceiveExits();        and also render closable exits like doors nicely; "(closed)"?
            //   var entities = senses.PerceiveEntities();  and also render players nicely; "(AFK)" etc?
            //   var items = senses.PerceiveItems();        and also track tick or build sense-specific strings (like hearing only while blind...)

            var exits = senses.PerceiveExits();
            
            if (exits.Count > 0 || room.Children.Count > 0)
            {
                sb.AppendLine("<%yellow%>Here you notice:<%n%>");
            }
            else
            {
                sb.AppendLine("<%yellow%>You notice nothing else inside the room.<%n%>");
            }

            // Handle exits differently from other Thing types
                   // TODO: Also render closable exits like doors nicely; "(closed)"?
            if (exits.Count > 0)
            {
                sb.Append($"  routes: ");

                for (var i = 0; i < exits.Count; i++)
                {
                    sb.Append($"<%magenta%>{exits[i]}<%n%>");
                    if(i != exits.Count - 1)
                        sb.Append(", ");
                }
                
                sb.AppendLine();
            }

            foreach (var presentThing in room.Children)
            {
                if (!senses.CanPerceiveThing(presentThing) || presentThing == viewer ||
                    presentThing.HasBehavior<ExitBehavior>()) continue;
                
                sb.AppendLine($"  <%magenta%>{presentThing.FullName}<%n%>");
            }

            return sb.ToString();
        }
    }
}
