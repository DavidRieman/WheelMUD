//-----------------------------------------------------------------------------
// <copyright file="DefaultPerceivedThingRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Renderer
{
    using System;

    [ExportPerceivedThingRenderer(0)]
    public class DefaultPerceivedThingRenderer : RendererDefinitions.PerceivedThing
    {
        public override string Render(Thing viewer, Thing viewedThing)
        {
            throw new NotImplementedException();
        }
    }
}
