//-----------------------------------------------------------------------------
// <copyright file="DefaultPerceivedRoomRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
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
                sb.Append($"<%red%><%b%>{room.Name}<%n%><%nl%>" + AnsiSequences.NewLine);
                sb.Append($"{room.Description}<%nl%>" + AnsiSequences.NewLine);
            }
            else
            {
                sb.Append($"You cannot perceive much of note about the room." + AnsiSequences.NewLine);
            }

            int insertAt = sb.Length;

            // TODO: Perhaps group things in the room by things you can pick up, things that are alive, etc?
            //   var senses = viewer.Behaviors.FindFirst<SensesBehavior>();
            //   var exits = senses.PerceiveExits();        and also render closable exits like doors nicely; "(closed)"?
            //   var entities = senses.PerceiveEntities();  and also render players nicely; "(AFK)" etc?
            //   var items = senses.PerceiveItems();        and also track tick or build sense-specific strings (like hearing only while blind...)

            bool hasNoticedSomething = false;

            // Handle exits differently from other Thing types
            var exits = senses.PerceiveExits();        // TODO: Aso render closable exits like doors nicely; "(closed)"?
            if (exits.Count > 0)
            {
                sb.Append($"  routes: ");
                foreach (var exit in exits)
                {
                    sb.Append($"<%magenta%>{exit}<%n%>, ");
                    hasNoticedSomething = true;
                }
                sb.Length--;
                sb.Length--;
                sb.Append(AnsiSequences.NewLine);
            }

            foreach (var presentThing in room.Children)
            {
                if (senses.CanPerceiveThing(presentThing) &&
                    presentThing != viewer &&
                    !presentThing.HasBehavior<ExitBehavior>())
                {
                    sb.Append($"  <%magenta%>{presentThing.FullName}<%n%>" + AnsiSequences.NewLine);
                    hasNoticedSomething = true;
                }
            }

            if (hasNoticedSomething)
            {
                sb.Insert(insertAt, $"<%yellow%>Here you notice:<%n%>" + AnsiSequences.NewLine);
            }
            else
            {
                sb.Insert(insertAt, $"<%yellow%>You notice nothing else inside the room.<%n%>");
            }

            return sb.ToString();
        }
    }
}
