//-----------------------------------------------------------------------------
// <copyright file="DefaultPerceivedThingRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

using WheelMUD.Server;

namespace WheelMUD.Core
{
    [RendererExports.PerceivedThing(0)]
    public class DefaultPerceivedThingRenderer : RendererDefinitions.PerceivedThing
    {
        public override string Render(TerminalOptions terminalOptions, Thing viewer, Thing viewedThing)
        {
            var senses = viewer.FindBehavior<SensesBehavior>();
            return senses.CanPerceiveThing(viewedThing) ? 
                $"You examine <%cyan%><%b%>{viewedThing.Name}<%n%>:<%nl%>{viewedThing.Description}<%nl%>" : "You cannot perceive that thing.";
        }
    }
}
