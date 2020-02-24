//-----------------------------------------------------------------------------
// <copyright file="DefaultPerceivedThingRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core
{
    [RendererExports.PerceivedThing(0)]
    public class DefaultPerceivedThingRenderer : RendererDefinitions.PerceivedThing
    {
        public override string Render(Thing viewer, Thing viewedThing)
        {
            return $"<%cyan%><%b%>{viewedThing.Name}<%n%><%nl%>{viewedThing.Description}<%nl%>";
        }
    }
}
