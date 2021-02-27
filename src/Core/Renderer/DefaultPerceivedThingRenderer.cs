//-----------------------------------------------------------------------------
// <copyright file="DefaultPerceivedThingRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Renderer
{
    [RendererExports.PerceivedThing(0)]
    public class DefaultPerceivedThingRenderer : RendererDefinitions.PerceivedThing
    {
        public override string Render(Thing viewer, Thing viewedThing)
        {
            var senses = viewer.FindBehavior<SensesBehavior>();
            if (senses.CanPerceiveThing(viewedThing))
            {
                return $"You examine <%cyan%><%b%>{viewedThing.Name}<%n%>:<%nl%>{viewedThing.Description}<%nl%>";
            }
            return "You cannot perceive that thing.";
        }
    }
}
