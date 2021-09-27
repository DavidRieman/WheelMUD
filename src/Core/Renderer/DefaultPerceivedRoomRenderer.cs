//-----------------------------------------------------------------------------
// <copyright file="DefaultPerceivedRoomRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System.Linq;
using WheelMUD.Server;

namespace WheelMUD.Core
{
    [RendererExports.PerceivedRoom(0)]
    public class DefaultPerceivedRoomRenderer : RendererDefinitions.PerceivedRoom
    {
        public override OutputBuilder Render(TerminalOptions terminalOptions, Thing viewer, Thing room)
        {
            var senses = viewer.FindBehavior<SensesBehavior>();
            var output = new OutputBuilder();
            if (senses.CanPerceiveThing(room))
            {
                output.AppendLine($"<%red%><%b%>{room.Name}<%n%>");
                output.AppendLine(room.Description);
            }
            else
            {
                output.AppendLine("You cannot perceive much of note about the room.");
            }

            // TODO: Perhaps group things in the room by things you can pick up, things that are alive, etc?
            //   var entities = senses.PerceiveEntities();  and also render players nicely; "(AFK)" etc?
            //   var items = senses.PerceiveItems();        and also track tick or build sense-specific strings (like hearing only while blind...)

            // Handle exits differently from other Thing types
            // TODO: Also render closable exits like doors nicely; "(closed)"?
            // TODO: For viewer that is PlayerBehavior with negotiated MXP connection, render with embedded command links for click-to-execute support?
            var outputExits = from exit in senses.PerceiveExits() select $"<%magenta%>{exit}<%n%>";

            // TODO: Color the parts of the thing names which are also legit keywords for the thing...
            // TODO: For viewer that is PlayerBehavior with negotiated MXP connection, render with embedded command links for click-to-execute support?
            var outputThings = from thing in room.Children
                               where senses.CanPerceiveThing(thing) && thing != viewer && !thing.HasBehavior<ExitBehavior>()
                               select $"  {thing.FullName}<%n%>";

            if (outputExits.Any() || outputThings.Any())
            {
                output.AppendLine("<%yellow%>Here you notice:<%n%>");
                if (outputExits.Any())
                {
                    output.AppendLine($"  Routes: {string.Join(", ", outputExits)}");
                }
                foreach (var outputThing in outputThings)
                {
                    output.AppendLine(outputThing);
                }
            }
            else
            {
                output.AppendLine("<%yellow%>You notice nothing else inside the room.<%n%>");
            }

            return output;
        }
    }
}
