//-----------------------------------------------------------------------------
// <copyright file="DefaultPerceivedRoomRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Linq;
using System.Text;
using WheelMUD.Utilities;

namespace WheelMUD.Core
{
    [RendererExports.PerceivedRoom(0)]
    public class DefaultPerceivedRoomRenderer : RendererDefinitions.PerceivedRoom
    {
        public override string Render(Thing viewer, Thing room)
        {
            var senses = viewer.FindBehavior<SensesBehavior>();
            var sb = new StringBuilder();
            if (senses.CanPerceiveThing(room))
            {
                sb.AppendAnsiLine($"<%red%><%b%>{room.Name}<%n%>");
                sb.AppendAnsiLine(room.Description);
            }
            else
            {
                sb.AppendAnsiLine($"You cannot perceive much of note about the room.");
            }

            // TODO: Perhaps group things in the room by things you can pick up, things that are alive, etc?
            //   var senses = viewer.Behaviors.FindFirst<SensesBehavior>();
            //   var exits = senses.PerceiveExits();        and also render closable exits like doors nicely; "(closed)"?
            //   var entities = senses.PerceiveEntities();  and also render players nicely; "(AFK)" etc?
            //   var items = senses.PerceiveItems();        and also track tick or build sense-specific strings (like hearing only while blind...)

            // Handle exits differently from other Thing types
            // TODO: Also render closable exits like doors nicely; "(closed)"?
            // TODO: For viewer that is PlayerBehavior with negotiated MXP connection, render with embedded command links for click-to-execute support?
            var coloredExits = from exit in senses.PerceiveExits() select $"<%magenta%>{exit}<%n%>";

            // TODO: Color the parts of the thing names which are also legit keywords for the thing...
            // TODO: For viewer that is PlayerBehavior with negotiated MXP connection, render with embedded command links for click-to-execute support?
            var coloredThings = from thing in room.Children
                                where senses.CanPerceiveThing(thing) && thing != viewer && !thing.HasBehavior<ExitBehavior>()
                                select $"  {thing.FullName}<%n%>";

            if (coloredExits.Any() || coloredThings.Any())
            {
                sb.AppendAnsiLine($"<%yellow%>Here you notice:<%n%>");
                if (coloredExits.Any())
                {
                    sb.AppendAnsiLine($"  Routes: {string.Join(", ", coloredExits)}");
                }
                foreach (var coloredThing in coloredThings)
                {
                    sb.AppendAnsiLine(coloredThing);
                }
            }
            else
            {
                sb.AppendAnsiLine($"<%yellow%>You notice nothing else inside the room.<%n%>");
            }

            return sb.ToString();
        }
    }
}
