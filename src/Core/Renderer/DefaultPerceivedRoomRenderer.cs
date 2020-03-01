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
            var senses = viewer.FindBehavior<SensesBehavior>();
            var sb = new StringBuilder();
            if (senses.CanPerceiveThing(room))
            {
                sb.AppendLine($"<%red%><%b%>{room.Name}<%n%><%nl%>");
                sb.AppendLine($"{room.Description}<%nl%>");
            }
            else
            {
                sb.AppendLine($"You cannot perceive much of note about the room.");
            }

            // TODO: Perhaps group things in the room by things you can pick up, things that are alive, etc?
            //   var senses = viewer.Behaviors.FindFirst<SensesBehavior>();
            //   var exits = senses.PerceiveExits();        and also render closable exits like doors nicely; "(closed)"?
            //   var entities = senses.PerceiveEntities();  and also render players nicely; "(AFK)" etc?
            //   var items = senses.PerceiveItems();        and also track tick or build sense-specific strings (like hearing only while blind...)
            bool hasNoticedSomething = false;
            foreach (var presentThing in room.Children)
            {
                if (senses.CanPerceiveThing(presentThing))
                {
                    if (!hasNoticedSomething)
                    {
                        sb.AppendLine($"<%yellow%>Here you notice:<%n%>");
                        hasNoticedSomething = true;
                    }
                    sb.AppendLine($"  <%magenta%>{presentThing.FullName}<%n%>");
                }
            }
            if (!hasNoticedSomething)
            {
                sb.AppendLine($"<%yellow%>You notice nothing else inside the room.<%n%>");
            }

            return sb.ToString();
        }
    }
}
