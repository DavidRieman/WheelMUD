//-----------------------------------------------------------------------------
// <copyright file="DefaultWhoRenderer.cs" company="WheelMUD Development Team">
//   Copyright (c) WheelMUD Development Team.  See LICENSE.txt.  This file is 
//   subject to the Microsoft Public License.  All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace WheelMUD.Core.Renderer
{
    using System;

    /// <summary>The default "who" command output renderer.</summary>
    /// <remarks>
    /// The default "who" renderer may simply show all online players. One might wish to build a "who" renderer
    /// with higher priority to customize things like hiding invisible admins, showing AFK states, etc.
    /// </remarks>
    [ExportWhoRenderer(0)]
    public class DefaultWhoRenderer : RendererDefinitions.Who
    {
        public override string Render(Thing player)
        {
            return "TODO: MOVE WHO RENDERING FROM WHO.CS!";
        }
    }
}
